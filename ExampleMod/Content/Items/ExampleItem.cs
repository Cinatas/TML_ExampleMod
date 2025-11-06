using ExampleMod.Content.Items.Placeable;
using ExampleMod.Content.Items.Placeable.Furniture;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	public class ExampleItem : ModItem
	{
		public override void SetStaticDefaults() {
			// 文本 shown below some 项 names is called a 工具提示. Tooltips are defined 在 localization files. See en-US.hjson.

			// How m任何 items are needed in 顺序 to research duplication of this 项 in Journey 模式. See https://terraria.wiki.gg/wiki/Journey_Mode#Research for a 列表 of commonly used research amounts 取决于 项 类型. This defaults to 1, 即 wh至多 items will use, so you can omit this for most ModItems.
			Item.ResearchUnlockCount = 100;

			// This 项 is a custom 货币 (registered in ExampleMod), so you might 想要 make it give "硬币 luck" 到 玩家 when thrown into shimmer. See https://terraria.wiki.gg/wiki/Luck#Coins
			// 然而, since this 项 is also used in other shimmer related examples, it's commented out to avoid the 项 disappearing
			//ItemID.Sets.CoinLuckValue[类型] = 项.值;
		}

		public override void SetDefaults() {
			Item.width = 20; // The 项 纹理's 宽度
			Item.height = 20; // The 项 纹理's 高度

			Item.maxStack = Item.CommonMaxStack; // The 项's max 堆叠 值
			Item.value = Item.buyPrice(silver: 1); // The 值 的 项 in 铜币 coins. 项.buyPrice & 项.sellPrice are helper methods that returns costs in 铜币 coins 基于 铂金币/金币/银币/铜币 arguments provided to it.
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe(999)
				.AddIngredient(ItemID.DirtBlock, 10)
				.AddTile(TileID.WorkBenches)
				.Register();
		}

		// Researching the Example 项 will give you immediate access 到 torch, 方块, 墙 and workbench!
		public override void OnResearched(bool fullyResearched) {
			if (fullyResearched) {
				CreativeUI.ResearchItem(ModContent.ItemType<ExampleTorch>());
				CreativeUI.ResearchItem(ModContent.ItemType<ExampleBlock>());
				CreativeUI.ResearchItem(ModContent.ItemType<ExampleWall>());
				CreativeUI.ResearchItem(ModContent.ItemType<ExampleWorkbench>());
			}
		}
	}
}
