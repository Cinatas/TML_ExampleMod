using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	/// <summary>
	/// This 项 lets you 测试 the existing ItemUseStyleID values for 项.useStyle. Note th在 sword 纹理 might not fit each 的 useStyle animations.
	/// </summary>
	public class UseStyleShowcase : ModItem
	{
		public override string Texture => "ExampleMod/Content/Items/Weapons/ExampleSword";

		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;

			// 在 Visual Studio, you can 点击 on "ItemUseStyleID" 然后 press F12 to see the 列表 of possible values. You can also 类型 "ItemUseStyleID." to 视图 the 列表 of possible values.
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item1;
		}

		public override void NetSend(BinaryWriter writer) {
			writer.Write((byte)Item.useStyle);
		}

		public override void NetReceive(BinaryReader reader) {
			Item.useStyle = reader.ReadByte();
		}

		public override bool AltFunctionUse(Player player) {
			return true;
		}

		public override bool? UseItem(Player player) {
			if (player.whoAmI != Main.myPlayer) {
				return true;
			}

			if (player.altFunctionUse == 2) {
				Item.useStyle++;
				if (Item.useStyle > ItemUseStyleID.RaiseLamp) {
					Item.useStyle = ItemUseStyleID.Swing;
				}
				Main.NewText($"Switching to ItemUseStyleID #{Item.useStyle}");
				// This line will 触发器 NetSend to be called 在 结束 of this game 更新, allowing the changes to useStyle to be in 同步. 
				Item.NetStateChanged();
			}
			else {
				Main.NewText($"This is ItemUseStyleID #{Item.useStyle}");
			}
			return true;
		}
	}
}
