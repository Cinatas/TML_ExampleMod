using ExampleMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Consumables
{
	// 此文件 showcases how to create an 项 that increases the 玩家's 最大 魔力 on use.
	// Within your ModPlayer, you need to 保存/加载 a 计数 of usages. You also need to 同步 the 数据 to other players.
	// overlay used to 显示 the custom 魔力 crystals 可以 found in Common/用户界面/ResourceDisplay/VanillaManaOverlay.cs
	internal class ExampleManaCrystal : ModItem
	{
		public static readonly int MaxExampleManaCrystals = 10;
		public static readonly int ManaPerCrystal = 10;

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaPerCrystal, MaxExampleManaCrystals);

		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 10;
		}

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.ManaCrystal);
		}

		public override bool CanUseItem(Player player) {
			// This check prevents this 项 from being used before vanilla 魔力 upgrades are maxed out.
			return player.ConsumedManaCrystals == Player.ManaCrystalMax;
		}

		public override bool? UseItem(Player player) {
			// Moving the exampleManaCrystals check from CanUseItem to here allows this example crystal to still "be used" like 魔力 Crystals 可以
			// when 在 max allowed, but it will just play the 动画 and not affect the 玩家's max 魔力
			if (player.GetModPlayer<ExampleStatIncreasePlayer>().exampleManaCrystals >= MaxExampleManaCrystals) {
				// 返回ing 空 will make the 项 不 consumed
				return null;
			}

			// 此方法 handles permanently increasing the 玩家's max 魔力 and displaying the blue 魔力 文本
			player.UseManaMaxIncreasingItem(ManaPerCrystal);

			// This 字段 tracks how many 的 example crystals have been consumed
			player.GetModPlayer<ExampleStatIncreasePlayer>().exampleManaCrystals++;

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
