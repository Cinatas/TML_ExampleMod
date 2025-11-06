using ExampleMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	public class ExampleResourceAccessory : ModItem
	{
		public static readonly int ResourceBoost = 100;

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ResourceBoost);

		public override void SetDefaults() {
			Item.width = 26;
			Item.height = 32;
			Item.maxStack = 1;
			Item.value = Item.sellPrice(gold: 5);
			Item.accessory = true;
			Item.rare = ItemRarityID.Red;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			var modPlayer = player.GetModPlayer<ExampleResourcePlayer>();
			modPlayer.exampleResourceMax2 += ResourceBoost; // 添加 100 到 exampleResourceMax2, 即 our max 例如 资源.
			modPlayer.exampleResourceRegenRate *= 6f; // multiply our 资源 regeneration 速度 by 6.
			modPlayer.exampleResourceMagnet = true; // Boosts pickup 范围 例如ResourcePickup
		}
	}
}
