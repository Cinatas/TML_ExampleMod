using ExampleMod.Common.Players;
using ExampleMod.Content.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content
{
	/// <summary>
	/// InfoDisplay 即 coupled with <seealso cref="ExampleInfoAccessory"/> and <seealso cref="ExampleInfoDisplayPlayer"/> to show
	/// off how to add a new info accessory (例如 a Radar, Lifeform Analyzer, etc.)
	/// </summary>
	public class ExampleInfoDisplay : InfoDisplay
	{
		public static Color RedInfoTextColor => new(255, 19, 19, Main.mouseTextColor);

		// 默认情况下, the vanilla circular outline texture 将 used. 
		// This info display has a square icon instead of a circular one, so we need to use a custom outline texture instead 的 vanilla outline texture.
		// 你 will only need to use a custom hover texture if your info display icon doesn't perfectly match the shape that vanilla info displays use
		public override string HoverTexture => Texture + "_Hover";

		// This dictates whether or not this info display 应该 active
		public override bool Active() {
			return Main.LocalPlayer.GetModPlayer<ExampleInfoDisplayPlayer>().showMinionCount;
		}

		// 在这里 we can change the value that 将 displayed 在 game
		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor) {
			// Counting how many minions we have
			// 这是 the value that will show up when viewing this display in normal play, right next 到 icon
			int minionCount = 0;
			foreach (var proj in Main.ActiveProjectiles) {
				if (proj.minion && proj.owner == Main.myPlayer) {
					minionCount++;
				}
			}

			bool noInfo = minionCount == 0;
			if (noInfo) {
				// 如果 "No minions" 将 displayed, grey out the text color, similar to DPS Meter or Radar
				displayColor = InactiveInfoTextColor;
			}
			else if (minionCount < Main.LocalPlayer.maxMinions) {
				// This red color serves as a warning th在 player has not summoned all their minions.
				displayColor = RedInfoTextColor;
			}
			/* 
			else if (minionCount == Main.LocalPlayer.maxMinions) {
				// gold text color used for gold critters by the Lifeform Analyzer is easily accessible 如果需要
				displayColor = GoldInfoTextColor;
				displayShadowColor = GoldInfoTextShadowColor;
			}
			*/

			return !noInfo ? $"{minionCount} minions" : "No minions";
		}
	}

}
