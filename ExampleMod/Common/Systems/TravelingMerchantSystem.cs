using ExampleMod.Content.NPCs;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ExampleMod.Common.Systems
{
	public class TravelingMerchantSystem : ModSystem
	{
		public override void PreUpdateWorld() {
			ExampleTravelingMerchant.UpdateTravelingMerchant();
		}

		public override void SaveWorldData(TagCompound tag) {
			tag["shopItems"] = ExampleTravelingMerchant.shopItems;
			if (ExampleTravelingMerchant.spawnTime != double.MaxValue) {
				tag["spawnTime"] = ExampleTravelingMerchant.spawnTime;
			}
		}

		public override void LoadWorldData(TagCompound tag) {
			ExampleTravelingMerchant.shopItems.Clear();
			ExampleTravelingMerchant.shopItems.AddRange(tag.Get<List<Item>>("shopItems"));
			if (!tag.TryGet("spawnTime", out ExampleTravelingMerchant.spawnTime)) {
				ExampleTravelingMerchant.spawnTime = double.MaxValue;
			}
		}

		public override void ClearWorld() {
			ExampleTravelingMerchant.shopItems.Clear();
			ExampleTravelingMerchant.spawnTime = double.MaxValue;
		}
 
		public override void NetSend(BinaryWriter writer) {
			// 请注意，每当发送 WorldData 数据包时都会调用 NetSend。
			// 我们使用这个，以便商店物品可以轻松同步到加入的玩家
			// 我们建议模组作者避免过于频繁地发送 WorldData，或用太多数据填充它，以免消耗太多带宽重复发送冗余数据
			// 如果你有大量数据要同步，请考虑发送自定义数据包而不是 WorldData

			writer.Write(ExampleTravelingMerchant.shopItems.Count);
			foreach (Item item in ExampleTravelingMerchant.shopItems) {
				ItemIO.Send(item, writer, writeStack: true);
			}
		}

		public override void NetReceive(BinaryReader reader) {
			ExampleTravelingMerchant.shopItems.Clear();
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++) {
				ExampleTravelingMerchant.shopItems.Add(ItemIO.Receive(reader, readStack: true));
			}
		}
	}
}
