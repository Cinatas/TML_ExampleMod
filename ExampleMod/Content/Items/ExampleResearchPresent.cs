using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	public class ExampleResearchPresent : ModItem
	{
		public override void SetStaticDefaults() {
			// 必须 researched as m任何 times as there are items 在 game.
			// 如果 fully researched, and a new mod is added, it 将come un-researched and require that much more
			// Research amount will never go down or over the max 限制 of 9999.
			Item.ResearchUnlockCount = Utils.Clamp(ItemLoader.ItemCount, 1, 9999);

			// 使用 a MonoMod hook to 允许 our presents to run through the Sacrifice system.
			On_CreativeUI.SacrificeItem_refItem_refInt32_bool += OnSacrificeItem;
		}

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.GoodieBag);
		}

		// 这允许 对于 present to be researched even when you already have infinite 的m.
		// 这是 not a standard use 的 research system, but allows for re-running a 'research complete' 效果
		private CreativeUI.ItemSacrificeResult OnSacrificeItem(On_CreativeUI.orig_SacrificeItem_refItem_refInt32_bool orig,
				ref Item item, out int amountWeSacrificed, bool returnRemainderToPlayer) {

			// 如果 the 项 being sacrificed has the same 类型 as us (is an ExampleResearchPresent) and is fully researched
			if (item.type == Type && CreativeUI.GetSacrificesRemaining(Type) == 0) {

				// Re-unlock all accessories, incase mods have changed
				OnResearched(true);

				// 我们 always lose a present when researching them, even if you already had infinite 的m. To show the 用户 something happened
				item.stack -= 1;

				// This code is copied 从 结束 of SacrificeItem
				if (item.stack > 0 && returnRemainderToPlayer) {
					item.position.X = Main.player[Main.myPlayer].Center.X - item.width / 2;
					item.position.Y = Main.player[Main.myPlayer].Center.Y - item.height / 2;
					item = Main.LocalPlayer.GetItem(Main.myPlayer, item, GetItemSettings.InventoryUIToInventorySettings);
				}

				// 这是 the amount the sacrifice 计数器 goes up by. We didn't actually change the total 数字 of sacrifices, so this is 0
				amountWeSacrificed = 0;

				// 返回 SacrificedAndDone, so the 动画 and effects happen
				return CreativeUI.ItemSacrificeResult.SacrificedAndDone;
			}

			// 否则, call the original 方法 to run the default behavior
			return orig(ref item, out amountWeSacrificed, returnRemainderToPlayer);
		}

		public override void OnResearched(bool fullyResearched) {
			if (fullyResearched) {
				LearnAllAccessories();
			}
			else {
				// Attempt to learn a 随机 饰品 for each present sacrificed
				int count = 0;
				for (int j = Item.stack; j > 0; j--) {
					if (LearnRandomAccessory()) {
						count++;
					}
				}
				if (count == 0) {
					Main.NewText("No new accessory...");
				}
				else {
					Main.NewText("Learned " + count + " new accessor" + (count == 1 ? "y" : "ies") + " !");
				}
			}
		}

		// try 1000 随机 项 ids and if we randomly select an 饰品, attempt learn it
		private bool LearnRandomAccessory() {
			for (int i = 0; i < 1000; i++) {
				int type = Main.rand.Next(1, ItemLoader.ItemCount);
				if (ContentSamples.ItemsByType[type].accessory) {
					if (CreativeUI.ResearchItem(type) == CreativeUI.ItemSacrificeResult.SacrificedAndDone) {
						return true;
					}
				}
			}
			return false;
		}

		private void LearnAllAccessories() {
			for (int i = 1; i < ItemLoader.ItemCount; i++) {
				if (ContentSamples.ItemsByType[i].accessory) {
					CreativeUI.ResearchItem(i);
				}
			}

			Main.NewText("You got all accessories!");
		}
	}
}
