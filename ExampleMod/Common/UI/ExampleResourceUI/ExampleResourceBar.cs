using ExampleMod.Common.Players;
using ExampleMod.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace ExampleMod.Common.UI.ExampleResourceUI
{
	// 每当玩家持有 ExampleCustomResourceWeapon 物品时，此自定义 UI 将显示，并将显示在 ExampleResourcePlayer 中跟踪的玩家自定义资源量
	internal class ExampleResourceBar : UIState
	{
		// 对于此条，我们将使用框架纹理，然后在条内使用渐变，因为这是更简单的方法之一，同时看起来还不错。
		// 一旦全部设置好，请确保在 ModSystem 类中为大多数 UI 执行所需的操作。
		private UIText text;
		private UIElement area;
		private UIImage barFrame;
		private Color gradientA;
		private Color gradientB;

		public override void OnInitialize() {
			// 创建一个 UIElement 供所有元素位于其上，这简化了数字，因为嵌套元素可以相对于此元素的左上角定位。 
			// UIElement 是不可见的，没有填充。
			area = new UIElement();
			area.Left.Set(-area.Width.Pixels - 600, 1f); // Place the resource bar 到 left 的 hearts.
			area.Top.Set(30, 0f); // Placing it just a bit below the top 的 screen.
			area.Width.Set(182, 0f); // We 将 placing the following 2 UIElements within this 182x60 area.
			area.Height.Set(60, 0f);

			barFrame = new UIImage(ModContent.Request<Texture2D>("ExampleMod/Common/UI/ExampleResourceUI/ExampleResourceFrame")); // Frame of our resource bar
			barFrame.Left.Set(22, 0f);
			barFrame.Top.Set(0, 0f);
			barFrame.Width.Set(138, 0f);
			barFrame.Height.Set(34, 0f);

			text = new UIText("0/0", 0.8f); // text to show stat
			text.Width.Set(138, 0f);
			text.Height.Set(34, 0f);
			text.Top.Set(40, 0f);
			text.Left.Set(0, 0f);

			gradientA = new Color(123, 25, 138); // A dark purple
			gradientB = new Color(187, 91, 201); // A light purple

			area.Append(text);
			area.Append(barFrame);
			Append(area);
		}

		public override void Draw(SpriteBatch spriteBatch) {
			// This prevents drawing unless we are using an ExampleCustomResourceWeapon
			if (Main.LocalPlayer.HeldItem.ModItem is not ExampleCustomResourceWeapon)
				return;

			base.Draw(spriteBatch);
		}

		// Here we draw our UI
		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			var modPlayer = Main.LocalPlayer.GetModPlayer<ExampleResourcePlayer>();
			// 计算 quotient
			float quotient = (float)modPlayer.exampleResourceCurrent / modPlayer.exampleResourceMax2; // Creating a quotient that represents the difference of your currentResource vs your maximumResource, resulting in a float of 0-1f.
			quotient = Utils.Clamp(quotient, 0f, 1f); // Clamping it to 0-1f so it doesn't go over that.

			// Here we get the screen dimensions 的 barFrame element, then tweak the resulting rectangle to arrive at a rectangle with在 barFrame texture that we will draw the gradient. These values were measured in a drawing program.
			Rectangle hitbox = barFrame.GetInnerDimensions().ToRectangle();
			hitbox.X += 12;
			hitbox.Width -= 24;
			hitbox.Y += 8;
			hitbox.Height -= 16;

			// Now, using this hitbox, we draw a gradient by drawing vertical lines while slowly interpolating between the 2 colors.
			int left = hitbox.Left;
			int right = hitbox.Right;
			int steps = (int)((right - left) * quotient);
			for (int i = 0; i < steps; i += 1) {
				// float percent = (float)i / steps; // Alternate Gradient Approach
				float percent = (float)i / (right - left);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), Color.Lerp(gradientA, gradientB, percent));
			}
		}

		public override void Update(GameTime gameTime) {
			if (Main.LocalPlayer.HeldItem.ModItem is not ExampleCustomResourceWeapon)
				return;

			var modPlayer = Main.LocalPlayer.GetModPlayer<ExampleResourcePlayer>();
			// 设置ting the text per tick to update and show our resource values.
			text.SetText(ExampleResourceUISystem.ExampleResourceText.Format(modPlayer.exampleResourceCurrent, modPlayer.exampleResourceMax2));
			base.Update(gameTime);
		}
	}

	// This class will only be autoloaded/registered if we're not loading on a server
	[Autoload(Side = ModSide.Client)]
	internal class ExampleResourceUISystem : ModSystem
	{
		private UserInterface ExampleResourceBarUserInterface;

		internal ExampleResourceBar ExampleResourceBar;

		public static LocalizedText ExampleResourceText { get; private set; }

		public override void Load() {
			ExampleResourceBar = new();
			ExampleResourceBarUserInterface = new();
			ExampleResourceBarUserInterface.SetState(ExampleResourceBar);

			string category = "UI";
			ExampleResourceText ??= Mod.GetLocalization($"{category}.ExampleResource");
		}

		public override void UpdateUI(GameTime gameTime) {
			ExampleResourceBarUserInterface?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"ExampleMod: Example Resource Bar",
					delegate {
						ExampleResourceBarUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
