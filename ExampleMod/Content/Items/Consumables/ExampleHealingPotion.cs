using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Consumables
{
	// This 项 showcases some advanced capabilities of healing potions. It heals a dynamic amount and adjusts its 工具提示 accordingly.
	// 一个 typical healing 药水 can get rid 的 ModifyTooltips and GetHealLife methods and just assign 项.healLife.
	// 一个 魔力 药水 is exactly the same, except 项.healMana is used instead. (Also GetHealMana 将 used for dynamic 魔力 recovery values)
	public class ExampleHealingPotion : ModItem
	{
		public static LocalizedText RestoreLifeText { get; private set; }

		public override void SetStaticDefaults() {
			RestoreLifeText = this.GetLocalization(nameof(RestoreLifeText));

			Item.ResearchUnlockCount = 30;
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(gold: 1);

			Item.healLife = 100; // While we change the actual healing 值 in GetHealLife, 项.healLife still needs to be higher than 0 对于 项 to be considered a healing 项
			Item.potion = true; // 使 it so this 项 applies 药水 sickness on use and allows it to be used with quick heal
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			// 查找 the 工具提示 line that corresponds to 'Heals ... life'
			// 参见 https://tmodloader.github.io/tModLoader/html/class_terraria_1_1_mod_loader_1_1_tooltip_line.html for a 列表 of vanilla 工具提示 line names
			TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "HealLife");

			if (line != null) {
				// 更改 the 文本 to 'Heals max/2 (max/4 when quick healing) life'
				line.Text = Language.GetTextValue("CommonItemTooltip.RestoresLife", RestoreLifeText.Format(Main.LocalPlayer.statLifeMax2 / 2, Main.LocalPlayer.statLifeMax2 / 4));
			}
		}

		public override void GetHealLife(Player player, bool quickHeal, ref int healValue) {
			// 使 the 项 heal half the 玩家's max 生命值 normally, or one fourth if used with quick heal
			healValue = player.statLifeMax2 / (quickHeal ? 4 : 2);
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile(TileID.Bottles) // Making this 配方 be crafted at bottles will automatically make Alchemy 表格's 效果 apply to its ingredients.
				.Register();
		}
	}
}