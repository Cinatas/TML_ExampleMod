using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	public class ExampleModAccessorySlot1 : ModAccessorySlot
	{
		// 如果类为空，所有内容都将默认为基本原版槽位。
	}

	public class ExampleCustomLocationAndTextureSlot : ModAccessorySlot
	{
		// 我们将槽位放置在地图的中心，决定不遵循内部 用户界面 处理
		public override Vector2? CustomLocation => new Vector2(Main.screenWidth / 2, 3 * Main.screenHeight / 4);

		// 当有染料时，我们将绘制时装槽位
		public override bool DrawVanitySlot => !DyeItem.IsAir;

		//     We will use our 'custom' textures
		// 背景 Textures -> 一般来说, you can use most 的 existing vanilla ones to get different colors
		public override string VanityBackgroundTexture => "Terraria/Images/Inventory_Back14"; // yellow
		public override string FunctionalBackgroundTexture => "Terraria/Images/Inventory_Back7"; // pale blue

		// 图标 textures. Nominal 图像 大小 is 32x32. Piggy bank is 16x24 but it still works as it's drawn centered.
		public override string VanityTexture => "Terraria/Images/Item_" + ItemID.PiggyBank;

		// 我们将 keep it hidden most 的 时间 以便 it isn't an intrusive example
		public override bool IsHidden() {
			return IsEmpty; // 仅 show when it contains an 项, items can 结束 up in functional slots via quick swap (右 点击 饰品)
		}
	}

	public class ExampleModWingSlot : ModAccessorySlot
	{
		public override bool CanAcceptItem(Item checkItem, AccessorySlotType context) {
			if (checkItem.wingSlot > 0) // if is Wing, then can go in 槽位
				return true;

			return false; // 否则 nothing in 槽位
		}

		// Designates our 槽位 to be a priority for putting wings in to. NOTE: use ItemLoader.CanEquipAccessory if aiming for restricting other slots from having wings!
		public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo) {
			if (item.wingSlot > 0) // If is Wing, then we 想要 prioritize it to go in to our 槽位.
				return true;

			return false;
		}

		public override bool IsEnabled() {
			if (Player.armor[0].headSlot >= 0) // if 玩家 is wearing a helmet, because flight safety
				return true; // Then can use 槽位

			return false; // 不能 use 槽位
		}

		// 覆盖s the default behavior where a disabled 饰品 槽位 will 允许 retrieve items if it contains items
		public override bool IsVisibleWhenNotEnabled() {
			return false; // We set to 假 to just not 显示 如果不是 Enabled. NOTE: this does not affect behavior when mod is unloaded!
		}

		// 图标 textures. Nominal 图像 大小 is 32x32. 将 centered 在 槽位.
		public override string FunctionalTexture => "Terraria/Images/Item_" + ItemID.CreativeWings;

		// 可以 用于 modify stuff while the 鼠标 is hovering over the 槽位.
		public override void OnMouseHover(AccessorySlotType context) {
			// 我们将 modify the 悬停 文本 while an 项 is not 在 槽位, 以便 it says "Wings".
			switch (context) {
				case AccessorySlotType.FunctionalSlot:
				case AccessorySlotType.VanitySlot:
					Main.hoverItemName = "Wings";
					break;
				case AccessorySlotType.DyeSlot:
					Main.hoverItemName = "Wings Dye";
					break;
			}
		}
	}
}
