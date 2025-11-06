using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Items
{
	// This file shows a very simple example of a GlobalItem class. GlobalItem hooks are called on all items 在 game and are suitable for sweeping changes like 
	// 添加ing additional data to all items 在 game. Here we simply adjust the damage 的 Copper Shortsword item, as it is simple to understand. 
	// See other GlobalItem classes in ExampleMod to see other ways that GlobalItem 可以 used.
	public class CopperShortsword : GlobalItem
	{
		public override void SetDefaults(Item item) {
			if (item.type == ItemID.CopperShortsword) { // Here we make sure to only change Copper Shortsword by checking item.type in an if statement
				item.damage = 50;	// 更改d original CopperShortsword's damage to 50!
			}
		}
	}
}
