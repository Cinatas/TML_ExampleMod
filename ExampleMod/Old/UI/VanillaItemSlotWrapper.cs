using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace ExampleMod.UI
{
	// This 类 wraps the vanilla ItemSlot 类 into a UIElement. The ItemSlot 类 was made before the 用户界面 system was made, so it can't be used normally with UIState. 
	// By wrapping the vanilla ItemSlot 类, we can easily use ItemSlot.
	// ItemSlot isn't very modder friendly and operates 基于 a "Context" 数字 that dictates how the 槽位 behaves when 左, 右, or shift clicked and the 背景 used when drawn. 
	// If you want more 控制, you might 需要 write your own UIElement.
	// I've added basic functionality for validating the 项 attempting to be placed 在 槽位 via the validItem Func. 
	// See ExamplePersonUI for usage and use the Awesomify chat 选项 of Example Person to see in action.
	internal class VanillaItemSlotWrapper : UIElement
	{
		internal Item Item;
		private readonly int _context;
		private readonly float _scale;
		internal Func<Item, bool> ValidItemFunc;

		public VanillaItemSlotWrapper(int context = ItemSlot.Context.BankItem, float scale = 1f) {
			_context = context;
			_scale = scale;
			Item = new Item();
			Item.SetDefaults(0);

			Width.Set(Main.inventoryBack9Texture.Width * scale, 0f);
			Height.Set(Main.inventoryBack9Texture.Height * scale, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			float oldScale = Main.inventoryScale;
			Main.inventoryScale = _scale;
			Rectangle rectangle = GetDimensions().ToRectangle();

			if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface) {
				Main.LocalPlayer.mouseInterface = true;
				if (ValidItemFunc == null || ValidItemFunc(Main.mouseItem)) {
					// 处理 handles all the 点击 and 悬停 actions based 在 context.
					ItemSlot.Handle(ref Item, _context);
				}
			}
			// 绘制 draws the 槽位 itself and 项. Depending on context, the 颜色 will change, as will drawing other things like 堆叠 counts.
			ItemSlot.Draw(spriteBatch, ref Item, _context, rectangle.TopLeft());
			Main.inventoryScale = oldScale;
		}
	}
}