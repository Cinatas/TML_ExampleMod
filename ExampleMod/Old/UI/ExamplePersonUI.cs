using System.Text;
using ExampleMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.UI
{
	// This 类 represents the UIState for our ExamplePerson Awesomeify chat 函数. It is similar 到 Goblin Tinkerer's Reforge 函数, except it only gives Awesome and ReallyAwesome prefixes. 
	internal class ExamplePersonUI : UIState
	{
		private VanillaItemSlotWrapper _vanillaItemSlot;

		public override void OnInitialize() {
			_vanillaItemSlot = new VanillaItemSlotWrapper(ItemSlot.Context.BankItem, 0.85f) {
				Left = { Pixels = 50 },
				Top = { Pixels = 270 },
				ValidItemFunc = item => item.IsAir || !item.IsAir && item.Prefix(-3)
			};

			// Here we 限制 the items that 可以 placed 在 槽位. We are fine with placing an empty 项 in or a non-empty 项 that 可以 prefixed. Calling 前缀(-3) is the way to know if the 项 in question can take a 前缀 or not.
			Append(_vanillaItemSlot);
		}

		// OnDeactivate is called when the UserInterface switches to a different 状态. In this mod, we switch between no 状态 (空) and this 状态 (ExamplePersonUI).
		// Using OnDeactivate is useful for clearing out 项 slots and returning them 到 玩家, as we do here.
		public override void OnDeactivate() {
			if (_vanillaItemSlot.Item.IsAir) {
				return;
			}

			// QuickSpawnClonedItem will preserve mod 数据 的 项. QuickSpawnItem will just 生成 a fresh 版本 的 项, losing the 前缀.
			Main.LocalPlayer.QuickSpawnClonedItem(_vanillaItemSlot.Item, _vanillaItemSlot.Item.stack);

			// Now that we've spawned the 项 back on到 玩家, we 重置 the 项 by turning it into air.
			_vanillaItemSlot.Item.TurnToAir();

			// Note that in ExamplePerson we call .SetState(new 用户界面.ExamplePersonUI());, thereby creating a new 实例 of this UIState 每次. 
			// You could go with a different design, keeping around the same UIState 实例 if you wanted. This would preserve the UIState between opening and closing. Up to you.
		}

		// 更新 is called on a UIState while it is the active 状态 的 UserInterface.
		// We use 更新 to 处理 automatically closing our 用户界面 when the 玩家 is 不再 talking to our Example Person NPC.
		public override void Update(GameTime gameTime) {
			// 不要 删除 this or the UIElements attached to this UIState will cease to 函数.
			base.Update(gameTime);

			// talkNPC is the 索引 的 NPC the 玩家 is currently talking to. By checking talkNPC, we can tell when the 玩家 switches to another NPC or closes the NPC chat 对话框.
			if (Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != NPCType<ExamplePerson>()) {
				// When that happens, we can set the 状态 of our UserInterface to 空, thereby closing this UIState. This will 触发器 OnDeactivate above.
				GetInstance<ExampleMod>().ExamplePersonUserInterface.SetState(null);
			}
		}

		private bool tickPlayed;

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			// This will hide the 制作 菜单 similar 到 reforge 菜单. For best results this 用户界面 is placed before "Vanilla: 库存" to 防止 1 帧 的 craft 菜单 showing.
			Main.HidePlayerCraftingMenu = true;

			// Here we have a lot of code. This code is mainly adapted 从 vanilla code 对于 reforge 选项.
			// This code draws "Place an 项 here" when no 项 is 在 槽位 and draws the reforge 成本 and a reforge 按钮 when an 项 is 在 槽位.
			// This code could possibly be better as different UIElements that are added and removed, but that's not the main 点 of this example.
			// If you are making a 用户界面, add UIElements in OnInitialize that act on your ItemSlot or other inputs rather than the non-UIElement approach you see below.

			const int SlotX = 50;
			const int SlotY = 270;

			if (_vanillaItemSlot.Item.IsAir) {
				const string Message = "Place an item here to Awesomeify";

				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, Message, new Vector2(SlotX + 50, SlotY), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
				return;
			}

			int awesomePrice = Item.buyPrice(0, 1, 0, 0);
			string costText = Language.GetTextValue("LegacyInterface.46") + ": ";
			int[] coins = Utils.CoinsSplit(awesomePrice);
			var coinsText = new StringBuilder();

			for (int i = 0; i < 4; i++) {
				coinsText.Append($"[c/{Colors.AlphaDarken(Colors.CoinPlatinum).Hex3()}:{coins[3 - i]} {Language.GetTextValue($"LegacyInterface.{15 + i}")}]");
			}

			ItemSlot.DrawSavings(Main.spriteBatch, SlotX + 130, Main.instance.invBottom, true);
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, costText, new Vector2(SlotX + 50, SlotY), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, coinsText.ToString(), new Vector2(SlotX + 50 + FontAssets.MouseText.Value.MeasureString(costText).X, (float)SlotY), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);

			int reforgeX = SlotX + 70;
			int reforgeY = SlotY + 40;
			bool hoveringOverReforgeButton = Main.mouseX > reforgeX - 15 && Main.mouseX < reforgeX + 15 && Main.mouseY > reforgeY - 15 && Main.mouseY < reforgeY + 15 && !PlayerInput.IgnoreMouseInterface;
			Texture2D reforgeTexture = TextureAssets.Reforge[hoveringOverReforgeButton ? 1 : 0].Value;

			Main.spriteBatch.Draw(reforgeTexture, new Vector2(reforgeX, reforgeY), null, Color.White, 0f, reforgeTexture.Size() / 2f, 0.8f, SpriteEffects.None, 0f);

			if (!hoveringOverReforgeButton) {
				tickPlayed = false;
				return;
			}

			Main.hoverItemName = Language.GetTextValue("LegacyInterface.19");

			if (!tickPlayed) {
				SoundEngine.PlaySound(SoundID.MenuTick, -1, -1, 1, 1f, 0f);
			}

			tickPlayed = true;
			Main.LocalPlayer.mouseInterface = true;

			if (!Main.mouseLeftRelease || !Main.mouseLeft || !Main.LocalPlayer.CanBuyItem(awesomePrice, -1) || !ItemLoader.PreReforge(_vanillaItemSlot.Item)) {
				return;
			}

			Main.LocalPlayer.BuyItem(awesomePrice, -1);

			bool favorited = _vanillaItemSlot.Item.favorited;
			int stack = _vanillaItemSlot.Item.stack;

			Item reforgeItem = new Item();

			reforgeItem.netDefaults(_vanillaItemSlot.Item.netID);

			reforgeItem = reforgeItem.CloneWithModdedDataFrom(_vanillaItemSlot.Item);

			// This is the main 效果 of this 槽位. Giving the Awesome 前缀 90% 的 时间 and the ReallyAwesome 前缀 the other 10% 的 时间. All for a constant 1 金币. Useless, but informative.
			if (Main.rand.NextBool(10)) {
				reforgeItem.Prefix(GetInstance<ExampleMod>().PrefixType("ReallyAwesome"));
			}
			else {
				reforgeItem.Prefix(GetInstance<ExampleMod>().PrefixType("Awesome"));
			}

			_vanillaItemSlot.Item = reforgeItem.Clone();
			_vanillaItemSlot.Item.position.X = Main.LocalPlayer.position.X + (float)(Main.LocalPlayer.width / 2) - (float)(_vanillaItemSlot.Item.width / 2);
			_vanillaItemSlot.Item.position.Y = Main.LocalPlayer.position.Y + (float)(Main.LocalPlayer.height / 2) - (float)(_vanillaItemSlot.Item.height / 2);
			_vanillaItemSlot.Item.favorited = favorited;
			_vanillaItemSlot.Item.stack = stack;

			ItemLoader.PostReforge(_vanillaItemSlot.Item);
			PopupText.NewText(PopupTextContext.RegularItemPickup, _vanillaItemSlot.Item, _vanillaItemSlot.Item.stack, true, false);
			SoundEngine.PlaySound(SoundID.Item37, -1, -1);
		}
	}
}
