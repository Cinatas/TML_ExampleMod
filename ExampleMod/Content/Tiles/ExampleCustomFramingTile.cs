using ExampleMod.Content.Biomes;
using ExampleMod.Content.Dusts;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ExampleMod.Content.Items;

namespace ExampleMod.Content.Tiles
{
	public class ExampleCustomFramingTile : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			//This indicates the 图格's spritesheet has extra frames for merging with adjacent tiles
			//Main.tileMergeDirt[类型] = 真 可以 used for merging with dirt automatically, but here we 将 looking at making the 图格 合并 with snow instead
			TileID.Sets.ChecksForMerge[Type] = true;
			//This is 以便 snow blocks 尝试 connect to this 图格
			//Our custom framing code will make our 图格 have custom merging with snow, but we also 需要 specify this so the merging happens 在 snow 方块 side 以及
			Main.tileMerge[TileID.SnowBlock][Type] = true;
			AddMapEntry(new Color(200, 200, 200));
		}


		public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight) {
			//We use this 方法 to set the 合并 values 的 adjacent tiles to -2 if the 图格 nearby is a snow 方块
			//-2 is what terraria uses to designate the tiles that will 合并 with ours using the custom frames
			WorldGen.TileMergeAttempt(-2, TileID.SnowBlock, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
		}

		public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight) {
			//For 每个 even Y 坐标, we will 偏移 the 图格's vertical 帧 by the 大小 的 sheet so the 图格's 帧 ends up using the alternate 版本 在 duplicated sheet below
			if (j % 2 == 0) {
				Tile t = Main.tile[i, j];
				t.TileFrameY += 270;
			}
				
		}
	}

	internal class ExampleCustomFramingTileItem : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<ExampleCustomFramingTile>());
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}