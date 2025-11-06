using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Armor.Vanity
{
	// This tells tModLoader to look for a 纹理 called MinionBossMask_Head, 即 the 纹理 在 玩家
	// 然后 registers this 项 to be accepted in head equip slots
	[AutoloadEquip(EquipType.Head)]
	public class MinionBossMask : ModItem
	{
		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 28;

			// 常见 values for 每个 Boss mask
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 75);
			Item.vanity = true;
			Item.maxStack = 1;
		}
	}
}
