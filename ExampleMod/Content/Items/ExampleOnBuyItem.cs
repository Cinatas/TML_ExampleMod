using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	/// <summary>
	/// This item showcases one 的 ways for you to do something when an item is bought from an NPC with a shop.
	/// </summary>
	public class ExampleOnBuyItem : ModItem
	{
		public static LocalizedText DeathMessage { get; private set; }

		public override void SetStaticDefaults() {
			// 参见 the localization files f或更多 info! (Localization/en-US.hjson)
			DeathMessage = this.GetLocalization(nameof(DeathMessage));
		}

		public override void SetDefaults() {
			Item.width = 16;
			Item.height = 16;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(silver: 1, copper: 50);
			Item.maxStack = 9999;
		}

		// 注意 that alternatively, you can use the ModPlayer.PostBuyItem hook to achieve the same functionality!
		public override void OnCreated(ItemCreationContext context) {
			if (context is not BuyItemCreationContext buyContext) {
				return;
			}

			// 对于 fun, we'll give the buying player a 50% chance to die whenever they buy this item from an NPC.
			if (!Main.rand.NextBool()) {
				return;
			}

			// 这是 only ever called 在 local client, so the local player will do.
			Player player = Main.LocalPlayer;
			player.KillMe(PlayerDeathReason.ByCustomReason(DeathMessage.Format(player.name)), 9999, 0);
		}
	}
}
