using ExampleMod.Common.Players;
using ExampleMod.Content.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content
{
	/// <summary>
	/// InfoDisplay 即 coupled with <seealso cref="ExampleInfoAccessory"/> and <seealso cref="ExampleInfoDisplayPlayer"/> to show
	/// off how to add a new info 饰品 (例如 a Radar, Lifeform Analyzer, etc.)
	/// </summary>
	public class ExampleInfoDisplay : InfoDisplay
	{
		public static Color RedInfoTextColor => new(255, 19, 19, Main.mouseTextColor);

		// 默认情况下, the vanilla circular outline 纹理 将 used. 
		// This info 显示 has a square 图标 代替 a circular one, so we 需要 use a custom outline 纹理 instead 的 vanilla outline 纹理.
		// 你 will only 需要 use a custom 悬停 纹理 if your info 显示 图标 doesn't perfectly 匹配 the shape that vanilla info displays use
		public override string HoverTexture => Texture + "_Hover";

		// This dictates whether or not this info 显示 应该 active
		public override bool Active() {
			return Main.LocalPlayer.GetModPlayer<ExampleInfoDisplayPlayer>().showMinionCount;
		}

		// 在这里 we can change the 值 that 将 displayed 在 game
		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor) {
			// Counting how m任何 minions we have
			// 这是 the 值 that will show up when viewing this 显示 in normal play, 右 next 到 图标
			int minionCount = 0;
			foreach (var proj in Main.ActiveProjectiles) {
				if (proj.minion && proj.owner == Main.myPlayer) {
					minionCount++;
				}
			}

			bool noInfo = minionCount == 0;
			if (noInfo) {
				// 如果 "No minions" 将 displayed, grey out the 文本 颜色, 类似于 DPS 仪表 or Radar
				displayColor = InactiveInfoTextColor;
			}
			else if (minionCount < Main.LocalPlayer.maxMinions) {
				// This red 颜色 serves as a 警告 th在 玩家 has not summoned all their minions.
				displayColor = RedInfoTextColor;
			}
			/* 
			else if (minionCount == Main.LocalPlayer.maxMinions) {
				// 金币 文本 颜色 used for 金币 critters by the Lifeform Analyzer is easily accessible 如果需要
				displayColor = GoldInfoTextColor;
				displayShadowColor = GoldInfoTextShadowColor;
			}
			*/

			return !noInfo ? $"{minionCount} minions" : "No minions";
		}
	}

}
