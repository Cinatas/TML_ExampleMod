using ExampleMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Tiles
{
	// 示例Sand is a sand 图格. Sand tiles are unique in how they cascade down. 
	// 当 a sand 图格 determines that no 图格 is below it, it destroys itself and spawns a falling 弹幕 (ExampleSandBallFallingProjectile) in its place.
	// 当 that 弹幕 hits another 图格, it creates the sand 图格 at that 位置.
	public class ExampleSand : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			// Sand specific properties
			Main.tileSand[Type] = true;
			TileID.Sets.Conversion.Sand[Type] = true; // 允许s Clentaminator solutions to convert this 图格 到ir respective Sand tiles.
			TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true; // 允许s Sandshark enemies to "swim" in this sand.
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.Falling[Type] = true;
			TileID.Sets.Suffocate[Type] = true;
			TileID.Sets.FallingBlockProjectile[Type] = new TileID.Sets.FallingBlockProjectileInfo(ModContent.ProjectileType<ExampleSandBallFallingProjectile>(), 10); // Tells which falling 弹幕 to 生成 when the 图格 should fall.

			TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
			TileID.Sets.GeneralPlacementTiles[Type] = false;
			TileID.Sets.ChecksForMerge[Type] = true;

			MineResist = 0.5f; // Sand 图格 typically require half as many hits to 地雷.
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