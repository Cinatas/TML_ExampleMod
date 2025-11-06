using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace ExampleMod.Content.EmoteBubbles
{
	// This emote 将 randomly displayed when using ExamplePickaxe
	// 参见 Content/Items/Tools/ExamplePickaxe.cs for letting the player use this emote
	public class ExamplePickaxeEmote : ModEmoteBubble
	{
		public override void SetStaticDefaults() {
			AddToCategory(EmoteID.Category.Items);
		}
	}
}
