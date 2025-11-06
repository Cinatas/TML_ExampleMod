using ExampleMod.Content.Items.Mounts;
using ExampleMod.Content.Pets.ExampleLightPet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Systems
{
	// 此类展示如何向原版箱子添加额外物品。
	// 此示例只是添加额外物品。其他场景可能需要更复杂的逻辑。
	// 如果此代码令人困惑，请了解"for 循环"和"continue"和"break"关键字：https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/jump-statements
	public class ChestItemWorldGen : ModSystem
	{
		// 我们为此使用 PostWorldGen，因为我们希望确保在添加物品之前放置所有箱子。
		public override void PostWorldGen() {
			// 在冰冻箱中放置一些额外物品：
			// These are the 3 new items we will place.
			int[] itemsToPlaceInFrozenChests = { ModContent.ItemType<ExampleMountItem>(), ModContent.ItemType<ExampleLightPetItem>(), ItemID.PinkJellyfishJar };
			// This variable will help cycle through the items so that different Frozen Chests get different items
			int itemsToPlaceInFrozenChestsChoice = 0;
			// Rather than place items in each chest, we'll place up to 6 items (2 of each). 
			int itemsPlaced = 0;
			int maxItems = 6;
			// Loop over all the chests
			for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++) {
				Chest chest = Main.chest[chestIndex];
				if (chest == null) {
					continue;
				}
				Tile chestTile = Main.tile[chest.x, chest.y];
				// We need to check if the current chest is the Frozen Chest. We need to check that it exists and has the TileType and TileFrameX values corresponding 到 Frozen Chest.
				// If you look 在 sprite for Chests by extracting Tiles_21.xnb, you'll see th在 12th chest is the Frozen Chest. Since we are counting from 0, this is where 11 comes from. 36 comes 从 width of each tile including padding. An alternate approach is to check the wiki and looking 对于 "Internal Tile ID" section 在 infobox: https://terraria.wiki.gg/wiki/Frozen_Chest
				if (chestTile.TileType == TileID.Containers && chestTile.TileFrameX == 11 * 36) {
					// We have found a Frozen Chest
					// If we don't want to add one 的 items to every Frozen Chest, we can randomly skip this chest with a 33% chance.
					if (WorldGen.genRand.NextBool(3))
						continue;
					// Next we need to find the first empty slot for our item
					for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++) {
						if (chest.item[inventoryIndex].type == ItemID.None) {
							// Place the item
							chest.item[inventoryIndex].SetDefaults(itemsToPlaceInFrozenChests[itemsToPlaceInFrozenChestsChoice]);
							// Decide 在 next item that 将 placed.
							itemsToPlaceInFrozenChestsChoice = (itemsToPlaceInFrozenChestsChoice + 1) % itemsToPlaceInFrozenChests.Length;
							// Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(WorldGen.genRand.Next(itemsToPlaceInFrozenChests));
							itemsPlaced++;
							break;
						}
					}
				}
				// Once we've placed as many items as we wanted, break out 的 loop
				if (itemsPlaced >= maxItems) {
					break;
				}
			}
		}
	}
}
