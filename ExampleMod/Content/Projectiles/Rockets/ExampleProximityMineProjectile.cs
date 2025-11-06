using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles.Rockets
{
	public class ExampleProximityMineProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			ProjectileID.Sets.IsAMineThatDealsTripleDamageWhenStationary[Type] = true; // Deal triple 伤害 when not moving and "armed".
			ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true; // 伤害 dealt to players does not 缩放 with difficulty in vanilla.

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

			// Proximity Mines use explosive AI, ProjAIStyleID.Explosive (16). You could use that instead here 与 correct AIType.
			// But, using our own AI allows us to customize things like the dusts th在 地雷 creates.
			// 弹幕.aiStyle = ProjAIStyleID.Explosive;
			// AIType = ProjectileID.ProximityMineI;
		}
		public override void AI() {
			// 如果 timeLeft is <= 3, then explode the 地雷.
			if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3) {
				Projectile.PrepareBombToBlow();
			}
			else {
				// 如果 the 地雷 is not moving or barely moving, make it turn almost invisible.
				if (Projectile.velocity.X > -0.2f && Projectile.velocity.X < 0.2f && Projectile.velocity.Y > -0.2f && Projectile.velocity.Y < 0.2f) {
					Projectile.alpha += 2;
					if (Projectile.alpha > 200) {
						Projectile.alpha = 200; // 255 Alpha is completely transparent. So, 200 is almost completely invisible.
					}
				}
				// 否则 make it opaque and 生成 a bunch of smoke dusts.
				else {
					Projectile.alpha = 0; // 0 Alpha is completely opaque.
					var smokeDust = Dust.NewDustDirect(new Vector2(Projectile.position.X + 3f, Projectile.position.Y + 3f) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Smoke, 0f, 0f, 100);
					smokeDust.scale *= 1.6f + Main.rand.Next(5) * 0.1f;
					smokeDust.velocity *= 0.05f;
					smokeDust.noGravity = true;
				}
			}

			Projectile.velocity.Y += 0.2f; // 使 it fall down. Remember, positive Y is down.
			Projectile.velocity *= 0.97f; // 使 it slow down.

			// 如果 the 地雷 is moving very slowly, just make it 停止 entirely.
			if (Projectile.velocity.X > -0.1f && Projectile.velocity.X < 0.1f) {
				Projectile.velocity.X = 0f;
			}

			if (Projectile.velocity.Y > -0.1f && Projectile.velocity.Y < 0.1f) {
				Projectile.velocity.Y = 0f;
			}

			Projectile.rotation += Projectile.velocity.X * 0.1f; // 旋转 the 地雷 based 在 方向 it is moving.
		}

		public override bool OnTileCollide(Vector2 oldVelocity) {
			// Bounce off of tiles.
			if (Projectile.velocity.X != oldVelocity.X) {
				Projectile.velocity.X = oldVelocity.X * -0.4f;
			}

			if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f) {
				Projectile.velocity.Y = oldVelocity.Y * -0.4f;
			}
			// 返回 假 so the 弹幕 doesn't get killed. If you do want your 弹幕 to explode on contact with tiles, do not 返回 真 here.
			// 如果 you 返回 真, the 弹幕 will die without being resized (no blast radius).
			// 代替, set `弹幕.timeLeft = 3;` like the Example 火箭 弹幕.
			return false;
		}

		public override void PrepareBombToBlow() {
			Projectile.tileCollide = false; // This is important or the explosion 将 在 wrong place if the 地雷 explodes on slopes.
			Projectile.alpha = 255; // 使 the 地雷 invisible.

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
				var smokeDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
				smokeDust.velocity *= 1.4f;
			}

			// 生成 a bunch of fire dusts.
			for (int j = 0; j < 20; j++) {
				var fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
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

				var smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
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