using ExampleMod.Content.Items;
using ExampleMod.Content.Items.Placeable;
using ExampleMod.Content.Items.Placeable.Furniture;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	public class ExampleInventoryPlayer : ModPlayer
	{
		// 添加StartingItems 是一种方法，你可以使用它将物品添加到玩家的起始库存中。
		// 当玩家中核死亡时也会调用它
		// 返回一个包含你想要添加到库存的物品的可枚举对象。
		// This 方法 adds an ExampleItem and 256 金币 ore 到 玩家's 库存.
		//
		// If you know what 'yield 返回' is, you can also use that here, if you prefer so.
		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath) {
			if (mediumCoreDeath) {
				return new[] {
					new Item(ItemID.HealingPotion)
				};
			}

			return new[] {
				new Item(ModContent.ItemType<ExampleItem>()),
				new Item(ItemID.GoldOre, 256),
				new Item(ModContent.ItemType<ExampleBlock>(), 256),
				new Item(ModContent.ItemType<ExampleWall>(), 256),
				new Item(ModContent.ItemType<ExampleOre>(), 256),
				new Item(ModContent.ItemType<ExampleChair>(), 99),
				new Item(ModContent.ItemType<ExampleTable>(), 99),
				new Item(ModContent.ItemType<ExampleChest>(), 99),
				new Item(ModContent.ItemType<ExamplePlatform>(), 256)
			};
		}

		// 修改StartingItems is a more elaborate 版本 of AddStartingItems, which lets you 删除 items
		// that either vanilla or other mods add. You can technically use it to add items 以及, but it's recommended
		// to only do that in AddStartingItems.
		// In this example, we 停止 Terraria from adding an Iron Axe 到 玩家's 库存 if it's journey 模式.
		// (If you want to 停止 another mod from adding an 项, its entry is the mod's internal 名称, e.g itemsByMod["SomeMod"]
		// Terraria's entry is always named just "Terraria"
		public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath) {
			itemsByMod["Terraria"].RemoveAll(item => item.type == ItemID.IronAxe);
		}
	}
}
