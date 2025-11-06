using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	public class ExampleOre : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 100;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;

			// This ore can 生成 in slime bodies like other pre-Boss ores. (铜币, tin, iron, etch)
			// It will 放下 in amount from 3 to 13.
			ItemID.Sets.OreDropsFromSlime[Type] = (3, 13);
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.ExampleOre>());
			Item.width = 12;
			Item.height = 12;
			Item.value = 3000;
		}
	}
}