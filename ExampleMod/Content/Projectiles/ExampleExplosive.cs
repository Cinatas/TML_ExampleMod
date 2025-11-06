using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	// This 弹幕 demonstrates exploding tiles (like a bomb or dynamite), spawning child projectiles, and explosive visual effects.
	public class ExampleExplosive : ModProjectile
	{
		private const int DefaultWidthHeight = 15;
		private const int ExplosionWidthHeight = 250;

		private bool IsChild {
			get => Projectile.localAI[0] == 1;
			set => Projectile.localAI[0] = value.ToInt();
		}

		public override void SetStaticDefaults() {
			ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true; // 伤害 dealt to players does not 缩放 with difficulty in vanilla.

			// This set handles some things for us already:
			// 设置s the timeLeft to 3 and the 弹幕 方向 when colliding with an NPC or 玩家 in PVP (so the explosive can detonate).
			// Explosives also bounce off the 顶部 of Shimmer, detonate with no blast 伤害 when touching the 底部 or sides of Shimmer, and 伤害 other players in 对于 Worthy worlds.
			ProjectileID.Sets.Explosive[Type] = true;
		}

		public override void SetDefaults() {
			// While the 精灵 is actually bigger than 15x15, 我们使用 15x15 since it lets the 弹幕 clip into tiles as it bounces. It looks better.
			Projectile.width = DefaultWidthHeight;
			Projectile.height = DefaultWidthHeight;
			Projectile.friendly = true;
			Projectile.penetrate = -1;

			// 5 second fuse.
			Projectile.timeLeft = 300;

			// These 帮助 the 弹幕 hitbox be centered 在 弹幕 精灵.
			DrawOffsetX = -2;
			DrawOriginOffsetY = -5;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
			// Vanilla explosions do less 伤害 to Eater of Worlds in expert 模式, so we will too.
			if (Main.expertMode) {
				if (target.type >= NPCID.EaterofWorldsHead && target.type <= NPCID.EaterofWorldsTail) {
					modifiers.FinalDamage /= 5;
				}
			}
		}

		// 弹幕 is very bouncy, but the spawned children projectiles shouldn't bounce at all.
		public override bool OnTileCollide(Vector2 oldVelocity) {
			// Die immediately if IsChild is 真 (We set this to 真 对于 5 extra explosives we 生成 in OnKill)
			if (IsChild) {
				// These two are so the bomb will 伤害 the 玩家 correctly.
				Projectile.timeLeft = 0;
				Projectile.PrepareBombToBlow();
				return true;
			}
			// OnTileCollide can 触发器 quite frequently, so using soundDelay helps 防止 the 声音 from overlapping too much.
			if (Projectile.soundDelay == 0) {
				// 我们 adjust 音量 since the 声音 is a bit too loud. PitchVariance gives the 声音 some 随机 音高 variance.
				SoundStyle impactSound = new SoundStyle($"{nameof(ExampleMod)}/Assets/Sounds/Items/BananaImpact") {
					Volume = 0.7f,
					PitchVariance = 0.5f,
				};
				SoundEngine.PlaySound(impactSound);
			}
			Projectile.soundDelay = 10;

			// This code makes the 弹幕 very bouncy.
			if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f) {
				Projectile.velocity.X = oldVelocity.X * -0.9f;
			}
			if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f) {
				Projectile.velocity.Y = oldVelocity.Y * -0.9f;
			}
			return false;
		}

		public override void AI() {
			// 弹幕 is 在 midst of exploding during the last 3 updates.
			if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3) {
				Projectile.PrepareBombToBlow(); // 获取 ready to explode.
			}
			else {
				// Smoke and fuse dust 生成. The 位置 is calculated to 生成 the dust directly 在 fuse.
				if (Main.rand.NextBool()) {
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1f);
					dust.scale = 0.1f + Main.rand.Next(5) * 0.1f;
					dust.fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
					dust.noGravity = true;
					dust.position = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation - 2.1f, default) * 10f;

					dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1f);
					dust.scale = 1f + Main.rand.Next(5) * 0.1f;
					dust.noGravity = true;
					dust.position = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation - 2.1f, default) * 10f;
				}
			}
			Projectile.ai[0] += 1f;
			if (Projectile.ai[0] > 10f) {
				Projectile.ai[0] = 10f;
				// Roll 速度 dampening. 
				if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f) {
					Projectile.velocity.X = Projectile.velocity.X * 0.96f;

					if (Projectile.velocity.X > -0.01 && Projectile.velocity.X < 0.01) {
						Projectile.velocity.X = 0f;
						Projectile.netUpdate = true;
					}
				}
				// Delayed gravity
				Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
			}
			// 旋转 increased by 速度.X 
			Projectile.rotation += Projectile.velocity.X * 0.1f;
		}

		public override void PrepareBombToBlow() {
			Projectile.tileCollide = false; // This is important or the explosion 将 在 wrong place if the bomb explodes on slopes.
			Projectile.alpha = 255; // 设置 to transparent. This 弹幕 technically lives as transparent for about 3 frames

			// 更改 the hitbox 大小, centered about the original 弹幕 中心. This makes the 弹幕 伤害 enemies during the explosion.
			Projectile.Resize(ExplosionWidthHeight, ExplosionWidthHeight);

			Projectile.damage = 250; // Bomb: 100, Dynamite: 250
			Projectile.knockBack = 10f; // Bomb: 8f, Dynamite: 10f
		}

		public override void OnKill(int timeLeft) {
			// 如果 we are the original 弹幕 running 在 所有者, 生成 the 5 child projectiles.
			if (Projectile.owner == Main.myPlayer && !IsChild) {
				for (int i = 0; i < 5; i++) {
					// 随机 upward vector.
					Vector2 launchVelocity = new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-10, -8));
					// 重要ly, IsChild is set to 真 here. This is checked in OnTileCollide to 防止 bouncing and here in OnKill to 防止 an infinite chain of splitting projectiles.
					Projectile child = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, launchVelocity, Projectile.type, Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, 1);
					(child.ModProjectile as ExampleExplosive).IsChild = true;
					// Usually editing a 弹幕 after NewProjectile would require sending MessageID.SyncProjectile, but IsChild only affects logic running 对于 所有者 so it is not necessary here.
				}
			}

			// Play explosion 声音
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
			// Smoke Dust 生成
			for (int i = 0; i < 50; i++) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
				dust.velocity *= 1.4f;
			}

			// Fire Dust 生成
			for (int i = 0; i < 80; i++) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
				dust.noGravity = true;
				dust.velocity *= 5f;
				dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
				dust.velocity *= 3f;
			}

			// Large Smoke Gore 生成
			for (int g = 0; g < 2; g++) {
				var goreSpawnPosition = new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
				Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X += 1.5f;
				gore.velocity.Y += 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X -= 1.5f;
				gore.velocity.Y += 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X += 1.5f;
				gore.velocity.Y -= 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X -= 1.5f;
				gore.velocity.Y -= 1.5f;
			}
			// 重置 大小 to normal 宽度 and 高度.
			Projectile.Resize(DefaultWidthHeight, DefaultWidthHeight);

			// 最后, actually explode the tiles and walls. Run this code only 对于 所有者
			if (Projectile.owner == Main.myPlayer) {
				int explosionRadius = 7; // Bomb: 4, Dynamite: 7, Explosives & TNT Barrel: 10
				int minTileX = (int)(Projectile.Center.X / 16f - explosionRadius);
				int maxTileX = (int)(Projectile.Center.X / 16f + explosionRadius);
				int minTileY = (int)(Projectile.Center.Y / 16f - explosionRadius);
				int maxTileY = (int)(Projectile.Center.Y / 16f + explosionRadius);

				// 确保 that all 图格 coordinates are with在 世界 bounds
				Utils.ClampWithinWorld(ref minTileX, ref minTileY, ref maxTileX, ref maxTileY);

				// These 2 methods 处理 actually mining the tiles and walls while honoring 图格 explosion conditions
				bool explodeWalls = Projectile.ShouldWallExplode(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY);
				Projectile.ExplodeTiles(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY, explodeWalls);
			}
		}
	}
}
