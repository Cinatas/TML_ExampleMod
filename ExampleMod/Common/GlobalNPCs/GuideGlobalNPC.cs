using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalNPCs
{
	public class GuideGlobalNPC : GlobalNPC
	{
		public override bool AppliesToEntity(NPC npc, bool lateInstantiation) {
			return npc.type == NPCID.Guide;
		}

		public override void AI(NPC npc) {
			// 使向导巨大且绿色。
			npc.scale = 1.5f;
			npc.color = Color.ForestGreen;
		}

		public override void EmoteBubblePosition(NPC npc, ref Vector2 position, ref SpriteEffects spriteEffects) {
			// 翻转并将他的表情气泡移到他的前面。
			spriteEffects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			position.X += npc.width * npc.spriteDirection;
		}

		public override void PartyHatPosition(NPC npc, ref Vector2 position, ref SpriteEffects spriteEffects) {
			// 移动向导身上派对帽的位置。
			position.Y -= npc.height / 2f;
			position.X += 4 * npc.spriteDirection;
		}
	}
}
