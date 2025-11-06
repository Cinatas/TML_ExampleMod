using ExampleMod.Common.Systems;
using ExampleMod.Content.Biomes;
using ExampleMod.Content.EmoteBubbles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalNPCs
{
	// 这是我们为所有 NPC 添加常规表情的地方
	public class EmotePickerGlobalNPC : GlobalNPC
	{
		public override int? PickEmote(NPC npc, Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor) {
			// 如果玩家在示例生物群系中，则将生物群系表情添加到列表
			// 并且有随机机会，此表情出现的可能性会降低
			if (Main.rand.NextBool(2) && ModContent.GetInstance<ExampleSurfaceBiome>().IsBiomeActive(closestPlayer)) {
				emoteList.Add(ModContent.EmoteBubbleType<ExampleBiomeEmote>());
			}

			// 如果仆从 Boss 被击败，其表情应该出现
			if (Main.rand.NextBool(3) && DownedBossSystem.downedMinionBoss) {
				emoteList.Add(ModContent.EmoteBubbleType<MinionBossEmote>());
			}

			// 不要忘记返回基础方法，因为我们不想完全覆盖表情
			return base.PickEmote(npc, closestPlayer, emoteList, otherAnchor);
		}
	}
}
