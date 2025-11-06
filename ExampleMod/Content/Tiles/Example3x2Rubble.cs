using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Content.Tiles
{
	// 此文件 contains 3 classes and shows off using inheritance to share code between classes.
	// Terraria has many tiles that are purely decorative and do not drop items when broken.
	// These tiles go by many names, 例如 ambient tiles, background tiles, piles, detritus, and rubble. We will use the term rubble because 的 recently added Rubblemaker item. 
	// Rubblemaker (https://terraria.wiki.gg/wiki/Rubblemaker) is a special item that can place these decorative tiles. The tile placed by the Rubblemaker looks the same as the original rubble tile but behaves slightly differently.

	// 示例3x2RubbleBase is an abstract class, it is not an actual tile, but the other 2 classes in this file will reuse the Texture and SetStaticDefaults code shown here because they inherit from it. 
	public abstract class Example3x2RubbleBase : ModTile
	{
		// 我们 want both tiles to use the same texture
		public override string Texture => "ExampleMod/Content/Tiles/Example3x2Rubble";

		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileObsidianKill[Type] = true;

			DustType = DustID.Stone;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(152, 171, 198));
		}
	}

	// 这是 the fake tile that 将 placed by the Rubblemaker.
	public class Example3x2RubbleFake : Example3x2RubbleBase
	{
		public override void SetStaticDefaults() {
			// 调用 to base SetStaticDefaults. Must inherit static defaults from base type 
			base.SetStaticDefaults();

			// 添加 rubble variant, all existing styles, to Rubblemaker, allowing to place this tile by consuming ExampleBlock
			FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<Items.Placeable.ExampleBlock>(), Type, 0, 1, 2, 3, 4, 5);

			// Tiles placed by Rubblemaker drop the item used to place them.
			RegisterItemDrop(ModContent.ItemType<Items.Placeable.ExampleBlock>());
		}
	}

	// 这是 the natural tile, this version is placed during world generation 在 RubbleWorldGen class.
	public class Example3x2RubbleNatural : Example3x2RubbleBase
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			// 默认情况下, the TileObjectData.Style3x2 tile we copied in Example3x2RubbleBase has LavaDeath = true. Natural rubble tiles don't have this behavior, so we want to be immune to lava.
			TileObjectData.GetTileData(Type, 0).LavaDeath = false;
		}

		// Natural rubble tiles typically drop critters, but no item.
		public override void DropCritterChance(int i, int j, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance) {
			wormChance = 6;
		}
	}
}