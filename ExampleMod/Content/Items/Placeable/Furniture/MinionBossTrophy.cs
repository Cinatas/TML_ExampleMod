using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable.Furniture
{
	public class MinionBossTrophy : ModItem
	{
		public override void SetDefaults() {
			// Vanilla has m任何 useful methods like these, use them! This substitutes 设置 项.createTile and 项.placeStyle 以及 as 设置 一些 values that are common across all placeable items
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MinionBossTrophy>());

			Item.width = 32;
			Item.height = 32;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 1);
		}
	}
}
