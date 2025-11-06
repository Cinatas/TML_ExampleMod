using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Content.Tiles
{
	// 此文件 contains 3 classes and shows off using inheritance to share code between classes.
	// Terraria has m任何 tiles that are purely decorative and do not 放下 items when broken.
	// These tiles go by m任何 names, 例如 ambient tiles, 背景 tiles, piles, detritus, and rubble. We will use the term rubble because 的 recently added Rubblemaker 项. 
	// Rubblemaker (https://terraria.wiki.gg/wiki/Rubblemaker) is a special 项 that can place these decorative tiles. The 图格 placed by the Rubblemaker looks the same as the original rubble 图格 but behaves slightly differently.

	// 示例2x1RubbleBase is an abstract 类, it is not an actual 图格, but the other 2 classes in this 文件 will reuse the 纹理 and SetStaticDefaults code shown here because they inherit from it. 
	public abstract class Example2x1RubbleBase : ModTile
	{
		// 我们 want 两者 tiles to use the same 纹理
		public override string Texture => "ExampleMod/Content/Tiles/Example2x1Rubble";

		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileObsidianKill[Type] = true;

			DustType = DustID.Stone;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.DrawYOffset = 2;

			TileObjectData.addTile(Type);

			AddMapEntry(new Color(152, 171, 198));
		}
	}

	// 这是 the fake 图格 that 将 placed by the Rubblemaker.
	public class Example2x1RubbleFake : Example2x1RubbleBase
	{
		public override void SetStaticDefaults() {
			// 调用 to base SetStaticDefaults. Must inherit static defaults from base 类型 
			base.SetStaticDefaults();

			// 添加 rubble variant, all existing styles, to Rubblemaker, allowing to place this 图格 by consuming ExampleBlock
			FlexibleTileWand.RubblePlacementMedium.AddVariations(ModContent.ItemType<Items.Placeable.ExampleBlock>(), Type, 0, 1, 2, 3, 4, 5);

			// Tiles placed by Rubblemaker 放下 the 项 用于 place them.
			RegisterItemDrop(ModContent.ItemType<Items.Placeable.ExampleBlock>());
		}
	}

	// 这是 the natural 图格, this 版本 is placed during 世界 生成 在 RubbleWorldGen 类.
	public class Example2x1RubbleNatural : Example2x1RubbleBase
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			// 默认情况下, the TileObjectData.Style2x1 图格 we copied in Example2x1RubbleBase has LavaDeath = 真. Natural rubble tiles don't have this behavior, so we 想要 be immune to lava.
			TileObjectData.GetTileData(Type, 0).LavaDeath = false;
		}

		public override void DropCritterChance(int i, int j, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance) {
			wormChance = 6;
		}
	}
}