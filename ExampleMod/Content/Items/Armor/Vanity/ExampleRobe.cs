using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Armor.Vanity
{
	// 参见 also: ExampleCostume
	[AutoloadEquip(EquipType.Body)]
	public class ExampleRobe : ModItem
	{
		public override void Load() {
			// code below runs only if we're not loading on a server
			if (Main.netMode == NetmodeID.Server) {
				return;
			}

			// By passing this (the ModItem) in到 item parameter we can reference it later in GetEquipSlot with just the item's name
			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
		}

		public override void SetStaticDefaults() {
			// 隐藏sHands defaults to true which we don't want.
			ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = false;
		}

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 14;
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}

		public override void SetMatch(bool male, ref int equipSlot, ref bool robes) {
			// By changing the equipSlot 到 leg equip texture slot, the leg texture will now be drawn 在 player
			// We're changing the leg slot so we set this to true
			robes = true;
			// 在这里 we can get the equip slot by name since we referenced the item when adding the texture
			// 你 can also cache the equip slot in a variable when you add it so this way you don't have to call GetEquipSlot
			equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
		}
	}
}
