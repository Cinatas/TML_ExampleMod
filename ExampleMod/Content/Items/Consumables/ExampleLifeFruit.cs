using ExampleMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Consumables
{
	// 此文件 showcases how to create an 项 that increases the 玩家's 最大 生命值 on use.
	// Within your ModPlayer, you need to 保存/加载 a 计数 of usages. You also need to 同步 the 数据 to other players.
	// overlay used to 显示 the custom life fruit 可以 found in Common/用户界面/ResourceDisplay/VanillaLifeOverlay.cs
	internal class ExampleLifeFruit : ModItem
	{
		public static readonly int MaxExampleLifeFruits = 10;
		public static readonly int LifePerFruit = 10;

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifePerFruit, MaxExampleLifeFruits);

		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 10;
		}

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.LifeFruit);
		}

		public override bool CanUseItem(Player player) {
			// This check prevents this 项 from being used before vanilla 生命值 upgrades are maxed out.
			return player.ConsumedLifeCrystals == Player.LifeCrystalMax && player.ConsumedLifeFruit == Player.LifeFruitMax;
		}

		public override bool? UseItem(Player player) {
			// Moving the exampleLifeFruits check from CanUseItem to here allows this example fruit to still "be used" like Life Fruit 可以
			// when 在 max allowed, but it will just play the 动画 and not affect the 玩家's max life
			if (player.GetModPlayer<ExampleStatIncreasePlayer>().exampleLifeFruits >= MaxExampleLifeFruits) {
				// 返回ing 空 will make the 项 不 consumed
				return null;
			}

			// 此方法 handles permanently increasing the 玩家's max 生命值 and displaying the green heal 文本
			player.UseHealthMaxIncreasingItem(LifePerFruit);

			// This 字段 tracks how many 的 example fruit have been consumed
			player.GetModPlayer<ExampleStatIncreasePlayer>().exampleLifeFruits++;

			return true;
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
