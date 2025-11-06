using ExampleMod.Content.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalNPCs
{
	internal class DamageOverTimeGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public bool exampleJavelinDebuff;

		public override void ResetEffects(NPC npc) {
			exampleJavelinDebuff = false;
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage) {
			if (exampleJavelinDebuff) {
				if (npc.lifeRegen > 0) {
					npc.lifeRegen = 0;
				}
				// 计算有多少个 ExampleJavelinProjectile 附加到此 NPC。
				int exampleJavelinCount = 0;
				foreach (var p in Main.ActiveProjectiles) {
					if (p.type == ModContent.ProjectileType<ExampleJavelinProjectile>() && p.ai[0] == 1f && p.ai[1] == npc.whoAmI) {
						exampleJavelinCount++;
					}
				}
				// 请记住，lifeRegen 影响实际生命损失，damage 只是文本。
				// 此处显示的逻辑与原版减益在显示的伤害数字和实际生命损失方面的堆叠方式相匹配。
				npc.lifeRegen -= exampleJavelinCount * 2 * 3;
				if (damage < exampleJavelinCount * 3) {
					damage = exampleJavelinCount * 3;
				}
			}
		}

	}
}
