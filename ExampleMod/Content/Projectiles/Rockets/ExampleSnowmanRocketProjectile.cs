using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles.Rockets
{
	public class ExampleSnowmanRocketProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			ProjectileID.Sets.IsARocketThatDealsDoubleDamageToPrimaryEnemy[Type] = true; // Deals double 伤害 on direct hits.
			ProjectileID.Sets.CultistIsResistantTo[Type] = true; // The Lunatic Cultist is resistant to homing weapons.
			ProjectileID.Sets.RocketsSkipDamageForPlayers[Type] = true; // This set makes it so the 火箭 doesn't deal 伤害 to players.

			// This set handles some things for us already:
			// 设置s the timeLeft to 3 and the 弹幕 方向 when colliding with an NPC or 玩家 in PVP (so the explosive can detonate).
			// Explosives also bounce off the 顶部 of Shimmer, detonate with no blast 伤害 when touching the 底部 or sides of Shimmer, and 伤害 other players in 对于 Worthy worlds.
			ProjectileID.Sets.Explosive[Type] = true;
		}
		public override void SetDefaults() {
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.penetrate = -1; // Infinite penetration so th在 blast can hit all enemies within its radius.
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.scale = 0.9f; // All snowmen rockets are 0.9f 缩放.

			// Rockets use explosive AI, ProjAIStyleID.Explosive (16). You could use that instead here 与 correct AIType.
			// But, using our own AI allows us to customize things like the dusts th在 火箭 creates.
			// 弹幕.aiStyle = ProjAIStyleID.Explosive;
			// AIType = ProjectileID.RocketSnowmanI;
		}
		public override void AI() {
			// 如果 timeLeft is <= 3, then explode the 火箭.
			if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3) {
				PrepareBombToBlow();
			}
			else {
				Projectile.localAI[1]++;
				// 之后 6 ticks, make the 火箭 completely opaque.
				if (Projectile.localAI[1] > 6f) {
					Projectile.alpha = 0; // 0 Alpha is completely opaque.
				}
				else {
					// 之前 then, fade 在 火箭 each tick.
					Projectile.alpha = (int)(255f - 42f * Projectile.localAI[1]) + 100;
					if (Projectile.alpha > 255) {
						Projectile.alpha = 255; // 255 Alpha is completely transparent.
					}
				}

				for (int i = 0; i < 2; i++) {
					// 不要 开始 spawning dusts until after 9 ticks have passed.
					if (!(Projectile.localAI[1] > 9f)) {
						continue;
					}

					// These two variables are used to add some movement 到 dusts.
					float velocityXAdder = 0f;
					float velocityYAdder = 0f;
					if (i == 1) {
						velocityXAdder = Projectile.velocity.X * 0.5f;
						velocityYAdder = Projectile.velocity.Y * 0.5f;
					}

					// 生成 some fire dusts.
					if (Main.rand.NextBool(2)) {
						Dust fireDust = Dust.NewDustDirect(new Vector2(Projectile.position.X + 3f + velocityXAdder, Projectile.position.Y + 3f + velocityYAdder) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Torch, 0f, 0f, 100);
						fireDust.scale *= 1.4f + Main.rand.Next(10) * 0.1f;
						fireDust.velocity *= 0.2f;
						fireDust.noGravity = true;

						// 使用d by the liquid rockets which leave trails 的ir liquid instead of fire.
						// if (fireDust.类型 == Dust.dustWater()) {
						//	fireDust.缩放 *= 0.65f;
						//	fireDust.速度 += 弹幕.速度 * 0.1f;
						// }
					}

					// 生成 some smoke dusts.
					if (Main.rand.NextBool(2)) {
						Dust smokeDust = Dust.NewDustDirect(new Vector2(Projectile.position.X + 3f + velocityXAdder, Projectile.position.Y + 3f + velocityYAdder) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Smoke, 0f, 0f, 100, default, 0.5f);
						smokeDust.fadeIn = 0.5f + Main.rand.Next(5) * 0.1f;
						smokeDust.velocity *= 0.05f;
					}
				}

				// 首先, set the destination 的 火箭 to its current 位置. This 将 updated 在 following section.
				float projDestinationX = Projectile.position.X;
				float projDestinationY = Projectile.position.Y;
				float maxHomingDistance = 600f; // Max homing 距离 in pixels. 16 pixels per 图格, so 600 pixels = 37.5 tiles.

				bool isHoming = false;
				Projectile.ai[0]++; // 计时器 for how long to wait before homing.

				// Wait a short amount of 时间 before homing. 15 ticks in this case.
				if (Projectile.ai[0] > 15f) {
					Projectile.ai[0] = 15f;

					// 搜索 through all 的 NPCs to 查找 a 目标.
					for (int i = 0; i < Main.maxNPCs; i++) {
						NPC searchNPC = Main.npc[i];
						// 如果 the 目标 可以 homed on to.
						if (searchNPC.CanBeChasedBy(this)) {
							// 获取 the 目标's 位置.
							float targetPosX = searchNPC.position.X + (searchNPC.width / 2);
							float targetPosY = searchNPC.position.Y + (searchNPC.height / 2);
							// 查找 the 距离 从 弹幕 到 目标.
							float distanceFromProjToTarget = Math.Abs(Projectile.position.X + (Projectile.width / 2) - targetPosX) + Math.Abs(Projectile.position.Y + (Projectile.height / 2) - targetPosY);
							// 如果 the 距离 is with在 max homing 距离 and the 弹幕 has line of sight.
							if (distanceFromProjToTarget < maxHomingDistance && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, searchNPC.position, searchNPC.width, searchNPC.height)) {
								maxHomingDistance = distanceFromProjToTarget;
								projDestinationX = targetPosX;
								projDestinationY = targetPosY;
								isHoming = true;
							}
						}
					}
				}

				// 如果 the 火箭 is not homing, set its destination to ahead of where it is currently traveling.
				if (!isHoming) {
					projDestinationX = Projectile.position.X + (Projectile.width / 2) + Projectile.velocity.X * 100f;
					projDestinationY = Projectile.position.Y + (Projectile.height / 2) + Projectile.velocity.Y * 100f;
				}

				// Values above 16f could cause the 火箭 to no clip through blocks.
				// 要 increase the 速度 even more, increase extraUpdates in SetDefaults().
				float speed = 16f;

				// Travel 到 位置 set above. Either it 将 到 目标's 位置 or just ahead of itself.
				Vector2 finalVelocity = (new Vector2(projDestinationX, projDestinationY) - Projectile.Center).SafeNormalize(-Vector2.UnitY) * speed;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, finalVelocity, 1f / 12f);

				// 旋转 the 火箭 在 方向 that it is moving and keep the 精灵 facing the correct 方向.
				// This way the face 在 精灵 will always be 右 side up.
				if (Projectile.velocity.X < 0f) {
					Projectile.spriteDirection = -1;
					Projectile.rotation = (float)Math.Atan2(0f - Projectile.velocity.Y, 0f - Projectile.velocity.X) - MathHelper.PiOver2;
				}
				else {
					Projectile.spriteDirection = 1;
					Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity) {
			Projectile.velocity *= 0f; // 停止 moving so the explosion is where the 火箭 was.
			Projectile.timeLeft = 3; // 设置 the timeLeft to 3 so it can get ready to explode.
			return false; // 返回ing 假 is important here. Otherwise the 弹幕 will die without being resized (no blast radius).
		}

		public override void PrepareBombToBlow() {
			Projectile.tileCollide = false; // This is important or the explosion 将 在 wrong place if the 火箭 explodes on slopes.
			Projectile.alpha = 255; // 使 the 火箭 invisible.

			// Resize the hitbox 的 弹幕 对于 blast "radius".
			// 火箭 I: 128, 火箭 III: 200, Mini Nuke 火箭: 250
			// Measurements are in pixels, so 128 / 16 = 8 tiles.
			Projectile.Resize(128, 128);
			// 设置 the knockback 的 blast.
			// 火箭 I: 8f, 火箭 III: 10f, Mini Nuke 火箭: 12f
			Projectile.knockBack = 8f;
		}

		public override void OnKill(int timeLeft) {
			// Play an exploding 声音.
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

			// Resize the 弹幕 again so the explosion dust and gore 生成 从 middle.
			// 火箭 I: 22, 火箭 III: 80, Mini Nuke 火箭: 50
			Projectile.Resize(22, 22);

			// 生成 a bunch of smoke dusts.
			for (int i = 0; i < 30; i++) {
				Dust smoke = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
				smoke.velocity *= 1.4f;
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

			// 要 make the explosion destroy tiles, take a look 在 commented out code in Example 火箭 弹幕.
		}
	}
}