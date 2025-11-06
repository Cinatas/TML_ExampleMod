using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Pets.MinionBossPet
{
	// 你 can 查找 a simple 宠物 example in ExampleMod\Content\Pets\ExamplePet
	// This 宠物 uses custom AI and drawing to make it more special (It's a Master 模式 Boss 宠物 after all)
	// It behaves similarly 到 Creeper Egg or Suspicious Grinning Eye pets, but takes some visual properties from ExampleMod's 仆从 Boss
	public class MinionBossPetProjectile : ModProjectile
	{
		// 这是 a ref 属性, lets us write 弹幕.ai[0] as whatever 名称 we want
		public ref float AlphaForVisuals => ref Projectile.ai[0];

		// This 弹幕 uses an additional 纹理 for drawing
		public static Asset<Texture2D> EyeAsset;

		public override void Load() {
			// 加载/缓存 the additional 纹理
			EyeAsset = ModContent.Request<Texture2D>(Texture + "_Eye");
		}

		public override void SetStaticDefaults() {
			Main.projFrames[Projectile.type] = 6;
			Main.projPet[Projectile.type] = true;

			// 基本s of CharacterPreviewAnimations explained in ExamplePetProjectile
			// 注意 we define our own 方法 to use in .WithCode() below. This technically allows us to animate the 弹幕 manually using frameCounter and 帧 以及
			ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Projectile.type], 5)
				.WithOffset(-2, -22f)
				.WithCode(CharacterPreviewCustomization);
		}

		public static void CharacterPreviewCustomization(Projectile proj, bool walking) {
			// 修改d floating from DelegateMethods.CharacterPreview.Float, this is technically not representative of how the 宠物 actually looks and moves ingame, but the Suspicious Grinning Eye has that too

			// 如果 you don't need to modify it, just call DelegateMethods.CharacterPreview.Float(proj, walking) directly here instead and change properties of your 宠物 after it.
			// 你 do not need this otherwise and can use the preset directly as showcased in ExamplePetProjectile
			float half = 0.5f;
			float timer = (float)Main.timeForVisualEffects % 60f / 60f;
			float speed = 1f; // This is normally 2
			proj.position.Y += 0f - half + (float)(Math.Cos(timer * MathHelper.TwoPi * speed) * half * 2f);

			// 我们 are only using this 方法 for one specific 弹幕, so it's fine to cast the ModProjectile directly like this
			MinionBossPetProjectile minion = (MinionBossPetProjectile)proj.ModProjectile;

			// Need to set the alpha to 1f to hide the eyes that would normally draw and show the actual 宠物
			minion.AlphaForVisuals = 1f;

			// 你 can use 弹幕.isAPreviewDummy 在 draw code instead, it depends if you prefer changing the conditions leading up 到 drawing, or the drawing itself
		}

		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.EyeOfCthulhuPet); // 复制 the stats 的 Suspicious Grinning Eye 弹幕

			Projectile.aiStyle = -1; // 使用 custom AI
		}

		public override Color? GetAlpha(Color lightColor) {
			return Color.White * AlphaForVisuals * Projectile.Opacity;
		}

		public override void PostDraw(Color lightColor) {
			// 绘制 surrounding eyes to mimic the Boss
			Texture2D eyeTexture = EyeAsset.Value;

			Vector2 offset = new Vector2(0, Projectile.gfxOffY); // Vertical 偏移 when the 弹幕 is changing elevation on tiles (does not apply to this particular 弹幕 because it is always airborne)
			Vector2 orbitingCenter = Projectile.Center + offset;

			// 不要 need to draw the eyes if the 宠物 is fully faded in
			if (AlphaForVisuals >= 1) {
				return;
			}

			int eyeCount = 10;
			for (int i = 0; i < eyeCount; i++) {
				Vector2 origin = Vector2.Zero; // Using 原点 as zero because the draw 位置 is the 中心
				Vector2 rotatedPos = (Vector2.UnitY * 24).RotatedBy(i / (float)eyeCount * MathHelper.TwoPi); // 创建 a vector of 长度 24 with a specific 旋转 based on 循环 索引
				Vector2 drawPos = orbitingCenter - Main.screenPosition + origin + rotatedPos; // 始终 important to substract Main.screenPosition to translate it into 屏幕 coordinates
				Color color = Color.White * (1f - AlphaForVisuals) * Projectile.Opacity; // 绘制 it in reversed alpha 到 弹幕

				// 使用 this instead of Main.spriteBatch.Draw so that dyes apply to it
				Main.EntitySpriteDraw(eyeTexture, drawPos, eyeTexture.Bounds, color, 0f, origin, 1f, SpriteEffects.None, 0);
			}
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];

			// 对于 organization, the AI is 拆分 into several methods defined below
			// They are NOT part 的 ModProjectile 类!
			CheckActive(player);

			bool movesFast = Movement(player);

			Animate(movesFast);

			AlphaForVisuals = GetAlphaForVisuals(player);
		}

		private void CheckActive(Player player) {
			// Keep the 弹幕 from disappearing as long as the 玩家 isn't dead and has the 宠物 增益
			if (!player.dead && player.HasBuff(ModContent.BuffType<MinionBossPetBuff>())) {
				Projectile.timeLeft = 2;
			}
		}

		private bool Movement(Player player) {
			// 处理s movement, returns 真 if moving fast (used for 动画)
			float velDistanceChange = 2f;

			// 计算s the desired resting 位置, 以及 as some vectors used in 速度/旋转 calculations
			int dir = player.direction;
			Projectile.direction = Projectile.spriteDirection = dir;

			Vector2 desiredCenterRelative = new Vector2(dir * 30, -30f);

			// 添加 some sine motion
			desiredCenterRelative.Y += (float)Math.Sin(Main.GameUpdateCount / 120f * MathHelper.TwoPi) * 5;

			Vector2 desiredCenter = player.MountedCenter + desiredCenterRelative;
			Vector2 betweenDirection = desiredCenter - Projectile.Center;
			float betweenSQ = betweenDirection.LengthSquared(); // It is recommended to operate on squares of distances, to 保存 computing 时间 on square-rooting

			if (betweenSQ > 1000f * 1000f || betweenSQ < velDistanceChange * velDistanceChange) {
				// 设置 位置 directly if too far away 从 玩家, or when near the desired 位置
				Projectile.Center = desiredCenter;
				Projectile.velocity = Vector2.Zero;
			}

			if (betweenDirection != Vector2.Zero) {
				Projectile.velocity = betweenDirection * 0.1f * 2;
			}

			bool movesFast = Projectile.velocity.LengthSquared() > 6f * 6f;

			if (movesFast) {
				// 如果 moving very fast, 旋转 the 弹幕 towards it smoothly
				float rotationVel = Projectile.velocity.X * 0.08f + Projectile.velocity.Y * Projectile.spriteDirection * 0.02f;
				if (Math.Abs(Projectile.rotation - rotationVel) >= MathHelper.Pi) {
					if (rotationVel < Projectile.rotation) {
						Projectile.rotation -= MathHelper.TwoPi;
					}
					else {
						Projectile.rotation += MathHelper.TwoPi;
					}
				}

				float rotationInertia = 12f;
				Projectile.rotation = (Projectile.rotation * (rotationInertia - 1f) + rotationVel) / rotationInertia;
			}
			else {
				// 如果 moving at regular speeds, 旋转 the 弹幕 towards its default 旋转 (0) smoothly 如有必要
				if (Projectile.rotation > MathHelper.Pi) {
					Projectile.rotation -= MathHelper.TwoPi;
				}

				if (Projectile.rotation > -0.005f && Projectile.rotation < 0.005f) {
					Projectile.rotation = 0f;
				}
				else {
					Projectile.rotation *= 0.96f;
				}
			}

			return movesFast;
		}

		private void Animate(bool movesFast) {
			int animationSpeed = 7;

			if (movesFast) {
				// Increase 动画 速度 if 弹幕 moves fast (less is faster)
				animationSpeed = 4;
			}

			// Animate all frames from 顶部 to 底部, going back 到 first
			Projectile.frameCounter++;
			if (Projectile.frameCounter > animationSpeed) {
				Projectile.frameCounter = 0;
				Projectile.frame++;

				if (Projectile.frame >= Main.projFrames[Projectile.type]) {
					Projectile.frame = 0;
				}
			}
		}

		private static float GetAlphaForVisuals(Player player) {
			// 0f on full life, 1f for below half life
			float lifeRatio = player.statLife / (float)player.statLifeMax2;
			return Utils.Clamp(2 * (1f - lifeRatio), 0f, 1f);
		}
	}
}
