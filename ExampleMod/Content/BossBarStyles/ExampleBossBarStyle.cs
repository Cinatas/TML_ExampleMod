using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace ExampleMod.Content.BossBars
{
	// 展示 very basic code for a custom Boss 条 style 即 selectable 在 菜单 in "界面"
	// 如果 you want custom NPC selection code for which Boss bars to 显示, 返回 真 for PreventUpdate, and implement your own code 在 更新 hook
	public class ExampleBossBarStyle : ModBossBarStyle
	{
		public override bool PreventDraw => true; // 防止s the default drawing code

		public override void Draw(SpriteBatch spriteBatch, IBigProgressBar currentBar, BigProgressBarInfo info) {
			if (currentBar == null) {
				return;
				// 仅 draw if vanilla decided to draw one (we let it 更新 because we didn't override PreventUpdate to 返回 真)
			}

			if (currentBar is CommonBossBigProgressBar) {
				// 如果 这是一个 regular 条 without 任何 special features, we draw our own thing. Sadly, "life to 显示" is not a 变量 我们可以 access,
				// but since we are dealing 与 very basic implementation that only tracks a single NPC, 我们可以 use "info"

				NPC npc = Main.npc[info.npcIndexToAimAt];
				float lifePercent = Utils.Clamp(npc.life / (float)npc.lifeMax, 0f, 1f);

				// Unused 方法 by vanilla, which simply draws 一些 boxes that represent a Boss 条 (fixed 位置, colors, no 图标)
				BigProgressBarHelper.DrawBareBonesBar(spriteBatch, lifePercent);

				if (info.showText && BigProgressBarSystem.ShowText) {
					// 如果 the 条 can currently draw 文本 and the 设置 for it is enabled, draw the "life/lifeMax" 文本 在 中心 的 条 (位置 code taken from DrawBareBonesBar)
					Rectangle barDimensions = Utils.CenteredRectangle(Main.ScreenSize.ToVector2() * new Vector2(0.5f, 1f) + new Vector2(0f, -50f), new Vector2(400f, 20f));
					BigProgressBarHelper.DrawHealthText(spriteBatch, barDimensions, 2 * Vector2.UnitY, npc.life, npc.lifeMax);
				}
			}
			else {
				// 如果 a 条 with special behavior is currently selected, draw it instead because we don't have access to its special features

				currentBar.Draw(ref info, spriteBatch);
			}
		}
	}
}
