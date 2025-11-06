using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalBossBars
{
	// 展示你可以在绘制 Boss 血条方面做的事情
	public class ExampleGlobalBossBar : GlobalBossBar
	{
		public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams) {
			if (npc.type == NPCID.EyeofCthulhu) {
				drawParams.IconColor = Main.DiscoColor;
			}

			return true;
		}

		public override void PostDraw(SpriteBatch spriteBatch, NPC npc, BossBarDrawParams drawParams) {
			if (npc.type == NPCID.EyeofCthulhu) {
				string text = "GlobalBossBar Showcase";
				var font = FontAssets.MouseText.Value;
				Vector2 size = font.MeasureString(text);
				// 在 Boss 血条上居中绘制，向上偏移，否则会与生命值文本重叠
				spriteBatch.DrawString(font, text, drawParams.BarCenter - size / 2 + new Vector2(0, -30), Color.White);
			}
		}
	}
}
