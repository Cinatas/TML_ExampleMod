using ExampleMod.Content.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace ExampleMod.Common.UI.ExampleInGameNotification
{
	// 这是用于 InGameNotificationSystem 类的 IInGameNotification 的自定义实现。
	// 它在玩家加入世界时向玩家显示欢迎消息，通过 ExampleInGameNotificationPlayer 控制。
	public class ExampleJoinWorldInGameNotification : IInGameNotification
	{
		// 一旦 5 秒计时器结束，删除此通知。
		public bool ShouldBeRemoved => timeLeft <= 0;

		// 5 seconds, controls how long this 通知 lasts for.
		private int timeLeft = 5 * 60;

		// 我们将用于图标显示的纹理。
		// 让我们保持简单并使用 ExampleItem 的精灵。
		private Asset<Texture2D> iconTexture = TextureAssets.Item[ModContent.ItemType<ExampleItem>()];

		// The 缩放 and Opacity properties are 用于 控制 the 缩放 and opacity 的 用户界面 popup,
		// and are directly taken 从 vanilla 成就 popup 用户界面. This is done for consistency.
		private float Scale {
			get {
				if (timeLeft < 30) {
					return MathHelper.Lerp(0f, 1f, timeLeft / 30f);
				}

				if (timeLeft > 285) {
					return MathHelper.Lerp(1f, 0f, (timeLeft - 285) / 15f);
				}

				return 1f;
			}
		}

		// See the comments for 缩放.
		private float Opacity {
			get {
				if (Scale <= 0.5f) {
					return 0f;
				}

				return (Scale - 0.5f) / 0.5f;
			}
		}

		public void Update() {
			timeLeft--;

			// Keep the 计时器 kept to a 最小 值 of 0 to avoid issues, since we
			// use it for lerping and other effects.
			if (timeLeft < 0) {
				timeLeft = 0;
			}
		}

		public void DrawInGame(SpriteBatch spriteBatch, Vector2 bottomAnchorPosition) {
			// No reason to 继续 drawing if the 通知 is 不再 visible.

			if (Opacity <= 0f) {
				return;
			}

			string title = Language.GetTextValue("Mods.ExampleMod.UI.InGameNotificationTitle");

			// Below is draw-code directly from vanilla with some tweaks to suit our needs.
			// 更改s are minimal; important things to note:
			// - we draw the 面板 with Utils.DrawInvBG,
			// - we calculate the 面板 大小 based 在 称号 大小,
			// - we draw the 称号 and 图标 after the 面板,
			// - we utilize the calculated opacity and 缩放 values.

			float effectiveScale = Scale * 1.1f;
			Vector2 size = (FontAssets.ItemStack.Value.MeasureString(title) + new Vector2(58f, 10f)) * effectiveScale;
			Rectangle panelSize = Utils.CenteredRectangle(bottomAnchorPosition + new Vector2(0f, (0f - size.Y) * 0.5f), size);

			// 检查 if the 鼠标 is hovering over the 通知.
			bool hovering = panelSize.Contains(Main.MouseScreen.ToPoint());

			Utils.DrawInvBG(spriteBatch, panelSize, new Color(64, 109, 164) * (hovering ? 0.75f : 0.5f));
			float iconScale = effectiveScale * 0.7f;
			Vector2 vector = panelSize.Right() - Vector2.UnitX * effectiveScale * (12f + iconScale * iconTexture.Width());
			spriteBatch.Draw(iconTexture.Value, vector, null, Color.White * Opacity, 0f, new Vector2(0f, iconTexture.Width() / 2f), iconScale, SpriteEffects.None, 0f);
			Utils.DrawBorderString(color: new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor / 5, Main.mouseTextColor) * Opacity, sb: spriteBatch, text: title, pos: vector - Vector2.UnitX * 10f, scale: effectiveScale * 0.9f, anchorx: 1f, anchory: 0.4f);

			if (hovering) {
				OnMouseOver();
			}
		}

		private void OnMouseOver() {
			// This 方法 is called when the 用户 hovers over the 通知.

			// 跳过 if we're ignoring 鼠标 输入.
			if (PlayerInput.IgnoreMouseInterface) {
				return;
			}

			// We are now interacting with a 用户界面.
			Main.LocalPlayer.mouseInterface = true;

			if (!Main.mouseLeft || !Main.mouseLeftRelease) {
				return;
			}

			Main.mouseLeftRelease = false;

			// In our example, we just accelerate the exiting 过程 on 点击.
			// If you want it to 关闭 immediately, you can just set timeLeft to 0.
			// 这允许 the 通知 时间 to shrink and fade away, as expected.
			if (timeLeft > 30) {
				timeLeft = 30;
			}
		}

		public void PushAnchor(ref Vector2 positionAnchorBottom) {
			// 锚定 is used for determining how much space a popup takes up, essentially.
			// This is because notifications visually 堆叠. In our case, we 想要 let other notifications
			// go in front of ours once we 开始 fading off, so we 缩放 the 偏移 基于 opacity.
			positionAnchorBottom.Y -= 50f * Opacity;
		}
	}
}