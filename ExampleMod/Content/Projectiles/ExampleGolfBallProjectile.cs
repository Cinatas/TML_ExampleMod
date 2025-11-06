using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleGolfBallProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			ProjectileID.Sets.IsAGolfBall[Type] = true; // 允许s the 弹幕 to be placed 在 tee.
			ProjectileID.Sets.TrailingMode[Type] = 0; // 创建s a trail behind the golf ball.
			ProjectileID.Sets.TrailCacheLength[Type] = 20; // 设置s the 长度 的 trail.
		}

		public override void SetDefaults() {
			Projectile.netImportant = true; // 指示 that this 弹幕 将 synced to a joining 玩家 (默认情况下, 任何 projectiles active before the 玩家 joins (besides pets) are not synced over).
			Projectile.width = 7; // The 宽度 的 弹幕's hitbox.
			Projectile.height = 7; // The 高度 的 弹幕's hitbox.
			Projectile.friendly = true; // 设置ting this to 任何thing other than 真 causes an 索引 out of bounds 错误.
			Projectile.penetrate = -1; // 数字 of times the 弹幕 can penetrate enemies. -1 sets it to infinite penetration.
			Projectile.aiStyle = 149; // 149 is the golf ball AI.
			Projectile.tileCollide = false; // 图格 Collision is set to 假, as it's handled 在 AI.
		}
	}
}