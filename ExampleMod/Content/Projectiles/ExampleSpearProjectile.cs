using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleSpearProjectile : ModProjectile
	{
		// 定义 the 范围 的 Spear 弹幕. These are overridable properties, in case you'll 想要 make a 类 inheriting from this one.
		protected virtual float HoldoutRangeMin => 24f;
		protected virtual float HoldoutRangeMax => 96f;

		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for 宽度, 高度, aiStyle, friendly, penetrate, tileCollide, 缩放, hide, ownerHitCheck, and melee.
		}

		public override bool PreAI() {
			Player player = Main.player[Projectile.owner]; // Since we access the 所有者 玩家 实例 so much, it's useful to create a helper local 变量 for this
			int duration = player.itemAnimationMax; // 定义 the durati在 弹幕 will exist in frames

			player.heldProj = Projectile.whoAmI; // 更新 the 玩家's held 弹幕 ID

			// 重置 弹幕 时间 左 如有必要
			if (Projectile.timeLeft > duration) {
				Projectile.timeLeft = duration;
			}

			Projectile.velocity = Vector2.Normalize(Projectile.velocity); // 速度 isn't used in this spear implementation, but 我们使用 the 字段 to store the spear's 攻击 方向.

			float halfDuration = duration * 0.5f;
			float progress;

			// 在这里 'progress' is set to a 值 that goes from 0.0 to 1.0 and back during the 项 use 动画.
			if (Projectile.timeLeft < halfDuration) {
				progress = Projectile.timeLeft / halfDuration;
			}
			else {
				progress = (duration - Projectile.timeLeft) / halfDuration;
			}

			// 移动 the 弹幕 从 HoldoutRangeMin 到 HoldoutRangeMax and back, using SmoothStep for easing the movement
			Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

			// 应用 proper 旋转 到 精灵.
			if (Projectile.spriteDirection == -1) {
				// 如果 精灵 is facing 左, 旋转 45 degrees
				Projectile.rotation += MathHelper.ToRadians(45f);
			}
			else {
				// 如果 精灵 is facing 右, 旋转 135 degrees
				Projectile.rotation += MathHelper.ToRadians(135f);
			}

			// 避免 spawning dusts on dedicated servers
			if (!Main.dedServ) {
				// These dusts are added later, 对于 'ExampleMod' 效果
				if (Main.rand.NextBool(3)) {
					Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, Alpha: 128, Scale: 1.2f);
				}

				if (Main.rand.NextBool(4)) {
					Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), Alpha: 128, Scale: 0.3f);
				}
			}

			return false; // 不要 execute vanilla AI.
		}
	}
}
