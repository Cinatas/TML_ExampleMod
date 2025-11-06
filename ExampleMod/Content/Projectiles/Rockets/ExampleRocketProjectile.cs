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
			ProjectileID.Sets.IsARocketThatDealsDoubleDamageToPrimaryEnemy[Type] = true; // Deals double 伤害 on direct hits.
			ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true; // 伤害 dealt to players does not 缩放 with difficulty in vanilla.

			// This set handles some things for us already:
			// 设置s the timeLeft to 3 and the 弹幕 方向 when colliding with an NPC or 玩家 in PVP (so the explosive can detonate).
			// Explosives also bounce off the 顶部 of Shimmer, detonate with no blast 伤害 when touching the 底部 or sides of Shimmer, and 伤害 other players in 对于 Worthy worlds.
			ProjectileID.Sets.Explosive[Type] = true;

			// This set makes it so the 火箭 doesn't deal 伤害 to players. Only used for vanilla rockets.
			// Simply 删除 the 弹幕.HurtPlayer() part to 停止 the 弹幕 from damaging its 用户.
			// ProjectileID.Sets.RocketsSkipDamageForPlayers[类型] = 真;
		}
		public override void SetDefaults() {
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.penetrate = -1; // Infinite penetration so th在 blast can hit all enemies within its radius.
			Projectile.DamageType = DamageClass.Ranged;

			// Rockets use explosive AI, ProjAIStyleID.Explosive (16). You could use that instead here 与 correct AIType.
			// But, using our own AI allows us to customize things like the dusts th在 火箭 creates.
			// 弹幕.aiStyle = ProjAIStyleID.Explosive;
			// AIType = ProjectileID.RocketI;
		}
		public override void AI() {
			// 如果 timeLeft is <= 3, then explode the 火箭.
			if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3) {
				Projectile.PrepareBombToBlow();
			}
			else {
				// 生成 dusts if the 火箭 is moving at or greater than half of its max 速度.
				if (Math.Abs(Projectile.velocity.X) >= 8f || Math.Abs(Projectile.velocity.Y) >= 8f) {
					for (int i = 0; i < 2; i++) {
						float posOffsetX = 0f;
						float posOffsetY = 0f;
						if (i == 1) {
							posOffsetX = Projectile.velocity.X * 0.5f;
							posOffsetY = Projectile.velocity.Y * 0.5f;
						}

						// 生成 fire dusts 在 back 的 火箭.
						Dust fireDust = Dust.NewDustDirect(new Vector2(Projectile.position.X + 3f + posOffsetX, Projectile.position.Y + 3f + posOffsetY) - Projectile.velocity * 0.5f,
							Projectile.width - 8, Projectile.height - 8, DustID.Torch, 0f, 0f, 100);
						fireDust.scale *= 2f + Main.rand.Next(10) * 0.1f;
						fireDust.velocity *= 0.2f;
						fireDust.noGravity = true;

						// 使用d by the liquid rockets which leave trails 的ir liquid 代替 fire.
						// if (fireDust.类型 == Dust.dustWater()) {
						//	fireDust.缩放 *= 0.65f;
						//	fireDust.速度 += 弹幕.速度 * 0.1f;
						// }

						// 生成 smoke dusts 在 back 的 火箭.
						Dust smokeDust = Dust.NewDustDirect(new Vector2(Projectile.position.X + 3f + posOffsetX, Projectile.position.Y + 3f + posOffsetY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Smoke, 0f, 0f, 100, default, 0.5f);
						smokeDust.fadeIn = 1f + Main.rand.Next(5) * 0.1f;
						smokeDust.velocity *= 0.05f;
					}
				}

				// Increase the 速度 的 火箭 if it is moving 少于 1 方块 per second.
				// 它是 not recommended to increase the 数字 past 16f to increase the 速度 的 火箭. It could 开始 no clipping through blocks.
				// 代替, increase extraUpdates in SetDefaults() to make the 火箭 移动 faster.
				if (Math.Abs(Projectile.velocity.X) <= 15f && Math.Abs(Projectile.velocity.Y) <= 15f) {
					Projectile.velocity *= 1.1f;
				}
			}

			// 旋转 the 火箭 在 方向 that it is moving.
			if (Projectile.velocity != Vector2.Zero) {
				Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
			}
		}

		// 当 the 火箭 hits a 图格, NPC, or 玩家, get ready to explode.
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
			// Vanilla code takes care ensuring that in 对于 Worthy or Get Fixed Boi worlds the blast can 伤害 other players because
			// this 弹幕 is ProjectileID.Sets.Explosive[类型] = 真;. It also takes care of hurting the 所有者. The 弹幕.PrepareBombToBlow
			// and 弹幕.HurtPlayer methods 可以 used directly 如果需要 for a 弹幕 not using ProjectileID.Sets.Explosive

			// Play an exploding 声音.
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

			// Resize the 弹幕 again so the explosion dust and gore 生成 从 middle.
			// 火箭 I: 22, 火箭 III: 80, Mini Nuke 火箭: 50
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

			// 火箭 II explosion that damages tiles.
			//if (弹幕.所有者 == Main.myPlayer) {
			//	int blastRadius = 3; // 火箭 IV: 5, Mini Nuke 火箭 II: 7

			//	int minTileX = (int)(弹幕.中心.X / 16f - blastRadius);
			//	int maxTileX = (int)(弹幕.中心.X / 16f + blastRadius);
			//	int minTileY = (int)(弹幕.中心.Y / 16f - blastRadius);
			//	int maxTileY = (int)(弹幕.中心.Y / 16f + blastRadius);

				// 确保 the tiles are inside the 世界.
			// Utils.ClampWithinWorld(ref minTileX, ref maxTileX, ref minTileY, ref maxTileY);

			// 检查 to see if the walls 应该 destroyed, too.
			//	bool wallSplode = 弹幕.ShouldWallExplode(弹幕.位置, blastRadius, minTileX, maxTileX, minTileY, maxTileY);
			// Do the 伤害.
			//	弹幕.ExplodeTiles(弹幕.位置, blastRadius, minTileX, maxTileX, minTileY, maxTileY, wallSplode);
			//}
		}
	}
}