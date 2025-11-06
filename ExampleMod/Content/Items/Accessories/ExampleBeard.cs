using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	//Showcases a beard vanity 项 that uses a greyscale 精灵 which gets its' 颜色 从 players' hair
	//Requires ArmorIDs.Beard.Sets.UseHairColor and 项.颜色 to be used properly
	//For a beard with a fixed 颜色, 删除 the above mentioned code
	[AutoloadEquip(EquipType.Beard)]
	public class ExampleBeard : ModItem
	{
		public override void SetStaticDefaults() {
			ArmorIDs.Beard.Sets.UseHairColor[Item.beardSlot] = true;
		}

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 14;
			Item.maxStack = 1;
			Item.color = Main.LocalPlayer.hairColor;
			Item.value = Item.sellPrice(0, 1);
			Item.accessory = true;
			Item.vanity = true;
		}
	}
}
