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
		// This method adds an ExampleItem and 256 gold ore to the player's inventory.
		//
		// If you know what 'yield return' is, you can also use that here, if you prefer so.
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

		// 修改StartingItems is a more elaborate version of AddStartingItems, which lets you remove items
		// that either vanilla or other mods add. You can technically use it to add items as well, but it's recommended
		// to only do that in AddStartingItems.
		// In this example, we stop Terraria from adding an Iron Axe to the player's inventory if it's journey mode.
		// (If you want to stop another mod from adding an item, its entry is the mod's internal name, e.g itemsByMod["SomeMod"]
		// Terraria's entry is always named just "Terraria"
		public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath) {
			itemsByMod["Terraria"].RemoveAll(item => item.type == ItemID.IronAxe);
		}
	}
}
