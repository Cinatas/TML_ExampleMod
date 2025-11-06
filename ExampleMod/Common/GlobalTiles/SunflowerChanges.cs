using ExampleMod.Content.Tiles;
using System.Linq;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Common.GlobalTiles
{
	// 此示例使用 GlobalTile 来影响现有图格的属性。
	// 在某种程度上可以调整现有图格。
	public class SunflowerChanges : GlobalTile
	{
		public override void SetStaticDefaults() {
			// 这允许向日葵图格放置在 ExampleBlock 上
			TileObjectData tileObjectData = TileObjectData.GetTileData(TileID.Sunflower, 0);
			tileObjectData.AnchorValidTiles = tileObjectData.AnchorValidTiles.Append(ModContent.TileType<ExampleBlock>()).ToArray();
		}

		public override void Unload() {
			// 当模组卸载时，现有图格的 TileObjectData 不会自动重置。模组作者需要正确撤消这些更改。
			TileObjectData tileObjectData = TileObjectData.GetTileData(TileID.Sunflower, 0);
			tileObjectData.AnchorValidTiles = tileObjectData.AnchorValidTiles.Except(new int[] { ModContent.TileType<ExampleBlock>() }).ToArray();
		}
	}
}