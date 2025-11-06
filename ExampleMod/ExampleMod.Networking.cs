using ExampleMod.Common.Players;
using ExampleMod.Common.Systems;
using ExampleMod.Content.Items.Consumables;
using ExampleMod.Content.NPCs;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod
{
	// 这是一个部分类，意味着它的一些部分被拆分到其他文件中。请参阅 ExampleMod.*.cs 以了解其他部分。
	// 该类是部分类，用于将相似的代码组织在一起，以便更清楚地了解相关内容。
	// 这个类继承自 ExampleMod.cs 中的 Mod 类。如果使用此文件作为模组 Mod 类的模板，请确保在你自己的代码中继承自 Mod 类（": Mod"）。
	partial class ExampleMod
	{
		internal enum MessageType : byte
		{
			ExampleStatIncreasePlayerSync,
			ExampleTeleportToStatue,
			ExampleDodge,
			ExampleTownPetUnlockOrExchange,
			ExampleResourceEffect
		}

		// 重写此方法以处理为此模组发送的网络数据包。
		//TODO: 将面向对象的数据包引入 tML，以避免这种上帝类级别的硬编码。
		public override void HandlePacket(BinaryReader reader, int whoAmI) {
			MessageType msgType = (MessageType)reader.ReadByte();

			switch (msgType) {
				// 此消息同步 ExampleStatIncreasePlayer.exampleLifeFruits 和 ExampleStatIncreasePlayer.exampleManaCrystals
				case MessageType.ExampleStatIncreasePlayerSync:
					byte playerNumber = reader.ReadByte();
					ExampleStatIncreasePlayer examplePlayer = Main.player[playerNumber].GetModPlayer<ExampleStatIncreasePlayer>();
					examplePlayer.ReceivePlayerSync(reader);

					if (Main.netMode == NetmodeID.Server) {
						// 将更改转发给其他客户端
						examplePlayer.SyncPlayer(-1, whoAmI, false);
					}
					break;
				case MessageType.ExampleTeleportToStatue:
					if (Main.npc[reader.ReadByte()].ModNPC is ExamplePerson person && person.NPC.active) {
						person.StatueTeleport();
					}

					break;
				case MessageType.ExampleDodge:
					ExampleDamageModificationPlayer.HandleExampleDodgeMessage(reader, whoAmI);
					break;
				case MessageType.ExampleTownPetUnlockOrExchange:
					// 调用我们在许可证物品中创建的自定义函数。
					ExampleTownPetLicense.ExampleTownPetUnlockOrExchangePet(ref ExampleTownPetSystem.boughtExampleTownPet, ModContent.NPCType<Content.NPCs.TownPets.ExampleTownPet>(), ModContent.GetInstance<ExampleTownPetLicense>().GetLocalizationKey("LicenseExampleTownPetUse"));
					break;
				case MessageType.ExampleResourceEffect:
					ExampleResourcePlayer.HandleExampleResourceEffectMessage(reader, whoAmI);
					break;
				default:
					Logger.WarnFormat("ExampleMod: Unknown Message type: {0}", msgType);
					break;
			}
		}
	}
}