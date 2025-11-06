using ExampleMod.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ExampleMod.Common.Systems
{
	public class ExampleWorldHeaderSystem : ModSystem
	{
		public override void SaveWorldHeader(TagCompound tag) {
			tag["ExampleModExists"] = true;
		}

		public override void Load() {
			On_UIWorldListItem.DrawSelf += (orig, self, spriteBatch) => {
				orig(self, spriteBatch);
				DrawWorldSelectItemOverlay(self, spriteBatch);
			};
		}

		private void DrawWorldSelectItemOverlay(UIWorldListItem uiItem, SpriteBatch spriteBatch) {
			if (MenuLoader.CurrentMenu is not ExampleModMenu)
				return;

			if (!uiItem.Data.TryGetHeaderData(this, out var data) || !data.GetBool("ExampleModExists"))
				return;

			var dims = uiItem.GetInnerDimensions();
			var pos = new Vector2(dims.X + 400, dims.Y);
			Utils.DrawBorderString(spriteBatch, "EM played before", pos, Color.BlueViolet);
		}
	}

	public class ExampleWorldHeaderPlayer : ModPlayer
	{
		public override void OnEnterWorld() {
			// 此数据只能在单人模式下检查
			if (Main.netMode != NetmodeID.SinglePlayer) {
				return;
			}

			// 检查此世界是否至少使用模组的特定版本生成。
			// 在 v2023.8 中添加了跟踪用于生成世界的模组，因此如果 WorldGenModsRecorded 为 false，我们无法确定生成此世界时是否启用了 ExampleMod。
			if (!Main.ActiveWorldFileData.WorldGenModsRecorded) {
				return;
			}

			if (Main.ActiveWorldFileData.TryGetModVersionGeneratedWith("ExampleMod", out Version modVersion)) {
				if (modVersion < new Version(1, 0)) {
					// 在这里，我们可以有一条关于世界缺少在此模组的 v1.0 中添加的新生物群系的消息，该生物群系不会出现在世界中，因为它是在那之前生成的。
				}
			}
			else {
				Main.NewText(Language.GetTextValue(Mod.GetLocalizationKey("NotPresentDuringWorldGenMessage")), Color.Orange);
			}
		}
	}
}
