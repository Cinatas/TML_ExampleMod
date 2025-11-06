using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ExampleMod.Content.BossBars
{
	// 展示 basic Boss 条 code using a custom colored 纹理. It only does visual things, so for a more practical Boss 条, see the other example (MinionBossBossBar)
	// 要 use this, in an NPCs SetDefaults, write:
	//  NPC.BossBar = ModContent.GetInstance<ExampleBossBar>();

	// 请记住 that if the NPC has a Boss head 图标, it will automatically have the common Boss 生命值 条 from vanilla. A ModBossBar is not mandatory for a Boss.

	// 你 can make it so your NPC never shows a Boss 条, 例如 Dungeon Guardian or Lunatic Cultist Clone:
	//  NPC.BossBar = Main.BigBossProgressBar.NeverValid;
	public class ExampleBossBar : ModBossBar
	{
		public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame) {
			return TextureAssets.NpcHead[36]; // Corgi head 图标
		}

		public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams) {
			// 使 the 条 shake the less 生命值 the NPC has
			float lifePercent = drawParams.Life / drawParams.LifeMax;
			float shakeIntensity = Utils.Clamp(1f - lifePercent - 0.2f, 0f, 1f);
			drawParams.BarCenter.Y -= 20f;
			drawParams.BarCenter += Main.rand.NextVector2Circular(0.5f, 0.5f) * shakeIntensity * 15f;

			drawParams.IconColor = Main.DiscoColor;

			return true;
		}
	}
}
