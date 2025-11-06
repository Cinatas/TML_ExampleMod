using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	internal class ExampleLamp : ModItem
	{
		// 此示例 uses LocalizedText.Empty to 防止 any 翻译 键 from being generated. This 可以 used for items that definitely won't have a 工具提示, keeping the localization 文件 cleaner.
		public override LocalizedText Tooltip => LocalizedText.Empty;

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.ExampleLamp>());
			Item.width = 10;
			Item.height = 24;
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
}
