using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExamplePaperAirplaneProjectile : ModProjectile
	{
		public override void SetDefaults() {
			Projectile.width = 10; // The 宽度 的 弹幕
			Projectile.height = 10; // The 高度 的 弹幕

			Projectile.aiStyle = -1; // We are 设置 the aiStyle to -1 to use the custom AI below. If just want the vanilla behavior, you can set the aiStyle to 159.

			Projectile.friendly = true; // Can the 弹幕 deal 伤害 to enemies?
			Projectile.DamageType = DamageClass.Ranged; // 设置 the 伤害 类型 to ranged 伤害.

			// 设置ting this to 真 will 停止 the 弹幕 from automatically flipping its 精灵 when changing directions.
			// vanilla paper airplanes have this set to 真.
			// 如果 this is 真 the 弹幕 won't flip its 精灵 vertically while doing a 循环, but the paper airplane 可以 upside down if it is shot one 方向 然后 turns around on its own.
			// 设置 to 假 if you want the 弹幕 to always be 右 side up.
			Projectile.manualDirectionChange = true;

			// 如果 you are using 弹幕.aiStyle = 159, 设置 the AIType isn't necessary here because the two types of vanilla paper airplanes aiStyles have the same AI.
			// AIType = ProjectileID.PaperAirplaneA;
		}

		// 这是 the behavior 的 paper airplane.
		// 如果 you just want the same vanilla behavior, you can instead set 弹幕.aiStyle = 159 in SetDefaults and 删除 this AI() section.
		public override void AI() {
			// All projectiles have timers that 帮助 to 延迟 certain events
			// 弹幕.ai[0], 弹幕.ai[1] — timers that are automatically synchronized 在 客户端 and 服务器

			// 这将 run only once 一旦 the 弹幕 spawns.
			if (Projectile.ai[1] == 0f) {
				Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt(); // If it is moving 右, then set 弹幕.方向 to 1. If it is moving 左, then set 弹幕.方向 to -1.
				Projectile.rotation = Projectile.velocity.ToRotation(); // 设置 the 旋转 based 在 速度.
				Projectile.ai[1] = 1f; // 设置 弹幕.ai[1] to 1. This is only 用于 make this section of code run only once.
				Projectile.ai[0] = -Main.rand.Next(30, 80); // 设置 弹幕.ai[0] to a 随机 数字 from -30 to -79.
				Projectile.netUpdate = true; // 同步 the 弹幕 in a multiplayer game.
			}

			// Kill the 弹幕 if it touches a liquid. (It will automatically get killed by touching a 图格. You can change that by returning 假 in OnTileCollide())
			if (Projectile.wet && Projectile.owner == Main.myPlayer) {
				Projectile.Kill();
			}

			Projectile.ai[0] += 1f; // Increase 弹幕.ai[0] by 1 每个 tick. Remember, there are 60 ticks per second.

			Vector2 rotationVector = Projectile.rotation.ToRotationVector2() * 8f; // 获取 the 旋转 的 弹幕.

			float ySinModifier = (float)Math.Sin((float)Math.PI * 2f * (float)(Main.timeForVisualEffects % 90.0 / 90.0)) * Projectile.direction * Main.WindForVisuals; // 这将 make the 弹幕 fly in a sine wave fashion.

			Vector2 newVelocity = rotationVector + new Vector2(Main.WindForVisuals, ySinModifier); // 创建 a new 速度 using the 旋转 and wind.

			bool directionSameAsWind = Projectile.direction == Math.Sign(Main.WindForVisuals) && Projectile.velocity.Length() > 3f; // 真 if the 弹幕 is moving the same 方向 as the wind and is not moving slowly.
			bool readyForFlip = Projectile.ai[0] >= 20f && Projectile.ai[0] <= 69f; // 真 if 弹幕.ai[0] is between 20 and 69

			// Once 弹幕.ai[0] reaches 70...
			if (Projectile.ai[0] == 70f) {
				Projectile.ai[0] = -Main.rand.Next(120, 600); // 设置 it back to a 随机 数字 from -120 to -599.
			}

			// Do a flip! This will cause the 弹幕 to fly in a 循环 if directionSameAsWind and readyForFlip are 真.
			if (readyForFlip && directionSameAsWind) {
				float lerpValue = Utils.GetLerpValue(0f, 30f, Projectile.ai[0], clamped: true);
				newVelocity = rotationVector.RotatedBy((-Projectile.direction) * ((float)Math.PI * 2f) * 0.02f * lerpValue);
			}

			Projectile.velocity = newVelocity.SafeNormalize(Vector2.UnitY) * Projectile.velocity.Length(); // 设置 the 速度 到 值 we calculated above.

			// 如果 it is flying normally. i.e. not flying a 循环.
			if (!(readyForFlip && directionSameAsWind)) {
				float yModifier = MathHelper.Lerp(0.15f, 0.05f, Math.Abs(Main.WindForVisuals));

				// Half of 时间, decrease the y 速度 一点.
				if (Projectile.timeLeft % 40 < 20) {
					Projectile.velocity.Y -= yModifier;
				}
				// other half of 时间, increase the y 速度 一点.
				else {
					Projectile.velocity.Y += yModifier;
				}

				// Cap the y 速度 so the 弹幕 falls slowly and doesn't rise too quickly.
				// MathHelper.Clamp() allows you to set a 最小 and 最大 值. In this case, the result will always be between -2f and 2f (inclusive).
				Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y, -2f, 2f);

				// 设置 the x 速度.
				// MathHelper.Clamp() allows you to set a 最小 and 最大 值. In this case, the result will always be between -6f and 6f (inclusive).
				Projectile.velocity.X = MathHelper.Clamp(Projectile.velocity.X + Main.WindForVisuals * 0.006f, -6f, 6f);

				// Switch 方向 when the current 速度 and the oldVelocity have different signs.
				if (Projectile.velocity.X * Projectile.oldVelocity.X < 0f) {
					Projectile.direction *= -1; // Reverse the 方向
					Projectile.ai[0] = -Main.rand.Next(120, 300); // 设置 弹幕.ai[0] to a 随机 数字 from -120 to -599.
					Projectile.netUpdate = true; // 同步 the 弹幕 in a multiplayer game.
				}
			}

			// 设置 the 旋转 and spriteDirection
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.spriteDirection = Projectile.direction;

			// Let's add some dust for special 效果. In this case, it runs 每个 other tick (30 ticks per second).
			if (Projectile.timeLeft % 2 == 0) {
				Dust.NewDustPerfect(new Vector2(Projectile.Center.X - (Projectile.width * Projectile.direction), Projectile.Center.Y), ModContent.DustType<Dusts.Sparkle>(), null, 0, default, 0.5f); //Here we 生成 the dust 在 back 的 弹幕 with half 缩放.
			}
		}

		// 我们 需要 draw the 弹幕 manually. If you don't include this, the 弹幕 将 facing the wrong 方向 when flying 左.
		public override bool PreDraw(ref Color lightColor) {
			// 这是 where we specify which way to flip the 精灵. If the 弹幕 is moving 到 左, then flip it vertically.
			SpriteEffects spriteEffects = ((Projectile.spriteDirection <= 0) ? SpriteEffects.FlipVertically : SpriteEffects.None);

			// 获取ting 纹理 of 弹幕
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			// 获取 the currently selected 帧 在 纹理.
			Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);

			Vector2 origin = sourceRectangle.Size() / 2f;

			// 应用ing lighting and draw our 弹幕
			Color drawColor = Projectile.GetAlpha(lightColor);
			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

			// It's important to 返回 假, 否则 we also draw the original 纹理.
			return false;
		}

		public override void OnKill(int timeLeft) {
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position); // Play a 声音 when the 弹幕 dies. In this case, 即 when it hits a 方块 or a liquid.

			if (Projectile.owner == Main.myPlayer && !Projectile.noDropItem) {
				int dropItemType = ModContent.ItemType<Items.ExamplePaperAirplane>(); // This the 项 we want the paper airplane to 放下.
				int newItem = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Hitbox, dropItemType); // 创建 a new 项 在 世界.
				Main.item[newItem].noGrabDelay = 0; // 设置 the new 项 to be 能够 be picked up instantly

				// 在这里 we 需要 make sure the 项 is synced in multiplayer games.
				if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0) {
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
				}
			}

			// Let's add some dust for special 效果.
			for (int i = 0; i < 10; i++) {
				Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Sparkle>());
			}
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
			if (target.type == NPCID.GraniteGolem) {
				// Paper beats Rock!
				// 使用 FinalDamage since the 弹幕 isn't conceptually stronger, the 目标 is weaker to this 武器.
				modifiers.FinalDamage *= 20f; // 20x 伤害...isn't much since 防御 is high and 伤害 is low.
			}
		}
	}
}
