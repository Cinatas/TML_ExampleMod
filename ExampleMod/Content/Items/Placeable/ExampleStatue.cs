using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	// 项 用于 place the statue.
	public class ExampleStatue : ModItem
	{
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.ArmorStatue);
			Item.createTile = ModContent.TileType<Tiles.ExampleStatue>();
			Item.placeStyle = 0;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}