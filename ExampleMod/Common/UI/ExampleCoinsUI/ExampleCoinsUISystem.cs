using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ExampleMod.Common.UI.ExampleCoinsUI
{
	[Autoload(Side = ModSide.Client)] // This attribute makes this class only load on a particular side. Naturally this makes sense here since UI should only be a thing clientside. Be wary though that accessing this class serverside will error
	public class ExampleCoinsUISystem : ModSystem
	{
		private UserInterface exampleCoinUserInterface;
		internal ExampleCoinsUIState exampleCoinsUI;

		// 这两个方法将设置我们的自定义 UI 的状态，导致它显示或隐藏
		public void ShowMyUI() {
			exampleCoinUserInterface?.SetState(exampleCoinsUI);
		}

		public void HideMyUI() {
			exampleCoinUserInterface?.SetState(null);
		}

		public override void Load() {
			// 创建可以在不同 UIState 之间交换的自定义界面
			exampleCoinUserInterface = new UserInterface();
			// 创建自定义 UIState
			exampleCoinsUI = new ExampleCoinsUIState();

			// Activate 在 UIState 未初始化时调用 Initialize()，然后调用 OnActivate，然后在每个子元素上调用 Activate
			exampleCoinsUI.Activate();
		}

		public override void UpdateUI(GameTime gameTime) {
			// 在这里我们在自定义 UI 上调用 .Update 并将其传播到其状态和底层元素
			if (exampleCoinUserInterface?.CurrentState != null) {
				exampleCoinUserInterface?.Update(gameTime);
			}
		}

		// 添加ing a custom layer to the vanilla layer list that will call .Draw on your interface if it has a state
		// 设置ting the InterfaceScaleType to UI for appropriate UI scaling
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1) {
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"ExampleMod: Coins Per Minute",
					delegate {
						if (exampleCoinUserInterface?.CurrentState != null) {
							exampleCoinUserInterface.Draw(Main.spriteBatch, new GameTime());
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
