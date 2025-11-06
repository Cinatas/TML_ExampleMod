using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	// This 增益 has an extra 动画 spritesheet, and also showcases PreDraw specifically.
	// (We keep the autoloaded 纹理 as one 帧 in case other mods 需要 access the 增益 精灵 directly and aren't aware of it having special draw code).
	public class AnimatedBuff : ModBuff
	{
		// Some constants we define to make our life easier.
		public static readonly int FrameCount = 4; // Amount of frames we have on our 动画 spritesheet.
		public static readonly int AnimationSpeed = 60; // In ticks.
		public static readonly string AnimationSheetPath = "ExampleMod/Content/Buffs/AnimatedBuff_Animation";

		public static readonly int DamageBonus = 10;

		private Asset<Texture2D> animatedTexture;

		public override LocalizedText Description => base.Description.WithFormatArgs(DamageBonus);

		public override void SetStaticDefaults() {
			animatedTexture = ModContent.Request<Texture2D>(AnimationSheetPath);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams) {
			// 你 can use this hook to make something special happen when the 增益 图标 is drawn (例如 reposition it, pick a different 纹理, etc.).

			// 我们 draw our special 纹理 here with a specific 动画.

			// 使用 our 动画 spritesheet.
			Texture2D ourTexture = animatedTexture.Value;
			// Choose the 帧 to 显示, here 基于 constants and 游戏's tick 计数.
			Rectangle ourSourceRectangle = ourTexture.Frame(verticalFrames: FrameCount, frameY: (int)Main.GameUpdateCount / AnimationSpeed % FrameCount);

			// Other stuff you can do in this hook
			/*
			// 在这里 we make the 图标 have a lime green tint.
			drawParams.drawColor = Color.LimeGreen * Main.buffAlpha[buffIndex];
			*/

			// Be aware 的 fact that drawParams.mouseRectangle exists: it defaults 到 大小 的 autoloaded buffs' 精灵,
			// it handles mouseovering and clicking 在 增益 图标. Since our 帧 在 动画 is 32x32 (same as the autoloaded 精灵),
			// and we don't change drawParams.位置, we don't 必须 do 任何thing. If you 偏移 the 位置, or have a non-standard 大小, change it 相应地.

			// 我们 have two options here:
			// 选项 1 is the recommended one, as it requires less code.
			// 选项 2 allows you to customize drawing even more, but then you are on your own.

			// 对于 demonstration, 两者 options' codes are written down, but the latter is commented out using /* and */.

			// 选项 1 - Let 游戏 draw it for us. Therefore we 必须 assign our variables to drawParams:
			drawParams.Texture = ourTexture;
			drawParams.SourceRectangle = ourSourceRectangle;
			// 返回 真 to let 游戏 draw the 增益 图标.
			return true;

			/*
			// 选项 2 - Draw our 增益 manually:
			spriteBatch.Draw(ourTexture, drawParams.position, ourSourceRectangle, drawParams.drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			// 返回 假 to 防止 drawing the 图标, since we have already drawn it.
			return false;
			*/
		}

		public override void Update(Player player, ref int buffIndex) {
			// Increase all 伤害 by 10%
			player.GetDamage<GenericDamageClass>() += DamageBonus / 100f;
		}
	}
}
