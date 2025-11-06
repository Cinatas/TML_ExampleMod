using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalItems
{
	// 这 2 个文件展示了如何制作新的提取机类型。此示例将任何放置火把的物品转换为任何其他放置火把的物品。使用 ModSystem.PostSetupContent 而不是 GlobalItem.SetStaticDefaults 来确定哪些物品是火把物品，因为它需要在所有模组设置其内容后运行。
	public class TorchExtractinatorGlobalItem : GlobalItem
	{
		public override void ExtractinatorUse(int extractType, int extractinatorBlockType, ref int resultType, ref int resultStack) {
			// 如果提取机类型不是火把，我们不会更改任何内容
			if (extractType != ItemID.Torch)
				return;

			// 如果是，我们将堆叠设置为 1 并返回随机火把。如果用户使用叶绿提取机，我们会以 10% 的概率返回超亮火把。
			resultStack = 1;
			if (extractinatorBlockType == TileID.ChlorophyteExtractinator && Main.rand.NextBool(10)) {
				resultType = ItemID.UltrabrightTorch;
				return;
			}

			resultType = Main.rand.Next(TorchExtractinatorModSystem.TorchItems);
		}
	}

	public class TorchExtractinatorModSystem : ModSystem
	{
		internal static List<int> TorchItems;

		public override void PostSetupContent() {
			// 在这里，我们遍历所有物品并查找放置被标记为火把图格的图格的物品。我们将这些物品设置为 ItemID.Torch 的提取机模式，以表明它们都共享火把提取机结果池。
			ItemID.Sets.ExtractinatorMode[ItemID.Torch] = ItemID.Torch;
			TorchItems = new List<int>();

			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				int createTile = ContentSamples.ItemsByType[i].createTile;
				if (createTile != -1 && TileID.Sets.Torch[createTile] && ItemID.Sets.ExtractinatorMode[i] == -1) {
					ItemID.Sets.ExtractinatorMode[i] = ItemID.Torch;
					TorchItems.Add(i);
				}
			}
		}
	}
}
