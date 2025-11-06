using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ExampleMod.Common.Players
{
	// 如果我们将光标悬停在凝胶上，光标样式将改变。
	// 如果我们 shift-单击它，它会改变颜色和稀有度。
	// 另请参阅 GelGlobalItem.cs，我们为凝胶添加了一行工具提示以指示将发生什么
	public class ExampleShiftClickSlotPlayer : ModPlayer
	{
		public override bool ShiftClickSlot(Item[] inventory, int context, int slot) {
			// 应用 our changes if this 项 is in 库存 and is gel
			if (context == ItemSlot.Context.InventoryItem && inventory[slot].type == ItemID.Gel) {
				inventory[slot].color = Main.DiscoColor; // 更改 the 颜色 的 项 into a "随机" 颜色
				inventory[slot].rare = Main.rand.Next(ItemRarityID.Count); // 随机 稀有度
				SoundEngine.PlaySound(SoundID.Item4); // Play 魔力 crystal using 声音

				// 方块 vanilla code so the 项 will 不 picked up when it is clicked.
				return true;
			}
			return base.ShiftClickSlot(inventory, context, slot);
		}

		// Here we override the cursor style
		public override bool HoverSlot(Item[] inventory, int context, int slot) {
			// 应用 our changes if this 项 is in 库存 and is gel
			if (context == ItemSlot.Context.InventoryItem && inventory[slot].type == ItemID.Gel) {
				// If 玩家 is holding shift, use FavoriteStar 纹理 to indicate that a special action 将 performed
				if (ItemSlot.ShiftInUse) {
					Main.cursorOverride = CursorOverrideID.FavoriteStar;
					return true; // 返回 真 to 防止 other things from overriding cursor
				}
			}
			return base.HoverSlot(inventory, context, slot);
		}
	}
}
