using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	public class ExampleBar : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 25;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 59; // Influences the 库存 排序 顺序. 59 is PlatinumBar, higher is more valuable.

			// Chlorophyte Extractinator can exchange items. Here we tell it to 允许 a one-way exchanging of 5 ExampleBar for 2 ChlorophyteBar.
			ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 5, ItemID.ChlorophyteBar, 2);
		}

		public override void SetDefaults() {
			// ModContent.TileType returns the ID 的 图格 that this 项 should place when used. ModContent.TileType<T>() 方法 returns an integer ID 的 图格 provided to it through its generic 类型 参数 (the 类型 in 角度 brackets)
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.ExampleBar>());
			Item.width = 20;
			Item.height = 20;
			Item.value = 750; // The 成本 的 项 in 铜币 coins. (1 = 1 铜币, 100 = 1 银币, 1000 = 1 金币, 10000 = 1 铂金币)
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleOre>(4)
				.AddTile(TileID.Furnaces)
				.Register();
		}
	}
}
