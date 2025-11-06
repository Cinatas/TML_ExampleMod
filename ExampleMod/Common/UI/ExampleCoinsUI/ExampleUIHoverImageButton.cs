using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace ExampleMod.Common.UI.ExampleCoinsUI
{
	// 此 ExampleUIHoverImageButton 类继承自 UIImageButton。 
	// 继承是 UI 设计的好工具。 
	// 通过继承，我们从 UIImageButton 免费获得图像绘制、MouseOver 声音和淡入淡出
	// 我们添加了一些代码以允许按钮在悬停时显示文本工具提示
	internal class ExampleUIHoverImageButton : UIImageButton
	{
		// 悬停时将显示的工具提示文本
		internal string hoverText;

		public ExampleUIHoverImageButton(Asset<Texture2D> texture, string hoverText) : base(texture) {
			this.hoverText = hoverText;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			// When you override UIElement methods, don't forget call the base method
			// This helps to keep the basic behavior of the UIElement
			base.DrawSelf(spriteBatch);

			// IsMouseHovering becomes true when the mouse hovers over the current UIElement
			if (IsMouseHovering)
				Main.hoverItemName = hoverText;
		}
	}
}
