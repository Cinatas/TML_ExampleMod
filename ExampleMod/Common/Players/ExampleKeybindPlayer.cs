using ExampleMod.Common.Systems;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	// 有关按键绑定注册，请参阅 Common/Systems/KeybindSystem。
	public class ExampleKeybindPlayer : ModPlayer
	{
		private int LearningExampleKeybindHeldTimer;
		private int LearningExampleKeybindDoubleTapTimer;

		public override void ProcessTriggers(TriggersSet triggersSet) {
			// 使用按键绑定的最常见方法是使用 JustPressed 在按下按键绑定时运行代码
			if (KeybindSystem.RandomBuffKeybind.JustPressed) {
				int buff = Main.rand.Next(BuffID.Count);
				Player.AddBuff(buff, 600);
				Main.NewText($"ExampleMod's ModKeybind was just pressed. The {Lang.GetBuffName(buff)} buff was given 到 player.");
			}

			// 这些示例展示了按键绑定的其他潜在行为，例如双击和按住。
			
			// We can use Current and a 计时器 to run code after the keybind has been held for some 时间
			if (KeybindSystem.LearningExampleKeybind.Current) {
				LearningExampleKeybindHeldTimer++;
				if (LearningExampleKeybindHeldTimer == 30) {
					Main.NewText("LearningExampleKeybind held for half a second");
				}
			}
			else {
				LearningExampleKeybindHeldTimer = 0;
			}

			// We can use JustPressed and a 计时器 to implement a double tap behavior 以及.
			LearningExampleKeybindDoubleTapTimer = Math.Max(0, LearningExampleKeybindDoubleTapTimer - 1);
			if (KeybindSystem.LearningExampleKeybind.JustPressed) {
				if (LearningExampleKeybindDoubleTapTimer > 0) {
					Main.NewText("LearningExampleKeybind double tapped within a quarter of a a second");
				}
				else {
					// On 1st press, set 计时器 for 15, if a 2nd press happens before it reaches 0, it 将 a double tap.
					LearningExampleKeybindDoubleTapTimer = 15;
				}
			}
		}
	}
}
