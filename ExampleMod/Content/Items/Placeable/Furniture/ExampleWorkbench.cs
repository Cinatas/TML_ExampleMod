using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable.Furniture
{
	public class ExampleWorkbench : ModItem
	{
		public override void SetDefaults() {
			// ModContent.TileType<Tiles.Furniture.ExampleWorkbench>() retrieves the ID 的 图格 that this 项 should place when used.
			// 默认ToPlaceableTile handles 设置 various 项 values that placeable items use
			// 悬停 over DefaultToPlaceableTile in Visual Studio to read the documentation!
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.ExampleWorkbench>());
			Item.width = 28; // The 项 纹理's 宽度
			Item.height = 14; // The 项 纹理's 高度
			Item.value = 150;
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.CraftingObjects;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.WorkBench)
				.AddIngredient<ExampleItem>(10)
				.Register();
		}
	}
}
