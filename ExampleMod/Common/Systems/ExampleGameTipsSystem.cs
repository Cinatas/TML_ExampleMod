using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace ExampleMod.Common.Systems
{
	/// <summary>
	/// This small ModSystem shows off the <seealso cref="ModSystem.ModifyGameTipVisibility"/> hook, which allows you to modify
	/// the tips/hints that show up during loading screens.
	/// </summary>
	public class ExampleGameTipsSystem : ModSystem
	{

		public override void ModifyGameTipVisibility(IReadOnlyList<GameTipData> gameTips) {
			// 如果你想添加自己的提示，则必须将它们放在本地化文件中。查看
			// Localization/en-US.hjson 文件中的 GameTips 键以了解功能。

			// What if we want to modify Vanilla tips? There is a GameTipID built into tModLoader that should make
			// 禁用某些提示。
			// 例如，让我们关闭血月和日食提示！
			gameTips[GameTipID.BloodMoonZombieDoorOpening].Hide();
			gameTips[GameTipID.SolarEclipseCreepyMonsters].Hide();

			// Now, say you want to modify OTHER mod's tips? You can do that too! Make sure you use the 右 mod and 键 名称.
			GameTipData disabledTip = gameTips.FirstOrDefault(tip => tip.FullName == "ExampleMod/DisabledExampleTip");
			// 可选ly, if you want to be a bit more specific 与 提示 名称 and mod 名称, you can also do that 与 Mod and 名称 properties, like so:
			// GameTipData disabledTip = gameTips.FirstOrDefault(提示 => 提示.Mod is Mod { 名称: "ExampleMod" } && 提示.名称 == "DisabledExampleTip");

			// If you haven't seen 空 propagation before, in short, the question mark checks if the 值 is 空, and if it is,
			// nothing happens and no 错误 is thrown; but if it isn't 空, call the 方法 as usual!
			disabledTip?.Hide();
		}
	}
}
