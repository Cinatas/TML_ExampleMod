using System.IO;
using Terraria.ModLoader.IO;
using Terraria.ModLoader;
using Terraria;

namespace ExampleMod.Common.Systems
{
	// 有关保存世界数据的更多信息，请参阅 ExampleMod/Common/Systems/DownedBossSystem.cs。
	public class ExampleTownPetSystem : ModSystem
	{
		/// <summary>
		/// The bool for whether the Example Town 宠物 License has been used.
		/// <para/> (Doesn't really have 任何thing to do with buying, but it is named as such to 匹配 the vanilla NPC.boughtCat, NPC.boughtDog, and NPC.boughtBunny)
		/// </summary>
		public static bool boughtExampleTownPet = false;

		public override void ClearWorld() {
			boughtExampleTownPet = false;
		}

		public override void SaveWorldData(TagCompound tag) {
			if (boughtExampleTownPet) {
				tag["boughtExampleTownPet"] = true;
			}
		}

		public override void LoadWorldData(TagCompound tag) {
			boughtExampleTownPet = tag.ContainsKey("boughtExampleTownPet");
		}

		public override void NetSend(BinaryWriter writer) {
			BitsByte flags = new BitsByte();
			flags[0] = boughtExampleTownPet;
			writer.Write(flags);
		}

		public override void NetReceive(BinaryReader reader) {
			BitsByte flags = reader.ReadByte();
			boughtExampleTownPet = flags[0];
		}
	}
}