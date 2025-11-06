using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ExampleMod.Common.UI.ExampleCoinsUI
{
	// 此 DraggableUIPanel 类继承自 UIPanel
	// 继承是 用户界面 设计的好工具。通过继承，我们从 UIPanel 免费获得背景绘制
	// 我们添加了一些代码以允许面板被拖动
	// 我们还添加了一些代码以确保如果面板被拖到外面或屏幕调整大小，面板将弹回边界
	// UIPanel 不会阻止玩家在单击鼠标时使用物品，所以我们也添加了这个
	public class ExampleDraggableUIPanel : UIPanel
	{
		// 存储 the 偏移 从 顶部 左 的 UIPanel while dragging
		private Vector2 offset;
		// A 标志 that checks if the 面板 is currently being dragged
		private bool dragging;

		public override void LeftMouseDown(UIMouseEvent evt) {
			// When you override UIElement methods, don't forget call the base 方法
			// This helps to keep the basic behavior 的 UIElement
			base.LeftMouseDown(evt);
			// When the 鼠标 按钮 is down on this 元素, then we 开始 dragging
			if (evt.Target == this) {
				DragStart(evt);
			}
		}

		public override void LeftMouseUp(UIMouseEvent evt) {
			base.LeftMouseUp(evt);
			// When the 鼠标 按钮 is up, then we 停止 dragging
			if (evt.Target == this) {
				DragEnd(evt);
			}
		}

		private void DragStart(UIMouseEvent evt) {
			// The 偏移 变量 helps to remember the 位置 的 面板 relative 到 鼠标 位置
			// So no matter where you 开始 dragging the 面板, it will 移动 smoothly
			offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
			dragging = true;
		}

		private void DragEnd(UIMouseEvent evt) {
			Vector2 endMousePosition = evt.MousePosition;
			dragging = false;

			Left.Set(endMousePosition.X - offset.X, 0f);
			Top.Set(endMousePosition.Y - offset.Y, 0f);

			Recalculate();
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			// 检查ing ContainsPoint 然后 设置 mouseInterface to 真 is very common
			// This causes clicks on this UIElement to not cause the 玩家 to use current items
			if (ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}

			if (dragging) {
				Left.Set(Main.mouseX - offset.X, 0f); // Main.MouseScreen.X and Main.mouseX are the same
				Top.Set(Main.mouseY - offset.Y, 0f);
				Recalculate();
			}

			// Here we check if the DraggableUIPanel is outside the Parent UIElement rectangle
			// (In our example, the parent 将 ExampleCoinsUI, a UIState. 这意味着 that we are checking th在 DraggableUIPanel is outside the whole 屏幕)
			// By doing this and some simple math, 我们可以 snap the 面板 back on 屏幕 if the 用户 resizes his 窗口 or 否则 changes resolution
			var parentSpace = Parent.GetDimensions().ToRectangle();
			if (!GetDimensions().ToRectangle().Intersects(parentSpace)) {
				Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
				Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
				// Recalculate forces the 用户界面 system to do the positioning math again.
				Recalculate();
			}
		}
	}
}
