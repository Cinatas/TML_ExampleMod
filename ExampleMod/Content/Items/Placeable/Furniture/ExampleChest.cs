using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable.Furniture
{
	public class ExampleChest : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.ExampleChest>());
			// 项.placeStyle = 1; // 使用 this to place the 箱子 in its locked style
			Item.width = 26;
			Item.height = 22;
			Item.value = 500;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}

	public class ExampleChestKey : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 3; // 生物群系 keys usually take 1 项 to research instead.
		}

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.GoldenKey);
		}
	}
}
