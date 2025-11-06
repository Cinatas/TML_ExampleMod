using ExampleMod.Content.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalNPCs
{
	internal class DamageModificationGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public bool exampleDefenseDebuff;

		public override void ResetEffects(NPC npc) {
			exampleDefenseDebuff = false;
		}

		public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
			if (exampleDefenseDebuff) {
				// 为了获得最佳效果，防御减益应该是乘法的
				modifiers.Defense *= ExampleDefenseDebuff.DefenseMultiplier;
			}
		}

		public override void DrawEffects(NPC npc, ref Color drawColor) {
			// 这个简单的颜色效果表明增益是活动的
			if (exampleDefenseDebuff) {
				drawColor.G = 0;
			}
		}
	}
}
