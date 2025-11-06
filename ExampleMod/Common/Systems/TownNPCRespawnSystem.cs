using System.IO;
using Terraria.ModLoader.IO;
using Terraria.ModLoader;
using Terraria;
using ExampleMod.Content.NPCs;

namespace ExampleMod.Common.Systems
{
	// 此类跟踪特定城镇 NPC 是否曾在此世界中生成。如果已生成，则不再需要其生成条件即可在同一世界中重生。此行为是 Terraria v1.4.4 的新功能，并非自动的，需要代码来支持它。
	// 无法撤消的生成条件，例如击败 Boss，不需要这样的跟踪，因为当城镇 NPC 尝试重生时这些条件仍然为真。例如，像 ExamplePerson 那样检查玩家库存中物品的生成条件需要跟踪。
	public class TownNPCRespawnSystem : ModSystem
	{
		// 跟踪 ExamplePerson 是否曾在此世界中生成
		public static bool unlockedExamplePersonSpawn = false;

		// 在世界中救出的城镇 NPC 将遵循类似的实现，唯一的区别是如何将值设置为 真。
		// public static bool savedExamplePerson = 假;

		public override void ClearWorld() {
			unlockedExamplePersonSpawn = false;
		}

		public override void SaveWorldData(TagCompound tag) {
			tag[nameof(unlockedExamplePersonSpawn)] = unlockedExamplePersonSpawn;
		}

		public override void LoadWorldData(TagCompound tag) {
			unlockedExamplePersonSpawn = tag.GetBool(nameof(unlockedExamplePersonSpawn));

			// This line sets unlockedExamplePersonSpawn to 真 if an ExamplePerson is already 在 世界. This is only needed because unlockedExamplePersonSpawn was added in an 更新 to this mod, meaning that existing users might have unlockedExamplePersonSpawn incorrectly set to 假.
			// If you are tracking Town NPC unlocks from your initial mod release, then this isn't necessary.
			unlockedExamplePersonSpawn |= NPC.AnyNPCs(ModContent.NPCType<ExamplePerson>());
		}

		public override void NetSend(BinaryWriter writer) {
			writer.Write(new BitsByte(
				unlockedExamplePersonSpawn
			));
		}

		public override void NetReceive(BinaryReader reader) {
			BitsByte flags = reader.ReadByte();
			flags.Retrieve(
				ref unlockedExamplePersonSpawn
			);
		}
	}
}
