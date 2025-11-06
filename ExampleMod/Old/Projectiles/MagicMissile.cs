using System;
using ExampleMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.Projectiles
{
	// Code adapted 从 vanilla's magic missile.
	public class MagicMissile : ModProjectile
	{
		public override void SetDefaults() {
			projectile.width = 10;
			projectile.height = 10;
			// 弹幕.aiStyle = 9; // Vanilla magic missile uses this aiStyle, but using it wouldn't let us fine tune the 弹幕 速度 or dust
			projectile.friendly = true;
			projectile.light = 0.8f;
			projectile.magic = true;
			drawOriginOffsetY = -6;
		}

		public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 0);

		public override void AI() {
			// This part makes the 弹幕 do a shime 声音 每个 10 ticks 只要 it is moving.
			if (projectile.soundDelay == 0 && Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) > 2f) {
				projectile.soundDelay = 10;
				SoundEngine.PlaySound(SoundID.Item9, projectile.position);
			}

			Vector2 dustPosition = projectile.Center + new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, 5));
			Dust dust = Dust.NewDustPerfect(dustPosition, DustType<Sparkle>(), null, 100, Color.Lime, 0.8f);
			dust.velocity *= 0.3f;
			dust.noGravity = true;

			// In Multi 玩家 (MP) This code only runs 在 客户端 的 弹幕's 所有者, this is because it relies on 鼠标 位置, 即n't the same across all clients.
			if (Main.myPlayer == projectile.owner && projectile.ai[0] == 0f) {

				Player player = Main.player[projectile.owner];
				// If the 玩家 channels the 武器, do something. This check only works if 项.通道 is 真 对于 武器.
				if (player.channel) {
					float maxDistance = 18f; // This also sets the maximun 速度 the 弹幕 can reach while following the cursor.
					Vector2 vectorToCursor = Main.MouseWorld - projectile.Center;
					float distanceToCursor = vectorToCursor.Length();

					// Here 我们可以 see th在 速度 的 弹幕 depends 在 距离 到 cursor.
					if (distanceToCursor > maxDistance) {
						distanceToCursor = maxDistance / distanceToCursor;
						vectorToCursor *= distanceToCursor;
					}

					int velocityXBy1000 = (int)(vectorToCursor.X * 1000f);
					int oldVelocityXBy1000 = (int)(projectile.velocity.X * 1000f);
					int velocityYBy1000 = (int)(vectorToCursor.Y * 1000f);
					int oldVelocityYBy1000 = (int)(projectile.velocity.Y * 1000f);

					// This code checks if the precious 速度 的 弹幕 is different enough from its new 速度, and if it is, syncs it 与 服务器 and the other clients in MP.
					// We previously multiplied the 速度 by 1000, then casted it to int, this is to reduce its 精度 and 防止 the 速度 from being synced too much.
					if (velocityXBy1000 != oldVelocityXBy1000 || velocityYBy1000 != oldVelocityYBy1000) {
						projectile.netUpdate = true;
					}

					projectile.velocity = vectorToCursor;

				}
				// If the 玩家 stops channeling, do something else.
				else if (projectile.ai[0] == 0f) {

					// This code 方块 is very similar 到 previous one, but only runs once after the 玩家 stops channeling their 武器.
					projectile.netUpdate = true;

					float maxDistance = 14f; // This also sets the maximun 速度 the 弹幕 can reach after it stops following the cursor.
					Vector2 vectorToCursor = Main.MouseWorld - projectile.Center;
					float distanceToCursor = vectorToCursor.Length();

					//If the 弹幕 was 在 cursor's 位置, set it to 移动 在 oposite 方向 从 玩家.
					if (distanceToCursor == 0f) {
						vectorToCursor = projectile.Center - player.Center;
						distanceToCursor = vectorToCursor.Length();
					}

					distanceToCursor = maxDistance / distanceToCursor;
					vectorToCursor *= distanceToCursor;

					projectile.velocity = vectorToCursor;

					if (projectile.velocity == Vector2.Zero) {
						projectile.Kill();
					}

					projectile.ai[0] = 1f;
				}
			}

			// 设置 the 旋转 so the 弹幕 points towards where it's going.
			if (projectile.velocity != Vector2.Zero) {
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
			}
		}

		public override void Kill(int timeLeft) {
			// If the 弹幕 dies without hitting an 敌人, crate a small explosion that hits all enemies 在 区域.
			if (projectile.penetrate == 1) {
				// 使 the 弹幕 hit all enemies as it circunvents the penetrate 限制.
				projectile.maxPenetrate = -1;
				projectile.penetrate = -1;

				int explosionArea = 60;
				Vector2 oldSize = projectile.Size;
				// Resize the 弹幕 hitbox to be bigger.
				projectile.position = projectile.Center;
				projectile.Size += new Vector2(explosionArea);
				projectile.Center = projectile.position;

				projectile.tileCollide = false;
				projectile.velocity *= 0.01f;
				// 伤害 enemies inside the hitbox 区域
				projectile.Damage();
				projectile.scale = 0.01f;

				//Resize the hitbox to its original 大小
				projectile.position = projectile.Center;
				projectile.Size = new Vector2(10);
				projectile.Center = projectile.position;
			}

			SoundEngine.PlaySound(SoundID.Item10, projectile.position);
			for (int i = 0; i < 10; i++) {
				Dust dust = Dust.NewDustDirect(projectile.position - projectile.velocity, projectile.width, projectile.height, DustType<Sparkle>(), 0, 0, 100, Color.Lime, 0.8f);
				dust.noGravity = true;
				dust.velocity *= 2f;
				dust = Dust.NewDustDirect(projectile.position - projectile.velocity, projectile.width, projectile.height, DustType<Sparkle>(), 0f, 0f, 100, Color.Lime, 0.5f);
			}
		}
	}
}