using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace ExampleMod.Content.EmoteBubbles
{
	// This abstract 类 is used for town NPC emotes quick setup.
	public abstract class ModTownEmote : ModEmoteBubble
	{
		// Redirecting 纹理 路径.
		public override string Texture => "ExampleMod/Content/EmoteBubbles/NPCEmotes";

		public override void SetStaticDefaults() {
			// 添加 NPC emotes to "Town" category.
			AddToCategory(EmoteID.Category.Town);
		}

		/// <summary>
		/// Which 行 的 精灵 sheet is this NPC emote in?
		/// This is used to 帮助 get the correct 帧 rectangle for different emotes.
		/// </summary>
		public virtual int Row => 0;

		// 你 should decide the 帧 rectangle yourself by these two methods.
		public override Rectangle? GetFrame() {
			return new Rectangle(EmoteBubble.frame * 34, 28 * Row, 34, 28);
		}

		// Do note that you should never use EmoteBubble 实例 as the GetFrame() 方法 above
		// in "Emote 菜单 Methods" (methods with -InEmoteMenu 后缀).
		// Because in that case the 值 of EmoteBubble is always 空.
		public override Rectangle? GetFrameInEmoteMenu(int frame, int frameCounter) {
			return new Rectangle(frame * 34, 28 * Row, 34, 28);
		}
	}

	// 这是 a showcase of using the same 纹理 for different emotes.
	// 命令 names 的se classes are defined using .hjson files 在 Localization/ 文件夹.
	public class ExamplePersonEmote : ModTownEmote
	{
		public override void OnSpawn() {
			// This code makes the emote remain longer than vanilla emotes.
			EmoteBubble.lifeTime *= 2;
			EmoteBubble.lifeTimeStart *= 2;
		}

		public override int Row => 0;
	}

	public class ExampleTravellingMerchantEmote : ModTownEmote
	{
		public override int Row => 1;
	}

	public class ExampleBoneMerchantEmote : ModTownEmote
	{
		public override int Row => 2;
	}
}
