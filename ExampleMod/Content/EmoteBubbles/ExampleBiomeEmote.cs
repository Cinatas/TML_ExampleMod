using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.EmoteBubbles
{
	// 这是 a showcase of drawing the emote bubble yourself.
	// It performs totally the same as vanilla.
	// 检查 Common/GlobalNPC/EmotePickerGlobalNPC.cs for adding this emote for all NPCs.
	public class ExampleBiomeEmote : ModEmoteBubble
	{
		public override void SetStaticDefaults() {
			// 添加 the emote to "biomes" category
			AddToCategory(EmoteID.Category.NatureAndWeather);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle frame, Vector2 origin, SpriteEffects spriteEffects) {
			// Extra_48 is the 纹理 of all vanilla emotes.
			Texture2D bubbleTexture = TextureAssets.Extra[ExtrasID.EmoteBubble].Value;
			// 这是 the 帧 rectangle 对于 bubble in emotes 纹理.
			Rectangle bubbleFrame = bubbleTexture.Frame(8, 39, EmoteBubble.IsFullyDisplayed ? 1 : 0);

			// 绘制 the bubble 背景.
			spriteBatch.Draw(bubbleTexture, position, bubbleFrame, Color.White, 0f, origin, 1f, spriteEffects, 0f);

			// 如果 the emote bubble isn't fully displayed (bubble pop-up 动画 is being displayed),
			// don't draw the emote content.
			if (!EmoteBubble.IsFullyDisplayed) {
				return false;
			}

			// 绘制 the emote.
			spriteBatch.Draw(texture, position, frame, Color.White, 0f, origin, 1f, spriteEffects, 0f);

			return false; // 停止 vanilla drawing code.
		}

		// 此方法 is for drawing emote 在 emotes 菜单.
		public override bool PreDrawInEmoteMenu(SpriteBatch spriteBatch, EmoteButton uiEmoteButton, Vector2 position, Rectangle frame, Vector2 origin) {
			// This 颜色 is used for 边框 that becomes yellow (or blue) when you 悬停 your cursor over it.
			Color borderColor = Color.Black;
			if (uiEmoteButton.Hovered) {
				borderColor = Main.OurFavoriteColor;
			}
			// 这是 the 帧 rectangle 对于 bubble in emotes 纹理.
			Rectangle bubbleFrame = uiEmoteButton.BubbleTexture.Frame(8, 39, 1, 0);

			// 绘制 everything
			spriteBatch.Draw(uiEmoteButton.BubbleTexture.Value, position, bubbleFrame, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(uiEmoteButton.EmoteTexture.Value, position, frame, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(uiEmoteButton.BorderTexture.Value, position - Vector2.One * 2f, null, borderColor, 0f, origin, 1f, SpriteEffects.None, 0f);

			return false;
		}
	}
}
