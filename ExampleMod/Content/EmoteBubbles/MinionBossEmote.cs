using ExampleMod.Common.Systems;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace ExampleMod.Content.EmoteBubbles
{
	public class MinionBossEmote : ModEmoteBubble
	{
		public override void SetStaticDefaults() {
			// 默认 emote 命令 名称 将 a lowercase 版本 的 classname to 匹配 other vanilla commands.
			// This 可以 changed 在 localization files.

			// 添加 the emote to "bosses" category
			AddToCategory(EmoteID.Category.Dangers);
		}

		public override bool IsUnlocked() {
			// This emote only shows when 仆从 Boss is downed, just as vanilla do.
			return DownedBossSystem.downedMinionBoss;
		}
	}
}
