using ExampleMod.Common.Systems;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace ExampleMod.Content.EmoteBubbles
{
	public class MinionBossEmote : ModEmoteBubble
	{
		public override void SetStaticDefaults() {
			// 默认 emote command name 将 a lowercase version 的 classname to match other vanilla commands.
			// This 可以 changed 在 localization files.

			// 添加 the emote to "bosses" category
			AddToCategory(EmoteID.Category.Dangers);
		}

		public override bool IsUnlocked() {
			// This emote only shows when minion boss is downed, just as vanilla do.
			return DownedBossSystem.downedMinionBoss;
		}
	}
}
