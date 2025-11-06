using ExampleMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Tiles
{
	// 示例Sand is a sand tile. Sand tiles are unique in how they cascade down. 
	// 当 a sand tile determines that no tile is below it, it destroys itself and spawns a falling projectile (ExampleSandBallFallingProjectile) in its place.
	// 当 that projectile hits another tile, it creates the sand tile at that location.
	public class ExampleSand : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			// Sand specific properties
			Main.tileSand[Type] = true;
			TileID.Sets.Conversion.Sand[Type] = true; // 允许s Clentaminator solutions to convert this tile 到ir respective Sand tiles.
			TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true; // 允许s Sandshark enemies to "swim" in this sand.
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.Falling[Type] = true;
			TileID.Sets.Suffocate[Type] = true;
			TileID.Sets.FallingBlockProjectile[Type] = new TileID.Sets.FallingBlockProjectileInfo(ModContent.ProjectileType<ExampleSandBallFallingProjectile>(), 10); // Tells which falling projectile to spawn when the tile should fall.

			TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
			TileID.Sets.GeneralPlacementTiles[Type] = false;
			TileID.Sets.ChecksForMerge[Type] = true;

			MineResist = 0.5f; // Sand tile typically require half as many hits to mine.
			DustType = DustID.Stone;
			AddMapEntry(new Color(150, 150, 150));
		}

		public override bool HasWalkDust() {
			return Main.rand.NextBool(3);
		}

		public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color) {
			dustType = DustID.Sand;
		}
	}
}