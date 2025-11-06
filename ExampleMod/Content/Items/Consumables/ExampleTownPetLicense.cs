using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ExampleMod.Content.NPCs.TownPets;
using ExampleMod.Common.Systems;

namespace ExampleMod.Content.Items.Consumables
{
	public class ExampleTownPetLicense : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.UseSound = SoundID.Item92;
			Item.width = 28;
			Item.height = 28;
			Item.maxStack = Item.CommonMaxStack;
			Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(0, 5));
		}

		public override bool? UseItem(Player player) {
			// 仅 do something if the License hasn't been used before or the Town 宠物 exists 在 世界.
			int npcType = ModContent.NPCType<ExampleTownPet>(); // The NPC 类型 对于 Town 宠物.
			if (player.ItemAnimationJustStarted && (!ExampleTownPetSystem.boughtExampleTownPet || NPC.AnyNPCs(npcType))) {
				if (player.whoAmI == Main.myPlayer) {
					ExampleTownPetUnlockOrExchangePet(ref ExampleTownPetSystem.boughtExampleTownPet, npcType, this.GetLocalizationKey("LicenseExampleTownPetUse")); // 修改d NPC.UnlockOrExchangePet 方法.
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// <br>The vanilla 方法 NPC.UnlockOrExchangePet will not work for our modded Town Pets because the NetMessage only works with vanilla NPCs.</br>
		/// <br>This 版本 uses a ModPacket for that instead.</br>
		/// </summary>
		/// <param 名称="petBoughtFlag">The bool that determines if the License has been used once. It doesn't really have 任何thing to do with buying.</param>
		/// <param 名称="npcType">The NPC 类型 对于 Town 宠物.</param>
		/// <param 名称="textKeyForLicense">The localization 路径 for when the License has been used 对于 first 时间.</param>
		public static void ExampleTownPetUnlockOrExchangePet(ref bool petBoughtFlag, int npcType, string textKeyForLicense) {
			Color color = new(50, 255, 130); // Chat 消息 颜色.
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				if (!petBoughtFlag || NPC.AnyNPCs(npcType)) {
					// Send the ModPacket if used by a 玩家 in multiplayer 以便 other players can receive the change, too.
					// ModPacket is handled in ExampleMod.Networking.cs
					ModPacket packet = ModContent.GetInstance<ExampleMod>().GetPacket();
					packet.Write((byte)ExampleMod.MessageType.ExampleTownPetUnlockOrExchange);
					packet.Send();
				}
			}
			else if (!petBoughtFlag) {
				petBoughtFlag = true; // the bool 即 set and saved in our ModSystem 类.
				ChatHelper.BroadcastChatMessage(NetworkText.FromKey(textKeyForLicense), color); // Send the chat 消息.
				NetMessage.TrySendData(MessageID.WorldData); // 同步 the change for 每个one.
			}
			else if (NPC.RerollVariationForNPCType(npcType)) {
				ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.PetExchangeSuccess"), color);
			}
			else {
				ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.PetExchangeFail"), color);
			}
		}
	}
}