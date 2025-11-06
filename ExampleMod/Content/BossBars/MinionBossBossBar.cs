using ExampleMod.Content.NPCs.MinionBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace ExampleMod.Content.BossBars
{
	// 展示 a custom Boss 条 with basic logic for displaying the 图标, life, and shields properly.
	// Has no custom 纹理, meaning it will use the default vanilla Boss 条 纹理
	public class MinionBossBossBar : ModBossBar
	{
		private int bossHeadIndex = -1;

		public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame) {
			// 显示 the previously assigned head 索引
			if (bossHeadIndex != -1) {
				return TextureAssets.NpcHeadBoss[bossHeadIndex];
			}
			return null;
		}

		public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax) {
			// 在这里 the game wants to know if to draw the Boss 条 or not. 返回 假 whenever the conditions don't apply.
			// 如果 there is no possibility of returning 假 (or 空) the 条 will get drawn at times when it shouldn't, so write defensive code!

			NPC npc = Main.npc[info.npcIndexToAimAt];
			if (!npc.active)
				return false;

			// 我们 assign bossHeadIndex here because we need to use it in GetIconTexture
			bossHeadIndex = npc.GetBossHeadTextureIndex();

			life = npc.life;
			lifeMax = npc.lifeMax;

			if (npc.ModNPC is MinionBossBody body) {
				// 我们 did all the 计算 work on RemainingShields inside the body NPC already so we just have to fetch the 值 again
				shield = body.MinionHealthTotal;
				shieldMax = body.MinionMaxHealthTotal;
			}

			return true;
		}
	}
}
