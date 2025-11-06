using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleDrillProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			// 防止s jitter when stepping up and down blocks and half blocks
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.ownerHitCheck = true;
			Projectile.aiStyle = -1; // 替换 with 20 if you do not want custom code
			Projectile.hide = true; // 隐藏s the 弹幕, so it will draw 在 玩家's hand when we set the 玩家's heldProj to this one.
		}

		// This code is adapted and simplified from aiStyle 20 to use a different dust and more noises. If you 想要 use aiStyle 20, you do not 需要 do 任何 of this.
		// It 应该 noted that this 弹幕 has no 效果 on mining and is mostly visual.
		public override void AI() {
			Player player = Main.player[Projectile.owner];

			Projectile.timeLeft = 60;

			// 动画 code could go here if the 弹幕 was animated. 

			// Plays a 声音 每个 20 ticks. In aiStyle 20, soundDelay is set to 30 ticks.
			if (Projectile.soundDelay <= 0) {
				SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
				Projectile.soundDelay = 20;
			}

			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
			if (Main.myPlayer == Projectile.owner) {
				// This code must only be ran 在 客户端 的 弹幕 所有者
				if (player.channel) {
					float holdoutDistance = player.HeldItem.shootSpeed * Projectile.scale;
					// 计算 a normalized vector from 玩家 to 鼠标 and multiply by holdoutDistance to determine resulting holdoutOffset
					Vector2 holdoutOffset = holdoutDistance * Vector2.Normalize(Main.MouseWorld - playerCenter);
					if (holdoutOffset.X != Projectile.velocity.X || holdoutOffset.Y != Projectile.velocity.Y) {
						// 这将 同步 the 弹幕, most importantly, the 速度.
						Projectile.netUpdate = true;
					}

					// 弹幕.速度 acts as a holdoutOffset for held projectiles.
					Projectile.velocity = holdoutOffset;
				}
				else {
					Projectile.Kill();
				}
			}

			if (Projectile.velocity.X > 0f) {
				player.ChangeDir(1);
			}
			else if (Projectile.velocity.X < 0f) {
				player.ChangeDir(-1);
			}

			Projectile.spriteDirection = Projectile.direction;
			player.ChangeDir(Projectile.direction); // 更改 the 玩家's 方向 based 在 弹幕's own
			player.heldProj = Projectile.whoAmI; // We tell the 玩家 th在 drill is the held 弹幕, so it will draw 在ir hand
			player.SetDummyItemTime(2); // 使 sure the 玩家's 项 时间 does not change while the 弹幕 is out
			Projectile.Center = playerCenter; // Centers the 弹幕 在 玩家. 弹幕.速度 将 added to this in later Terraria code causing the 弹幕 to be held away 从 玩家 at a set 距离.
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

			// Gives the drill a slight jiggle
			Projectile.velocity.X *= 1f + Main.rand.Next(-3, 4) * 0.01f;

			// 生成ing dust
			if (Main.rand.NextBool(10)) {
				Dust dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity * Main.rand.Next(6, 10) * 0.15f, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), 0f, 0f, 80, Color.White, 1f);
				dust.position.X -= 4f;
				dust.noGravity = true;
				dust.velocity.X *= 0.5f;
				dust.velocity.Y = -Main.rand.Next(3, 8) * 0.1f;
			}
		}
	}
}
