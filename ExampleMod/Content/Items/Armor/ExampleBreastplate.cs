using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Armor
{
	// AutoloadEquip attribute automatically attaches an equip 纹理 to this 项.
	// Providing the EquipType.Body 值 here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png 精灵-sheet files to be placed next 到 项's main 纹理.
	[AutoloadEquip(EquipType.Body)]
	public class ExampleBreastplate : ModItem
	{
		public static readonly int MaxManaIncrease = 20;
		public static readonly int MaxMinionIncrease = 1;

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxManaIncrease, MaxMinionIncrease);

		public override void SetDefaults() {
			Item.width = 18; // 宽度 的 项
			Item.height = 18; // 高度 的 项
			Item.value = Item.sellPrice(gold: 1); // How many coins the 项 is worth
			Item.rare = ItemRarityID.Green; // The 稀有度 的 项
			Item.defense = 6; // The amount of 防御 the 项 will give when equipped
		}

		public override void UpdateEquip(Player player) {
			player.buffImmune[BuffID.OnFire] = true; // 使 the 玩家 immune to Fire
			player.statManaMax2 += MaxManaIncrease; // Increase how many 魔力 points the 玩家 can have by 20
			player.maxMinions += MaxMinionIncrease; // Increase how many minions the 玩家 can have by one
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe().AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
