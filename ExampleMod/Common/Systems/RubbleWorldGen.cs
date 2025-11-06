using ExampleMod.Content.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
namespace ExampleMod.Common.Systems
{
	// 此示例展示在世界生成期间生成碎石图格。
	public class RubbleWorldGen : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
			// 在"Piles"过程之后立即添加 GenPass。ExampleOreSystem 更详细地解释了这种方法。
			int PilesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Piles"));

			if (PilesIndex != -1) {
				tasks.Insert(PilesIndex + 1, new ExamplePilesPass("Example Mod Piles", 100f));
			}
		}
	}

	public class ExamplePilesPass : GenPass
	{
		public ExamplePilesPass(string name, float loadWeight) : base(name, loadWeight) {
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration) {
			progress.Message = "Example Mod Piles";

			int[] tileTypes = new int[] { ModContent.TileType<Example1x1RubbleNatural>(), ModContent.TileType<Example2x1RubbleNatural>(), ModContent.TileType<Example3x2RubbleNatural>() };

			// 为了不让人烦恼，我们只在出生点附近生成 15 个示例碎石。
			// 此示例使用 Try Until Success 方法：https://github.com/tModLoader/tModLoader/wiki/World-Generation#try-until-success
			for (int k = 0; k < 15; k++) {
				bool success = false;
				int attempts = 0;

				while (!success) {
					attempts++;
					if (attempts > 1000) {
						break;
					}
					int x = WorldGen.genRand.Next(Main.maxTilesX / 2 - 40, Main.maxTilesX / 2 + 40);
					int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.worldSurfaceHigh);
					int tileType = WorldGen.genRand.Next(tileTypes);
					int placeStyle = WorldGen.genRand.Next(6); // Each 的se tiles have 6 place styles
					if (Main.tile[x, y].TileType == tileType) {
						continue;
					}

					WorldGen.PlaceTile(x, y, tileType, mute: true, style: placeStyle);
					success = Main.tile[x, y].TileType == tileType;
				}
			}
		}
	}
}
