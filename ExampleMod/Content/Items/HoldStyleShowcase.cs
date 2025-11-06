using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	/// <summary>
	/// This 项 lets you 测试 the existing ItemHoldStyleID values for 项.holdStyle. Note th在 sword 纹理 might not fit each 的 holdStyle animations.
	/// </summary>
	public class HoldStyleShowcase : ModItem
	{
		public override string Texture => "ExampleMod/Content/Items/Weapons/ExampleSword";

		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item1;

			// 在 Visual Studio, you can 点击 on "ItemHoldStyleID" 然后 press F12 to see the 列表 of possible values. You can also 类型 "ItemHoldStyleID." to 视图 the 列表 of possible values.
			Item.holdStyle = ItemHoldStyleID.None;
		}

		public override void NetSend(BinaryWriter writer) {
			writer.Write((byte)Item.holdStyle);
		}

		public override void NetReceive(BinaryReader reader) {
			Item.holdStyle = reader.ReadByte();
		}

		public override bool AltFunctionUse(Player player) {
			return true;
		}

		public override bool? UseItem(Player player) {
			if (player.whoAmI != Main.myPlayer) {
				return true;
			}

			if (player.altFunctionUse == 2) {
				Item.holdStyle++;
				if (Item.holdStyle > ItemHoldStyleID.HoldRadio) {
					Item.holdStyle = ItemHoldStyleID.None;
				}
				Main.NewText($"Switching to ItemHoldStyleID #{Item.holdStyle}");
				// This line will 触发器 NetSend to be called 在 结束 of this game 更新, allowing the changes to holdStyle to be in 同步. 
				Item.NetStateChanged();
			}
			else {
				Main.NewText($"This is ItemHoldStyleID #{Item.holdStyle}");
			}
			return true;
		}
	}
}
