using ExampleMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	public class ExampleImmunityAccessory : ModItem
	{
		public override void SetDefaults() {
			Item.width = 26;
			Item.height = 32;
			Item.maxStack = 1;
			Item.value = Item.sellPrice(0, 1);
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			// 设置 the HasExampleImmunityAcc bool to 真 to ensure we have this 饰品
			// And apply the changes in ModPlayer.PostHurt correctly
			player.GetModPlayer<ExampleImmunityPlayer>().HasExampleImmunityAcc = true;
		}
	}
}
