using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	// This Example show how to implement simple homing 弹幕
	// 可以 tested with ExampleCustomAmmoGun
	public class ExampleHomingProjectile : ModProjectile
	{
		// 存储 the 目标 NPC using 弹幕.ai[0]
		private NPC HomingTarget {
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set {
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public ref float DelayTimer => ref Projectile.ai[1];

		public override void SetStaticDefaults() {
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // 使 the cultist resistant to this 弹幕, as it's resistant to all homing projectiles.
		}

		public override void SetDefaults() {
			Projectile.width = 8; // The 宽度 of 弹幕 hitbox
			Projectile.height = 8; // The 高度 of 弹幕 hitbox

			Projectile.DamageType = DamageClass.Ranged; // What 类型 of 伤害 does this 弹幕 affect?
			Projectile.friendly = true; // Can the 弹幕 deal 伤害 to enemies?
			Projectile.hostile = false; // Can the 弹幕 deal 伤害 到 玩家?
			Projectile.ignoreWater = true; // Does the 弹幕's 速度 be influenced by water?
			Projectile.light = 1f; // How much light emit around the 弹幕
			Projectile.timeLeft = 600; // The live 时间 对于 弹幕 (60 = 1 second, so 600 is 10 seconds)
		}

		// 自定义 AI
		public override void AI() {
			float maxDetectRadius = 400f; // The 最大 radius at which a 弹幕 can detect a 目标

			// 一个 short 延迟 to homing behavior after being fired
			if (DelayTimer < 10) {
				DelayTimer += 1;
				return;
			}

			// 首先, we 查找 a homing 目标 if we don't have one
			if (HomingTarget == null) {
				HomingTarget = FindClosestNPC(maxDetectRadius);
			}

			// 如果 we have a homing 目标, 确保 it is still valid. If the NPC dies or moves away, we'll 想要 查找 a new 目标
			if (HomingTarget != null && !IsValidTarget(HomingTarget)) {
				HomingTarget = null;
			}

			// 如果 we don't have a 目标, don't adjust trajectory
			if (HomingTarget == null)
				return;

			// 如果 found, we 旋转 the 弹幕 速度 在 方向 的 目标.
			// 我们 only 旋转 by 3 degrees an 更新 to give it a smooth trajectory. Increase the 旋转 速度 here to make tighter turns
			float length = Projectile.velocity.Length();
			float targetAngle = Projectile.AngleTo(HomingTarget.Center);
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(3)).ToRotationVector2() * length;
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		// Finding the closest NPC to 攻击 within maxDetectDistance 范围
		// 如果 not found then returns 空
		public NPC FindClosestNPC(float maxDetectDistance) {
			NPC closestNPC = null;

			// 使用 squared values in 距离 checks will let us 跳过 square root calculations, drastically improving this 方法's 速度.
			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			// 循环 through all NPCs
			foreach (var target in Main.ActiveNPCs) {
				// 检查 if NPC 能够 be targeted. 
				if (IsValidTarget(target)) {
					// DistanceSquared 函数 returns a squared 距离 between 2 points, skipping relatively expensive square root calculations
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					// 检查 if it is with在 radius
					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

		public bool IsValidTarget(NPC target) {
			// 此方法 checks th在 NPC is:
			// 1. active (alive)
			// 2. chaseable (e.g. not a cultist archer)
			// 3. max life bigger than 5 (e.g. not a critter)
			// 4. can take 伤害 (e.g. moonlord core after all it's parts are downed)
			// 5. hostile (!friendly)
			// 6. not immortal (e.g. not a 目标 dummy)
			// 7. doesn't have solid tiles blocking a line of sight between the 弹幕 and NPC
			return target.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, target.position, target.width, target.height);
		}
	}
}
