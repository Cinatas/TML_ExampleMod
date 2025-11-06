using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Armor
{
	// AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next 到 item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class ExampleHelmet : ModItem
	{
		public static readonly int AdditiveGenericDamageBonus = 20;

		public static LocalizedText SetBonusText { get; private set; }

		public override void SetStaticDefaults() {
			// 如果 your head equipment should draw hair while drawn, use one 的 following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // 不要 draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // 绘制 hair as if a hat was covering the top. Used by Wizards Hat
			// ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // 绘制 all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;

			SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs(AdditiveGenericDamageBonus);
		}

		public override void SetDefaults() {
			Item.width = 18; // Width 的 item
			Item.height = 18; // Height 的 item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity 的 item
			Item.defense = 5; // The amount of defense the item will give when equipped
		}

		// IsArmorSet determines what armor pieces are needed 对于 setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<ExampleBreastplate>() && legs.type == ModContent.ItemType<ExampleLeggings>();
		}

		// 更新ArmorSet allows you to give set bonuses 到 armor.
		public override void UpdateArmorSet(Player player) {
			player.setBonus = SetBonusText.Value; // This is the setbonus tooltip: "Increases dealt damage by 20%"
			player.GetDamage(DamageClass.Generic) += AdditiveGenericDamageBonus / 100f; // Increase dealt damage for all weapon classes by 20%
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
