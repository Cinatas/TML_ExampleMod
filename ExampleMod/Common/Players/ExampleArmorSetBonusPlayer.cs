using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	// 此 ModPlayer 促进套装奖励效果。此示例展示了如何根据你希望玩家与套装奖励效果交互的方式使用 ArmorSetBonusActivated 或 ArmorSetBonusHeld。 
	public class ExampleArmorSetBonusPlayer : ModPlayer
	{
		public bool ExampleSetHood; // 指示 if the ExampleSet with ExampleHood is the active armor set.
		public int ShadowStyle = 0; // This is the shadow to use. Note that ExampleHood.ArmorSetShadows will only be called if the full armor set is visible.

		public override void ResetEffects() {
			ExampleSetHood = false;
		}

		public override void ArmorSetBonusActivated() {
			if (!ExampleSetHood) {
				return;
			}

			if (ShadowStyle == 3) {
				ShadowStyle = 0;
			}
			ShadowStyle = (ShadowStyle + 1) % 3;
			ShowMessageForShadowStyle();
		}

		public override void ArmorSetBonusHeld(int holdTime) {
			if (!ExampleSetHood) {
				return;
			}

			if (holdTime == 60) {
				ShadowStyle = ShadowStyle == 3 ? 0 : 3;
				ShowMessageForShadowStyle();
			}
		}

		private void ShowMessageForShadowStyle() {
			string styleName = ShadowStyle switch {
				1 => "armorEffectDrawShadow",
				2 => "armorEffectDrawOutlines",
				3 => "armorEffectDrawOutlinesForbidden",
				_ => "None",
			};
			Main.NewText("Current shadow style: " + styleName);
		}
	}
}
