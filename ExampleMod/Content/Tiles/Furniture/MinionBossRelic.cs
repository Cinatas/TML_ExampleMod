using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Content.Tiles.Furniture
{
	// 常见 code for a Master 模式 Boss relic
	// Supports optional 项.placeStyle handling if you wish to add more relics but use the same 图格 类型 (then it 将 wise to 名称 this 类 something more generic like BossRelic)
	// 如果 you 想要 add more relics but don't 想要 use the 项.placeStyle approach, see the inheritance example 在 底部 的 文件
	public class MinionBossRelic : ModTile
	{
		public const int FrameWidth = 18 * 3;
		public const int FrameHeight = 18 * 4;
		public const int HorizontalFrames = 1;
		public const int VerticalFrames = 1; // 可选: Increase this 数字 to 匹配 the amount of relics you have on your extra sheet, if you choose to use the 项.placeStyle approach

		public Asset<Texture2D> RelicTexture;

		// Every relic has its own extra floating part, 应该 50x50. Optional: Expand this sheet if you 想要 add more, stacked vertically
		// 如果 you do not use the 项.placeStyle approach, and you extend from this 类, you can override this to 点 to a different 纹理
		public virtual string RelicTextureName => "ExampleMod/Content/Tiles/Furniture/MinionBossRelic";

		// All relics use the same pedestal 纹理, this one is copied from vanilla
		public override string Texture => "ExampleMod/Content/Tiles/Furniture/RelicPedestal";

		public override void Load() {
			// 缓存 the extra 纹理 displayed 在 pedestal
			RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);
		}

		public override void SetStaticDefaults() {
			Main.tileShine[Type] = 400; // Responsible for golden particles
			Main.tileFrameImportant[Type] = true; // Any multitile requires this
			TileID.Sets.InteractibleByNPCs[Type] = true; // Town NPCs will palm their hand at this 图格

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4); // Relics are 3x4
			TileObjectData.newTile.LavaDeath = false; // Does not 中断 when lava touches it
			TileObjectData.newTile.DrawYOffset = 2; // So the 图格 sinks in到 ground
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft; // 玩家 faces 到 左
			TileObjectData.newTile.StyleHorizontal = false; // Based on how the alternate sprites are positioned 在 精灵 (默认情况下, 真)

			// This controls how styles are laid out 在 纹理 文件. This 图格 is special in that all styles will use the same 纹理 section to draw the pedestal.
			TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.styleLineSkipVisualOverride = 0; // This forces the 图格 preview to draw as if drawing the 1st style.

			// 注册 an alternate 图格 数据 with flipped 方向
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile); // 复制 每个thing from above, saves us some code
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight; // 玩家 faces 到 右
			TileObjectData.addAlternate(1);

			// 注册 the 图格 数据 itself
			TileObjectData.addTile(Type);

			// 注册 地图 名称 and 颜色
			// "MapObject.Relic" refers 到 翻译 键 对于 vanilla "Relic" 文本
			AddMapEntry(new Color(233, 207, 94), Language.GetText("MapObject.Relic"));
		}

		public override bool CreateDust(int i, int j, ref int type) {
			return false;
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
			// This forces the 图格 to draw the pedestal even if the placeStyle differs. 
			tileFrameX %= FrameWidth; // Clamps the frameX
			tileFrameY %= FrameHeight * 2; // Clamps the frameY (two horizontally aligned place styles, 因此 * 2)
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
			// Since this 图格 does not have the hovering part on its sheet, we 必须 animate it ourselves
			// Therefore we register the 顶部-左 的 图格 as a "special 点"
			// 这允许 us to draw things in SpecialDraw
			if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0) {
				Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
			}
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) {
			// 这是 lighting-模式 specific, always include this if you draw tiles manually
			Vector2 offScreen = new Vector2(Main.offScreenRange);
			if (Main.drawToScreen) {
				offScreen = Vector2.Zero;
			}

			// Take the 图格, check if it actually exists
			Point p = new Point(i, j);
			Tile tile = Main.tile[p.X, p.Y];
			if (tile == null || !tile.HasTile) {
				return;
			}

			// 获取 the initial draw parameters
			Texture2D texture = RelicTexture.Value;

			int frameY = tile.TileFrameX / FrameWidth; // Picks the 帧 在 sheet based 在 placeStyle 的 项
			Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, frameY);

			Vector2 origin = frame.Size() / 2f;
			Vector2 worldPos = p.ToWorldCoordinates(24f, 64f);

			Color color = Lighting.GetColor(p.X, p.Y);

			bool direction = tile.TileFrameY / FrameHeight != 0; // This is related 到 alternate 图格 数据 we registered before
			SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			// Some math magic to make it smoothly 移动 up and down over 时间
			const float TwoPi = (float)Math.PI * 2f;
			float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
			Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, offset * 4f);

			// 绘制 the main 纹理
			spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

			// 绘制 the periodic glow 效果
			float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
			Color effectColor = color;
			effectColor.A = 0;
			effectColor = effectColor * 0.1f * scale;
			for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI)) {
				spriteBatch.Draw(texture, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
			}
		}
	}

	// 如果 you 想要 make more relics but do not use the 项.placeStyle approach, you can use inheritance to avoid using duplicate code:
	// Your 图格 code would then inherit 从 MinionBossRelic 类 (which you should make abstract) and should look like this:
	/*
	public class MyBossRelic : MinionBossRelic
	{
		public override string RelicTextureName => "ExampleMod/Content/Tiles/Furniture/MyBossRelic";

		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
		}
	}
	*/

	// Your 项 code would then just use the MyBossRelic 图格 类型, and keep placeStyle on 0
	// textures for MyBossRelic 项/图格 必须 be supplied separately
}
