using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Armor
{
	// AutoloadEquip attribute automatically attaches an equip 纹理 to this 项.
	// Providing the EquipType.Head 值 here will result in TML expecting a X_Head.png 文件 to be placed next 到 项's main 纹理.
	[AutoloadEquip(EquipType.Head)]
	public class ExampleHelmet : ModItem
	{
		public static readonly int AdditiveGenericDamageBonus = 20;

		public static LocalizedText SetBonusText { get; private set; }

		public override void SetStaticDefaults() {
			// 如果 your head equipment should draw hair while drawn, use one 的 following:
			// ArmorIDs.Head.Sets.DrawHead[项.headSlot] = 假; // 不要 draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[项.headSlot] = 真; // 绘制 hair as if a hat was covering the 顶部. Used by Wizards Hat
			// ArmorIDs.Head.Sets.DrawFullHair[项.headSlot] = 真; // 绘制 all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[项.headSlot] = 真;

			SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs(AdditiveGenericDamageBonus);
		}

		public override void SetDefaults() {
			Item.width = 18; // 宽度 的 项
			Item.height = 18; // 高度 的 项
			Item.value = Item.sellPrice(gold: 1); // How m任何 coins the 项 is worth
			Item.rare = ItemRarityID.Green; // The 稀有度 的 项
			Item.defense = 5; // The amount of 防御 the 项 will give when equipped
		}

		// IsArmorSet determines what 护甲 pieces are needed 对于 setbonus to take 效果
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<ExampleBreastplate>() && legs.type == ModContent.ItemType<ExampleLeggings>();
		}

		// 更新ArmorSet allows you to give set bonuses 到 护甲.
		public override void UpdateArmorSet(Player player) {
			player.setBonus = SetBonusText.Value; // 这是 setbonus 工具提示: "Increases dealt 伤害 by 20%"
			player.GetDamage(DamageClass.Generic) += AdditiveGenericDamageBonus / 100f; // Increase dealt 伤害 for all 武器 classes by 20%
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
