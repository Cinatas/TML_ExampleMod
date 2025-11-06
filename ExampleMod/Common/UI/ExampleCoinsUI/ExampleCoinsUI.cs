using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace ExampleMod.Common.UI.ExampleCoinsUI
{
	// 示例UI 的可见性通过在聊天中输入"/coins"来切换（参见 CoinCommand.cs）
	// 示例CoinsUI 是一个简单的 用户界面 示例，展示如何使用 UIPanel、UIImageButton，甚至自定义 UIElement
	// 有关 用户界面 的更多信息，你可以查看 https://github.com/tModLoader/tModLoader/wiki/Basic-用户界面-元素 和 https://github.com/tModLoader/tModLoader/wiki/Advanced-guide-to-custom-用户界面 
	internal class ExampleCoinsUIState : UIState
	{
		public ExampleDraggableUIPanel CoinCounterPanel;
		public UIMoneyDisplay MoneyDisplay;

		// 在 OnInitialize 中，我们将各种 UIElement 放置到我们的 UIState（此类）上。
		// UIState 类的宽度和高度等于全屏，因此，通常我们首先定义一个 UIElement，它将充当我们的 用户界面 的容器。
		// We then place 各种 other UIElement onto that 容器 UIElement positioned relative 到 容器 UIElement.
		public override void OnInitialize() {
			// Here we define our 容器 UIElement. In DraggableUIPanel.cs, you can see that DraggableUIPanel is a UIPanel with a couple added features.
			CoinCounterPanel = new ExampleDraggableUIPanel();
			CoinCounterPanel.SetPadding(0);
			// 我们需要 to place this UIElement in 关系 to its Parent. Later we 将 calling `base.Append(coinCounterPanel);`. 
			// This means that this 类, ExampleCoinsUI, 将 our Parent. Since ExampleCoinsUI is a UIState, the 左 and 顶部 are relative 到 顶部 左 的 屏幕.
			// 设置Rectangle 方法 帮助 us to set the 位置 and 大小 of UIElement
			SetRectangle(CoinCounterPanel, left: 400f, top: 100f, width: 170f, height: 70f);
			CoinCounterPanel.BackgroundColor = new Color(73, 94, 171);

			// Next, we create another UIElement that we will place. Since we 将 calling `coinCounterPanel.Append(playButton);`, 左 and 顶部 are relative 到 顶部 左 的 coinCounterPanel UIElement. 
			// By properly nesting UIElements, we can 位置 things relatively to each other easily.
			Asset<Texture2D> buttonPlayTexture = ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonPlay");
			ExampleUIHoverImageButton playButton = new ExampleUIHoverImageButton(buttonPlayTexture, "Reset Coins Per Minute Counter");
			SetRectangle(playButton, left: 110f, top: 10f, width: 22f, height: 22f);
			// UIHoverImageButton doesn't do 任何thing when Clicked. Here we assign a 方法 that we'd like to be called when the 按钮 is clicked.
			playButton.OnLeftClick += new MouseEvent(PlayButtonClicked);
			CoinCounterPanel.Append(playButton);

			Asset<Texture2D> buttonDeleteTexture = ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonDelete");
			ExampleUIHoverImageButton closeButton = new ExampleUIHoverImageButton(buttonDeleteTexture, Language.GetTextValue("LegacyInterface.52")); // Localized 文本 for "关闭"
			SetRectangle(closeButton, left: 140f, top: 10f, width: 22f, height: 22f);
			closeButton.OnLeftClick += new MouseEvent(CloseButtonClicked);
			CoinCounterPanel.Append(closeButton);

			// UIMoneyDisplay is a fairly complicated custom UIElement. UIMoneyDisplay handles drawing some 文本 and 硬币 textures.
			// Organization is 键 to managing 用户界面 design. Making a contained UIElement like UIMoneyDisplay will make m任何 things easier.
			MoneyDisplay = new UIMoneyDisplay();
			SetRectangle(MoneyDisplay, 15f, 20f, 100f, 40f);
			CoinCounterPanel.Append(MoneyDisplay);

			Append(CoinCounterPanel);
			// As a recap, ExampleCoinsUI is a UIState, meaning it covers the whole 屏幕. We attach CoinCounterPanel to ExampleCoinsUI some 距离 从 顶部 左 corner.
			// We then place playButton, closeButton, and MoneyDisplay onto CoinCounterPanel so we can easily place these UIElements relative to CoinCounterPanel.
			// Since CoinCounterPanel will 移动, this proper organization will 移动 playButton, closeButton, and MoneyDisplay properly when CoinCounterPanel moves.
		}

		private void SetRectangle(UIElement uiElement, float left, float top, float width, float height) {
			uiElement.Left.Set(left, 0f);
			uiElement.Top.Set(top, 0f);
			uiElement.Width.Set(width, 0f);
			uiElement.Height.Set(height, 0f);
		}

		private void PlayButtonClicked(UIMouseEvent evt, UIElement listeningElement) {
			SoundEngine.PlaySound(SoundID.MenuOpen);
			MoneyDisplay.ResetCoins();
		}

		private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement) {
			SoundEngine.PlaySound(SoundID.MenuClose);
			ModContent.GetInstance<ExampleCoinsUISystem>().HideMyUI();
		}

		public void UpdateValue(int pickedUp) {
			MoneyDisplay.AddCoinsPerMinute(pickedUp);
		}
	}

	public class UIMoneyDisplay : UIElement
	{
		// How m任何 coins have been collected in 铜币
		public long collectedCoins;
		// 时间 from 开始(or 重置) to calculate how m任何 coins collected per minute
		private DateTime? startTime;
		// Saving 硬币 textures to an 数组 to make them easier to access
		private readonly Texture2D[] coinsTextures = new Texture2D[4];

		public UIMoneyDisplay() {
			startTime = null;

			for (int j = 0; j < 4; j++) {
				// Textures may 不 loaded without it
				Main.instance.LoadItem(74 - j);
				coinsTextures[j] = TextureAssets.Item[74 - j].Value;
			}

			// 这允许 clicks to "pass-through" this 元素 到 parent 元素 and 不 consumed by this 元素. This allows ExampleDraggableUIPanel to be dragged even when the 用户 is clicking 在 UIMoneyDisplay.
			IgnoresMouseInteraction = true;
		}
		public void AddCoinsPerMinute(int coins) {
			collectedCoins += coins;

			// We begin to remember the 时间 only after 至少 one 硬币 has been collected
			if (startTime == null)
				startTime = DateTime.Now;
		}

		public int GetCoinsPerMinute() {
			if (collectedCoins == 0)
				return 0;

			// If the 时间 has passed less than minutes, the current 数字 of coins 将 displayed
			return (int)(collectedCoins / Math.Max(1, (DateTime.Now - startTime.Value).TotalMinutes));
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			CalculatedStyle innerDimensions = GetInnerDimensions();
			// 获取ting 顶部 左 位置 of this UIElement
			float shopx = innerDimensions.X;
			float shopy = innerDimensions.Y;

			// 绘制ing first line of coins (current collected coins)
			// CoinsSplit converts the 数字 of 铜币 coins into an 数组 of all types of coins
			DrawCoins(spriteBatch, shopx, shopy, Utils.CoinsSplit(collectedCoins));

			// 绘制ing second line of coins (coins per minute) and 文本 "CPM"
			DrawCoins(spriteBatch, shopx, shopy, Utils.CoinsSplit(GetCoinsPerMinute()), 0, 25);
			Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, "CPM", shopx + (float)(24 * 4), shopy + 25f, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
		}

		private void DrawCoins(SpriteBatch spriteBatch, float shopx, float shopy, int[] coinsArray, int xOffset = 0, int yOffset = 0) {
			for (int j = 0; j < 4; j++) {
				spriteBatch.Draw(coinsTextures[j], new Vector2(shopx + 11f + 24 * j + xOffset, shopy + yOffset), null, Color.White, 0f, coinsTextures[j].Size() / 2f, 1f, SpriteEffects.None, 0f);
				Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, coinsArray[3 - j].ToString(), shopx + 24 * j + xOffset, shopy + yOffset, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
			}
		}

		public void ResetCoins() {
			collectedCoins = 0;
			startTime = DateTime.Now;
		}
	}

	public class MoneyCounterGlobalItem : GlobalItem
	{
		public override bool AppliesToEntity(Item item, bool lateInstantiation) {
			return item.type >= ItemID.CopperCoin && item.type <= ItemID.PlatinumCoin;
		}

		public override bool OnPickup(Item item, Player player) {
			// If we have picked up coins of 任何 类型, then we will 更新 the values in exampleCoinsUI
			ModContent.GetInstance<ExampleCoinsUISystem>().exampleCoinsUI.UpdateValue(item.stack * (item.value / 5));
			return base.OnPickup(item, player);
		}
	}
}
