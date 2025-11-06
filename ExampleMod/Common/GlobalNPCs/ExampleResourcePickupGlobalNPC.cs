using ExampleMod.Common.Players;
using ExampleMod.Content.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalNPCs
{
	public class ExampleResourcePickupGlobalNPC : GlobalNPC
	{
		public override void OnKill(NPC npc) {
			// 在这里，我们以类似于心和星掉落的方式处理 ExampleResourcePickup 掉落。
			// 此代码密切模仿 NPC.NPCLoot_DropCommonLifeAndMana 方法以保持一致性。

			Player closestPlayer = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];
			ExampleResourcePlayer exampleResourcePlayer = closestPlayer.GetModPlayer<ExampleResourcePlayer>();

			// 这些条件与心和星掉落逻辑相匹配，但可以根据资源的预期稀有度进行调整。
			if (!NPCID.Sets.NeverDropsResourcePickups[npc.type] && closestPlayer.RollLuck(6) == 0 && npc.lifeMax > 1 && npc.damage > 0 && Main.rand.NextBool(2) && exampleResourcePlayer.exampleResourceCurrent < exampleResourcePlayer.exampleResourceMax2) {
				Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ModContent.ItemType<ExampleResourcePickup>());
			}
		}
	}
}
