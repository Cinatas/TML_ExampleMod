using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Pets.ExampleLightPet
{
	public class ExampleLightPetProjectile : ModProjectile
	{
		private const int DashCooldown = 1000; // How frequently this 宠物 will dash at enemies.
		private const float DashSpeed = 20f; // The 速度 with which this 宠物 will dash at enemies.
		private const int FadeInTicks = 30;
		private const int FullBrightTicks = 200;
		private const int FadeOutTicks = 30;
		private const float Range = 500f;

		private static readonly float RangeHypotenuse = (float)(Math.Sqrt(2.0) * Range); // This comes 从 公式 for calculating the diagonal of a square (a * √2)
		private static readonly float RangeHypotenuseSquared = RangeHypotenuse * RangeHypotenuse;

		// following 2 lines of code are ref properties (learn about them in google) 到 弹幕.ai 数组 entries, which will 帮助 us make our code way more readable.
		// We're using the ai 数组 because it's automatically synchronized by the base game in multiplayer, which saves us from writing 很多 boilerplate code.
		// 注意 th在 弹幕.ai 数组 is only 3 entries big. If you need more than 3 synchronized variables - you'll 必须 use fields and 同步 them manually.
		public ref float AIFadeProgress => ref Projectile.ai[0];
		public ref float AIDashCharge => ref Projectile.ai[1];

		public override void SetStaticDefaults() {
			Main.projFrames[Projectile.type] = 1;
			Main.projPet[Projectile.type] = true;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			ProjectileID.Sets.LightPet[Projectile.type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.penetrate = -1;
			Projectile.netImportant = true;
			Projectile.timeLeft *= 5;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.scale = 0.8f;
			Projectile.tileCollide = false;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];

			// 如果 the 玩家 is 不再 active (online) - 停用 (删除) the 弹幕.
			if (!player.active) {
				Projectile.active = false;
				return;
			}

			// Keep the 弹幕 disappearing as long as the 玩家 isn't dead and has the 宠物 增益.
			if (!player.dead && player.HasBuff(ModContent.BuffType<ExampleLightPetBuff>())) {
				Projectile.timeLeft = 2;
			}

			UpdateDash(player);
			UpdateFading(player);
			UpdateExtraMovement();

			// Rotates the 宠物 when it moves horizontally.
			Projectile.rotation += Projectile.velocity.X / 20f;

			// Lights up 区域 around it.
			if (!Main.dedServ) {
				Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.9f, Projectile.Opacity * 0.1f, Projectile.Opacity * 0.3f);
			}
		}

		private void UpdateDash(Player player) {
			// following code makes our 宠物 dash at enemies when certain conditions are met

			AIDashCharge++;

			if (AIDashCharge <= DashCooldown || (int)AIFadeProgress % 100 != 0) {
				return;
			}

			// Enumerate
			foreach (var npc in Main.ActiveNPCs) {
				// 忽略 this npc if it's friendly.
				if (npc.friendly) {
					continue;
				}

				// 忽略 this npc if it's too far away. Note that we're using squared values for our checks, to avoid square root calculations as a small, but effective optimization.
				if (player.DistanceSQ(npc.Center) >= RangeHypotenuseSquared) {
					continue;
				}

				Projectile.velocity += Vector2.Normalize(npc.Center - Projectile.Center) * DashSpeed; // Fling the 弹幕 towards the npc.
				AIDashCharge = 0f; // 重置 the charge.

				// Play a 声音.
				if (!Main.dedServ) {
					SoundEngine.PlaySound(SoundID.Item42, Projectile.Center);
				}

				break;
			}
		}

		private void UpdateFading(Player player) {
			//TODO: Comment and clean this up more.

			var playerCenter = player.Center; // 缓存 the 玩家's 中心 vector to avoid recalculations.

			AIFadeProgress++;

			if (AIFadeProgress < FadeInTicks) {
				Projectile.alpha = (int)(255 - 255 * AIFadeProgress / FadeInTicks);
			}
			else if (AIFadeProgress < FadeInTicks + FullBrightTicks) {
				Projectile.alpha = 0;

				if (Main.rand.NextBool(6)) {
					var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PinkFairy, 0f, 0f, 200, default, 0.8f);

					dust.velocity *= 0.3f;
				}
			}
			else if (AIFadeProgress < FadeInTicks + FullBrightTicks + FadeOutTicks) {
				Projectile.alpha = (int)(255 * (AIFadeProgress - FadeInTicks - FullBrightTicks) / FadeOutTicks);
			}
			else {
				Projectile.Center = playerCenter + Main.rand.NextVector2Circular(Range, Range);
				AIFadeProgress = 0f;

				Projectile.velocity = 2f * Vector2.Normalize(playerCenter - Projectile.Center);
			}

			if (Vector2.Distance(playerCenter, Projectile.Center) > RangeHypotenuse) {
				Projectile.Center = playerCenter + Main.rand.NextVector2Circular(Range, Range);
				AIFadeProgress = 0f;

				Projectile.velocity += 2f * Vector2.Normalize(playerCenter - Projectile.Center);
			}

			if ((int)AIFadeProgress % 100 == 0) {
				Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(90));
			}
		}

		private void UpdateExtraMovement() {
			// 添加s some friction 到 宠物's movement as long as its 速度 is above 1
			if (Projectile.velocity.Length() > 1f) {
				Projectile.velocity *= 0.98f;
			}

			// 如果 the 宠物 stops - launch it into a 随机 方向 at a low 速度.
			if (Projectile.velocity == Vector2.Zero) {
				Projectile.velocity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat() * MathHelper.TwoPi) * 2f;
			}
		}
	}
}