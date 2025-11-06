using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	/// <summary>
	/// This 项 showcases one 的 ways for you to do something when an 项 is bought from an NPC with a 商店.
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

			// 对于 fun, we'll give the buying 玩家 a 50% 概率 to die whenever they 购买 this 项 from an NPC.
			if (!Main.rand.NextBool()) {
				return;
			}

			// 这是 only ever called 在 local 客户端, so the local 玩家 will do.
			Player player = Main.LocalPlayer;
			player.KillMe(PlayerDeathReason.ByCustomReason(DeathMessage.Format(player.name)), 9999, 0);
		}
	}
}
