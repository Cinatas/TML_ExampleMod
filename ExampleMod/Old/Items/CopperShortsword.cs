using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Items
{
	// This 文件 shows a very simple example of a GlobalItem 类. GlobalItem hooks are called on all items 在 game and are suitable for sweeping changes like 
	// 添加ing additional 数据 to all items 在 game. Here we simply adjust the 伤害 的 铜币 Shortsword 项, as it is simple to understand. 
	// See other GlobalItem classes in ExampleMod to see other ways that GlobalItem 可以 used.
	public class CopperShortsword : GlobalItem
	{
		public override void SetDefaults(Item item) {
			if (item.type == ItemID.CopperShortsword) { // Here we 确保 to only change 铜币 Shortsword by checking 项.类型 in an if statement
				item.damage = 50;	// 更改d original CopperShortsword's 伤害 to 50!
			}
		}
	}
}
