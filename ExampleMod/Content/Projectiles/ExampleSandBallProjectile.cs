using ExampleMod.Content.Items.Placeable;
using ExampleMod.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

// 此文件 contains ExampleSandBallProjectile, ExampleSandBallFallingProjectile, and ExampleSandBallGunProjectile.
// 示例SandBallFallingProjectile and ExampleSandBallGunProjectile inherit from ExampleSandBallProjectile, allowing cleaner code and shared logic.
// 示例SandBallFallingProjectile is the projectile that spawns when the ExampleSand tile falls.
// 示例SandBallGunProjectile is the projectile 即 shot by the Sandgun weapon.
// Both projectiles share the same aiStyle, ProjAIStyleID.FallingTile, but the AIType line in ExampleSandBallGunProjectile ensures that specific logic 的 aiStyle is used 对于 sandgun projectile.
// It is possible to make a falling projectile not using ProjAIStyleID.FallingTile, but it is a lot of code.
namespace ExampleMod.Content.Projectiles
{
	public abstract class ExampleSandBallProjectile : ModProjectile
	{
		public override string Texture => "ExampleMod/Content/Projectiles/ExampleSandBallProjectile";

		public override void SetStaticDefaults() {
			ProjectileID.Sets.FallingBlockDoesNotFallThroughPlatforms[Type] = true;
			ProjectileID.Sets.ForcePlateDetection[Type] = true;
		}
	}

	public class ExampleSandBallFallingProjectile : ExampleSandBallProjectile
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			ProjectileID.Sets.FallingBlockTileItem[Type] = new(ModContent.TileType<ExampleSand>(), ModContent.ItemType<ExampleSandBlock>());
		}

		public override void SetDefaults() {
			// falling projectile when compared 到 sandgun projectile is hostile.
			Projectile.CloneDefaults(ProjectileID.EbonsandBallFalling);
		}
	}

	public class ExampleSandBallGunProjectile : ExampleSandBallProjectile
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			ProjectileID.Sets.FallingBlockTileItem[Type] = new(ModContent.TileType<ExampleSand>());
		}

		public override void SetDefaults() {
			// sandgun projectile when compared 到 falling projectile has a ranged damage type, isn't hostile, and has extraupdates = 1.
			// 注意 that EbonsandBallGun has infinite penetration, unlike SandBallGun
			Projectile.CloneDefaults(ProjectileID.EbonsandBallGun);
			AIType = ProjectileID.EbonsandBallGun; // This is needed for some logic 在 ProjAIStyleID.FallingTile code.
		}
	}
}