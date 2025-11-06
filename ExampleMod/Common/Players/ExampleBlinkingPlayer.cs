using Terraria.GameContent;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	// 展示在特定条件下修改眼睛状态和帧。
	public class ExampleBlinkingPlayer : ModPlayer
	{
		public override void PostUpdate() {
			// 如果玩家在水中并且处于正常眨眼状态，则将眼睛状态设置为我们的自定义眼睛状态。
			if (Player.eyeHelper.CurrentEyeState == PlayerEyeHelper.EyeState.NormalBlinking && Player.wet) {
				Player.eyeHelper.CurrentEyeFrame = PlayerEyeHelper.EyeFrame.EyeClosed;
				Player.eyeHelper.TimeInState = 0;
			}

			// 用我们自己的覆盖原版失明眼睛状态。
			// It's enough to check for `Player.blind`, but that way it wouldn't replace `IsBlind` eye state set by other mods.
			// Decide yourself whatever option fits your needs.
			if (Player.eyeHelper.CurrentEyeState == PlayerEyeHelper.EyeState.IsBlind) {
				// Close players eyes for 115 ticks out of 120 ticks.
				// The remaining 5 ticks player will have half closed eyes.
				if ((Player.eyeHelper.TimeInState % 120 - 115) < 0) {
					Player.eyeHelper.CurrentEyeFrame = PlayerEyeHelper.EyeFrame.EyeClosed;
				}
				else {
					Player.eyeHelper.CurrentEyeFrame = PlayerEyeHelper.EyeFrame.EyeHalfClosed;
				}
			}
		}
	}
}
