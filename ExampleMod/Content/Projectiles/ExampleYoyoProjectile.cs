using ExampleMod.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleYoyoProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			// following sets are only applicable to yoyo that use aiStyle 99.

			// YoyosLifeTimeMultiplier is how long in seconds the yoyo will stay out before automatically returning 到 玩家. 
			// Vanilla values 范围 from 3f (Wood) to 16f (Chik), and defaults to -1f. Leaving as -1 will make the 时间 infinite.
			ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 3.5f;

			// YoyosMaximumRange is the 最大 距离 the yoyo sleep away 从 玩家. 
			// Vanilla values 范围 from 130f (Wood) to 400f (Terrarian), and defaults to 200f.
			ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 300f;

			// YoyosTopSpeed is 顶部 速度 的 yoyo 弹幕.
			// Vanilla values 范围 from 9f (Wood) to 17.5f (Terrarian), and defaults to 10f.
			ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 13f;
		}

		public override void SetDefaults() {
			Projectile.width = 16; // The 宽度 的 弹幕's hitbox.
			Projectile.height = 16; // The 高度 的 弹幕's hitbox.

			Projectile.aiStyle = ProjAIStyleID.Yoyo; // The 弹幕's ai style. Yoyos use aiStyle 99 (ProjAIStyleID.Yoyo). A lot of yoyo code checks for this aiStyle to work properly.

			Projectile.friendly = true; // 玩家 shot 弹幕. Does 伤害 to enemies but not to friendly Town NPCs.
			Projectile.DamageType = DamageClass.MeleeNoSpeed; // Benefits from melee bonuses. MeleeNoSpeed means the 项 will not 缩放 with 攻击 速度.
			Projectile.penetrate = -1; // All vanilla yoyos have infinite penetration. The 数字 of enemies the yoyo can hit before being pulled back in is based on YoyosLifeTimeMultiplier.
			// 弹幕.缩放 = 1f; // The 缩放 的 弹幕. Most yoyos are 1f, but a few are larger. The Kraken is the largest at 1.2f
		}

		// notes for aiStyle 99: 
		// localAI[0] is used for timing up to YoyosLifeTimeMultiplier
		// localAI[1] 可以 used freely by specific types
		// ai[0] and ai[1] usually 点 towards the x and y 世界 坐标 悬停 点
		// ai[0] is -1f once YoyosLifeTimeMultiplier is reached, when the 玩家 is stoned/frozen, when the yoyo is too far away, or the 玩家 is 不再 clicking the shoot 按钮.
		// ai[0] being negative makes the yoyo 移动 back towards the 玩家
		// Any AI 方法 可以 used for dust, spawning projectiles, etc specific to your yoyo.

		public override void PostAI() {
			if (Main.rand.NextBool(5)) {
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>()); // 使 the 弹幕 emit dust.
			}
		}
	}
}
