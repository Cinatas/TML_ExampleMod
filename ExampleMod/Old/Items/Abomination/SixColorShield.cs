using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.Items.Abomination
{
	public class SixColorShield : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Six-Color Shield");
			Tooltip.SetDefault("Creates elemental energy to protect you when damaged.");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(10, 4));
		}

		public override void SetDefaults() {
			item.width = 24;
			item.height = 24;
			item.value = Item.buyPrice(0, 10, 0, 0);
			item.rare = ItemRarityID.Cyan;
			item.expert = true;
			item.accessory = true;
			item.damage = 120;
			item.magic = true;
			item.knockBack = 2f;
			item.defense = 6;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetModPlayer<ExamplePlayer>().elementShield = true;
		}

		public override Color? GetAlpha(Color lightColor) {
			return Color.White;
		}

		public override bool CanEquipAccessory(Player player, int slot) {
			if (slot < 10) // 这允许 the 饰品 to equip in Vanity slots with no reservations.
			{
				int maxAccessoryIndex = 5 + player.extraAccessorySlots;
				for (int i = 3; i < 3 + maxAccessoryIndex; i++) {
					// 我们需要 "槽位 != i" because we don't care what is currently 在 槽位 we 将 replacing.
					if (slot != i && player.armor[i].type == ItemID.AnkhShield) {
						return false;
					}
				}
			}
			return true;
		}
	}

	// 我们需要 to do the same 对于 AnkhShield so our restriction is enforced 两者 ways.
	public class AnkhShield : GlobalItem
	{
		public override bool CanEquipAccessory(Item item, Player player, int slot) {
			if (item.type == ItemID.AnkhShield) {
				if (slot < 10) // 这允许 the 饰品 to equip in Vanity slots with no reservations.
				{
					int maxAccessoryIndex = 5 + player.extraAccessorySlots;
					for (int i = 3; i < 3 + maxAccessoryIndex; i++) {
						// 我们需要 "槽位 != i" because we don't care what is currently 在 槽位 we 将 replacing.
						if (slot != i && player.armor[i].type == ItemType<SixColorShield>()) {
							return false;
						}
					}
				}
			}
			return true;
		}
	}
}