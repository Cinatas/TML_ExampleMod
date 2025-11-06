using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable.Furniture
{
	public class MinionBossRelic : ModItem
	{
		public override void SetDefaults() {
			// Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle 以及 as setting a few values that are common across all placeable items
			// place style (here 默认情况下 0) is important if you decide to have more than one relic share the same tile type (more on that 在 tiles' code)
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MinionBossRelic>(), 0);

			Item.width = 30;
			Item.height = 40;
			Item.rare = ItemRarityID.Master;
			Item.master = true; // This makes sure that "Master" displays 在 tooltip, as the rarity only changes the item name color
			Item.value = Item.buyPrice(0, 5);
		}
	}
}
