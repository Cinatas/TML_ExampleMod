using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleGolfBallProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			ProjectileID.Sets.IsAGolfBall[Type] = true; // 允许s the projectile to be placed 在 tee.
			ProjectileID.Sets.TrailingMode[Type] = 0; // 创建s a trail behind the golf ball.
			ProjectileID.Sets.TrailCacheLength[Type] = 20; // 设置s the length 的 trail.
		}

		public override void SetDefaults() {
			Projectile.netImportant = true; // 指示 that this projectile 将 synced to a joining player (默认情况下, any projectiles active before the player joins (besides pets) are not synced over).
			Projectile.width = 7; // The width 的 projectile's hitbox.
			Projectile.height = 7; // The height 的 projectile's hitbox.
			Projectile.friendly = true; // 设置ting this to anything other than true causes an index out of bounds error.
			Projectile.penetrate = -1; // Number of times the projectile can penetrate enemies. -1 sets it to infinite penetration.
			Projectile.aiStyle = 149; // 149 is the golf ball AI.
			Projectile.tileCollide = false; // Tile Collision is set to false, as it's handled 在 AI.
		}
	}
}