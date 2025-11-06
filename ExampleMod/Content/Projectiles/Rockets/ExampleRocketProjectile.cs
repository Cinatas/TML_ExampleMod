using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles.Rockets
{
	public class ExampleRocketProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			ProjectileID.Sets.IsARocketThatDealsDoubleDamageToPrimaryEnemy[Type] = true; // Deals double damage on direct hits.
			ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true; // Damage dealt to players does not scale with difficulty in vanilla.

			// This set handles some things for us already:
			// 设置s the timeLeft to 3 and the projectile direction when colliding with an NPC or player in PVP (so the explosive can detonate).
			// Explosives also bounce off the top of Shimmer, detonate with no blast damage when touching the bottom or sides of Shimmer, and damage other players in 对于 Worthy worlds.
			ProjectileID.Sets.Explosive[Type] = true;

			// This set makes it so the rocket doesn't deal damage to players. Only used for vanilla rockets.
			// Simply remove the Projectile.HurtPlayer() part to stop the projectile from damaging its user.
			// ProjectileID.Sets.RocketsSkipDamageForPlayers[Type] = true;
		}
		public override void SetDefaults() {
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.penetrate = -1; // Infinite penetration so th在 blast can hit all enemies within its radius.
			Projectile.DamageType = DamageClass.Ranged;

			// Rockets use explosive AI, ProjAIStyleID.Explosive (16). You could use that instead here 与 correct AIType.
			// But, using our own AI allows us to customize things like the dusts th在 rocket creates.
			// Projectile.aiStyle = ProjAIStyleID.Explosive;
			// AIType = ProjectileID.RocketI;
		}
		public override void AI() {
			// 如果 timeLeft is <= 3, then explode the rocket.
			if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3) {
				Projectile.PrepareBombToBlow();
			}
			else {
				// 生成 dusts if the rocket is moving at or greater than half of its max speed.
				if (Math.Abs(Projectile.velocity.X) >= 8f || Math.Abs(Projectile.velocity.Y) >= 8f) {
					for (int i = 0; i < 2; i++) {
						float posOffsetX = 0f;
						float posOffsetY = 0f;
						if (i == 1) {
							posOffsetX = Projectile.velocity.X * 0.5f;
							posOffsetY = Projectile.velocity.Y * 0.5f;
						}

						// 生成 fire dusts 在 back 的 rocket.
						Dust fireDust = Dust.NewDustDirect(new Vector2(Projectile.position.X + 3f + posOffsetX, Projectile.position.Y + 3f + posOffsetY) - Projectile.velocity * 0.5f,
							Projectile.width - 8, Projectile.height - 8, DustID.Torch, 0f, 0f, 100);
						fireDust.scale *= 2f + Main.rand.Next(10) * 0.1f;
						fireDust.velocity *= 0.2f;
						fireDust.noGravity = true;

						// 使用d by the liquid rockets which leave trails 的ir liquid instead of fire.
						// if (fireDust.type == Dust.dustWater()) {
						//	fireDust.scale *= 0.65f;
						//	fireDust.velocity += Projectile.velocity * 0.1f;
						// }

						// 生成 smoke dusts 在 back 的 rocket.
						Dust smokeDust = Dust.NewDustDirect(new Vector2(Projectile.position.X + 3f + posOffsetX, Projectile.position.Y + 3f + posOffsetY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Smoke, 0f, 0f, 100, default, 0.5f);
						smokeDust.fadeIn = 1f + Main.rand.Next(5) * 0.1f;
						smokeDust.velocity *= 0.05f;
					}
				}

				// Increase the speed 的 rocket if it is moving less than 1 block per second.
				// It is not recommended to increase the number past 16f to increase the speed 的 rocket. It could start no clipping through blocks.
				// 代替, increase extraUpdates in SetDefaults() to make the rocket move faster.
				if (Math.Abs(Projectile.velocity.X) <= 15f && Math.Abs(Projectile.velocity.Y) <= 15f) {
					Projectile.velocity *= 1.1f;
				}
			}

			// Rotate the rocket 在 direction that it is moving.
			if (Projectile.velocity != Vector2.Zero) {
				Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
			}
		}

		// 当 the rocket hits a tile, NPC, or player, get ready to explode.
		public override bool OnTileCollide(Vector2 oldVelocity) {
			Projectile.velocity *= 0f; // Stop moving so the explosion is where the rocket was.
			Projectile.timeLeft = 3; // 设置 the timeLeft to 3 so it can get ready to explode.
			return false; // 返回ing false is important here. Otherwise the projectile will die without being resized (no blast radius).
		}

		public override void PrepareBombToBlow() {
			Projectile.tileCollide = false; // This is important or the explosion 将 在 wrong place if the rocket explodes on slopes.
			Projectile.alpha = 255; // 使 the rocket invisible.

			// Resize the hitbox 的 projectile 对于 blast "radius".
			// Rocket I: 128, Rocket III: 200, Mini Nuke Rocket: 250
			// Measurements are in pixels, so 128 / 16 = 8 tiles.
			Projectile.Resize(128, 128);
			// 设置 the knockback 的 blast.
			// Rocket I: 8f, Rocket III: 10f, Mini Nuke Rocket: 12f
			Projectile.knockBack = 8f;
		}

		public override void OnKill(int timeLeft) {
			// Vanilla code takes care ensuring that in 对于 Worthy or Get Fixed Boi worlds the blast can damage other players because
			// this projectile is ProjectileID.Sets.Explosive[Type] = true;. It also takes care of hurting the owner. The Projectile.PrepareBombToBlow
			// and Projectile.HurtPlayer methods 可以 used directly 如果需要 for a projectile not using ProjectileID.Sets.Explosive

			// Play an exploding sound.
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

			// Resize the projectile again so the explosion dust and gore spawn 从 middle.
			// Rocket I: 22, Rocket III: 80, Mini Nuke Rocket: 50
			Projectile.Resize(22, 22);

			// 生成 a bunch of smoke dusts.
			for (int i = 0; i < 30; i++) {
				Dust smokeDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
				smokeDust.velocity *= 1.4f;
			}

			// 生成 a bunch of fire dusts.
			for (int j = 0; j < 20; j++) {
				Dust fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
				fireDust.noGravity = true;
				fireDust.velocity *= 7f;
				fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
				fireDust.velocity *= 3f;
			}

			// 生成 a bunch of smoke gores.
			for (int k = 0; k < 2; k++) {
				float speedMulti = 0.4f;
				if (k == 1) {
					speedMulti = 0.8f;
				}

				Gore smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
				smokeGore.velocity *= speedMulti;
				smokeGore.velocity += Vector2.One;
				smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
				smokeGore.velocity *= speedMulti;
				smokeGore.velocity.X -= 1f;
				smokeGore.velocity.Y += 1f;
				smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
				smokeGore.velocity *= speedMulti;
				smokeGore.velocity.X += 1f;
				smokeGore.velocity.Y -= 1f;
				smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
				smokeGore.velocity *= speedMulti;
				smokeGore.velocity -= Vector2.One;
			}

			// Rocket II explosion that damages tiles.
			//if (Projectile.owner == Main.myPlayer) {
			//	int blastRadius = 3; // Rocket IV: 5, Mini Nuke Rocket II: 7

			//	int minTileX = (int)(Projectile.Center.X / 16f - blastRadius);
			//	int maxTileX = (int)(Projectile.Center.X / 16f + blastRadius);
			//	int minTileY = (int)(Projectile.Center.Y / 16f - blastRadius);
			//	int maxTileY = (int)(Projectile.Center.Y / 16f + blastRadius);

				// 确保 the tiles are inside the world.
			// Utils.ClampWithinWorld(ref minTileX, ref maxTileX, ref minTileY, ref maxTileY);

			// 检查 to see if the walls 应该 destroyed, too.
			//	bool wallSplode = Projectile.ShouldWallExplode(Projectile.position, blastRadius, minTileX, maxTileX, minTileY, maxTileY);
			// Do the damage.
			//	Projectile.ExplodeTiles(Projectile.position, blastRadius, minTileX, maxTileX, minTileY, maxTileY, wallSplode);
			//}
		}
	}
}