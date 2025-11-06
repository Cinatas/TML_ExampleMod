using ExampleMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Armor
{
	// AutoloadEquip attribute automatically attaches an equip 纹理 to this 项.
	// Providing the EquipType.Head 值 here will result in TML expecting a X_Head.png 文件 to be placed next 到 项's main 纹理.
	[AutoloadEquip(EquipType.Head)]
	public class ExampleHood : ModItem
	{
		public static readonly int ManaCostReductionPercent = 10;

		public static LocalizedText SetBonusText { get; private set; }

		public override void SetStaticDefaults() {
			// 我们 are passing in "{0}" into WithFormatArgs to 替换 "{0}" with itself because we do the final formatting for this LocalizedText in UpdateArmorSet itself according 到 players current ReversedUpDownArmorSetBonuses 设置.
			SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs("{0}", ManaCostReductionPercent);
		}

		public override void SetDefaults() {
			Item.width = 18; // 宽度 的 项
			Item.height = 18; // 高度 的 项
			Item.value = Item.sellPrice(gold: 1); // How many coins the 项 is worth
			Item.rare = ItemRarityID.Green; // The 稀有度 的 项
			Item.defense = 4; // The amount of 防御 the 项 will give when equipped
		}

		// IsArmorSet determines what 护甲 pieces are needed 对于 setbonus to take 效果
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<ExampleBreastplate>() && legs.type == ModContent.ItemType<ExampleLeggings>();
		}

		// 更新ArmorSet allows you to give set bonuses 到 护甲.
		public override void UpdateArmorSet(Player player) {
			// 这是 the setbonus 工具提示:
			//   Double tap or hold DOWN/UP to toggle various 护甲 shadow effects
			//   10% reduced 魔力 成本
			player.setBonus = SetBonusText.Format(Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN"));
			player.manaCost -= ManaCostReductionPercent / 100f; // Reduces 魔力 成本 by 10%
			player.GetModPlayer<ExampleArmorSetBonusPlayer>().ExampleSetHood = true;
		}

		public override void ArmorSetShadows(Player player) {
			var exampleArmorSetBonusPlayer = player.GetModPlayer<ExampleArmorSetBonusPlayer>();
			if(exampleArmorSetBonusPlayer.ShadowStyle == 1) {
				player.armorEffectDrawShadow = true;
			}
			else if(exampleArmorSetBonusPlayer.ShadowStyle == 2) {
				player.armorEffectDrawOutlines = true;
			}
			else if (exampleArmorSetBonusPlayer.ShadowStyle == 3) {
				player.armorEffectDrawOutlinesForbidden = true;
			}
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
