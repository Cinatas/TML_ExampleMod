using ExampleMod.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleWhipProjectileAdvanced : ModProjectile
	{
		// 纹理 doesn't have the same 名称 as the 项, so this 属性 points to it.
		public override string Texture => "ExampleMod/Content/Projectiles/ExampleWhipProjectile";

		public override void SetStaticDefaults() {
			// This makes the 弹幕 use whip collision detection and allows flasks to be applied to it.
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true; // This prevents the 弹幕 from hitting through solid tiles.
			Projectile.extraUpdates = 1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.WhipSettings.Segments = 10;
			Projectile.WhipSettings.RangeMultiplier = 1.5f;
		}

		private float Timer {
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private float ChargeTime {
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override void AI() {
			Player owner = Main.player[Projectile.owner];
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // Without PiOver2, the 旋转 将 off by 90 degrees counterclockwise.

			Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
			// Vanilla uses Vector2.Dot(弹幕.速度, Vector2.UnitX) here. Dot Product returns the difference between two vectors, 0 meaning they are perpendicular.
			// 然而, the use of UnitX basically turns it into a more complicated way of checking if the 弹幕's 速度 is above or equal to zero 在 X axis.
			Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

			// 删除 these 3 lines if you don't want the charging mechanic
			if (!Charge(owner)) {
				return; // 计时器 doesn't 更新 while charging, freezing the 动画 在 开始.
			}

			Timer++;

			float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
			if (Timer >= swingTime || owner.itemAnimation <= 0) {
				Projectile.Kill();
				return;
			}

			owner.heldProj = Projectile.whoAmI;
			if (Timer == swingTime / 2) {
				// Plays a whipcrack 声音 在 提示 的 whip.
				List<Vector2> points = Projectile.WhipPointsForCollision;
				Projectile.FillWhipControlPoints(Projectile, points);
				SoundEngine.PlaySound(SoundID.Item153, points[points.Count - 1]);
			}

			// 生成 Dust along the whip 路径
			// 这是 the dust code used by Durendal. Consult the Terraria source code for even more examples, found in 弹幕.AI_165_Whip.
			float swingProgress = Timer / swingTime;
			// This code limits dust to only 生成 during the the actual swing.
			if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f && !Main.rand.NextBool(3)) {
				List<Vector2> points = Projectile.WhipPointsForCollision;
				points.Clear();
				Projectile.FillWhipControlPoints(Projectile, points);
				int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
				Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));
				int dustType = DustID.Enchanted_Gold;
				if (Main.rand.NextBool(3))
					dustType = DustID.TintableDustLighted;

				// 之后 choosing a randomized dust and a whip segment to 生成 from, dust is spawned.
				Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, dustType, 0f, 0f, 100, Color.White);
				dust.position = points[pointIndex];
				dust.fadeIn = 0.3f;
				Vector2 spinningPoint = points[pointIndex] - points[pointIndex - 1];
				dust.noGravity = true;
				dust.velocity *= 0.5f;
				// This math causes these dust to 生成 with a 速度 perpendicular 到 方向 的 whip segments, giving the impression 的 dust flying off like sparks.
				dust.velocity += spinningPoint.RotatedBy(owner.direction * ((float)Math.PI / 2f));
				dust.velocity *= 0.5f;
			}
		}

		// 此方法 handles a charging mechanic.
		// 如果 you 删除 this, also 删除 项.通道 = 真 从 项's SetDefaults.
		// 返回s 真 if fully charged
		private bool Charge(Player owner) {
			// 像 other whips, this whip updates twice per 帧 (弹幕.extraUpdates = 1), so 120 is equal to 1 second.
			if (!owner.channel || ChargeTime >= 120) {
				return true; // finished charging
			}

			ChargeTime++;

			if (ChargeTime % 12 == 0) // 1 segment per 12 ticks of charge.
				Projectile.WhipSettings.Segments++;

			// Increase 范围 up to 2x for full charge.
			Projectile.WhipSettings.RangeMultiplier += 1 / 120f;

			// 重置 the 动画 and 项 计时器 while charging.
			owner.itemAnimation = owner.itemAnimationMax;
			owner.itemTime = owner.itemTimeMax;

			return false; // still charging
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(ModContent.BuffType<ExampleWhipAdvancedDebuff>(), 240);
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(Projectile.damage * 0.7f); // Multihit 惩罚. Decrease the 伤害 the more enemies the whip hits.
		}

		// 此方法 draws a line between all points 的 whip, in case there's empty space between the sprites.
		private void DrawLine(List<Vector2> list) {
			Texture2D texture = TextureAssets.FishingLine.Value;
			Rectangle frame = texture.Frame();
			Vector2 origin = new Vector2(frame.Width / 2, 2);

			Vector2 pos = list[0];
			for (int i = 0; i < list.Count - 1; i++) {
				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2;
				Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
				Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}
		}

		public override bool PreDraw(ref Color lightColor) {
			List<Vector2> list = new List<Vector2>();
			Projectile.FillWhipControlPoints(Projectile, list);

			DrawLine(list);

			//Main.DrawWhip_WhipBland(弹幕, 列表);
			// code below is for custom drawing.
			// 如果 you don't want that, you can 删除 it all and instead call one of vanilla's DrawWhip methods, like above.
			// 然而, you must adhere to how they draw if you do.

			SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++) {
				// These two values are set to suit this 弹幕's 精灵, but won't necessarily work for your own.
				// 你 can change them if they don't!
				Rectangle frame = new Rectangle(0, 0, 10, 26); // The 大小 的 处理 (measured in pixels)
				Vector2 origin = new Vector2(5, 8); // 偏移 for where the 玩家's hand will 开始 measured 从 顶部 左 的 图像.
				float scale = 1;

				// These statements determine what part 的 spritesheet to draw 对于 current segment.
				// They can also be changed to suit your 精灵.
				if (i == list.Count - 2) {
					// 这是 the head 的 whip. You need to measure the 精灵 to figure out these values.
					frame.Y = 74; // 距离 从 顶部 的 精灵 到 开始 的 帧.
					frame.Height = 18; // 高度 的 帧.

					// 对于 a more impactful look, this scales the 提示 的 whip up when fully extended, and down when curled up.
					Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
					float t = Timer / timeToFlyOut;
					scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
				}
				else if (i > 10) {
					// Third segment
					frame.Y = 58;
					frame.Height = 16;
				}
				else if (i > 5) {
					// 其次 Segment
					frame.Y = 42;
					frame.Height = 16;
				}
				else if (i > 0) {
					// 首先 Segment
					frame.Y = 26;
					frame.Height = 16;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2; // This 弹幕's 精灵 faces down, so PiOver2 is used to correct 旋转.
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

				pos += diff;
			}
			return false;
		}
	}
}
