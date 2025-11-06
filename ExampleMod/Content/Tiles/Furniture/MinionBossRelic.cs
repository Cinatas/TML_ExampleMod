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
	// 常见 code for a Master Mode boss relic
	// Supports optional Item.placeStyle handling if you wish to add more relics but use the same tile type (then it 将 wise to name this class something more generic like BossRelic)
	// 如果 you want to add more relics but don't want to use the Item.placeStyle approach, see the inheritance example 在 bottom 的 file
	public class MinionBossRelic : ModTile
	{
		public const int FrameWidth = 18 * 3;
		public const int FrameHeight = 18 * 4;
		public const int HorizontalFrames = 1;
		public const int VerticalFrames = 1; // 可选: Increase this number to match the amount of relics you have on your extra sheet, if you choose to use the Item.placeStyle approach

		public Asset<Texture2D> RelicTexture;

		// Every relic has its own extra floating part, 应该 50x50. Optional: Expand this sheet if you want to add more, stacked vertically
		// 如果 you do not use the Item.placeStyle approach, and you extend from this class, you can override this to point to a different texture
		public virtual string RelicTextureName => "ExampleMod/Content/Tiles/Furniture/MinionBossRelic";

		// All relics use the same pedestal texture, this one is copied from vanilla
		public override string Texture => "ExampleMod/Content/Tiles/Furniture/RelicPedestal";

		public override void Load() {
			// Cache the extra texture displayed 在 pedestal
			RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);
		}

		public override void SetStaticDefaults() {
			Main.tileShine[Type] = 400; // Responsible for golden particles
			Main.tileFrameImportant[Type] = true; // Any multitile requires this
			TileID.Sets.InteractibleByNPCs[Type] = true; // Town NPCs will palm their hand at this tile

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4); // Relics are 3x4
			TileObjectData.newTile.LavaDeath = false; // Does not break when lava touches it
			TileObjectData.newTile.DrawYOffset = 2; // So the tile sinks in到 ground
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft; // Player faces 到 left
			TileObjectData.newTile.StyleHorizontal = false; // Based on how the alternate sprites are positioned 在 sprite (默认情况下, true)

			// This controls how styles are laid out 在 texture file. This tile is special in that all styles will use the same texture section to draw the pedestal.
			TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.styleLineSkipVisualOverride = 0; // This forces the tile preview to draw as if drawing the 1st style.

			// 注册 an alternate tile data with flipped direction
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile); // Copy everything from above, saves us some code
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight; // Player faces 到 right
			TileObjectData.addAlternate(1);

			// 注册 the tile data itself
			TileObjectData.addTile(Type);

			// 注册 map name and color
			// "MapObject.Relic" refers 到 translation key 对于 vanilla "Relic" text
			AddMapEntry(new Color(233, 207, 94), Language.GetText("MapObject.Relic"));
		}

		public override bool CreateDust(int i, int j, ref int type) {
			return false;
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
			// This forces the tile to draw the pedestal even if the placeStyle differs. 
			tileFrameX %= FrameWidth; // Clamps the frameX
			tileFrameY %= FrameHeight * 2; // Clamps the frameY (two horizontally aligned place styles, hence * 2)
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
			// Since this tile does not have the hovering part on its sheet, we have to animate it ourselves
			// Therefore we register the top-left 的 tile as a "special point"
			// This allows us to draw things in SpecialDraw
			if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0) {
				Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
			}
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) {
			// 这是 lighting-mode specific, always include this if you draw tiles manually
			Vector2 offScreen = new Vector2(Main.offScreenRange);
			if (Main.drawToScreen) {
				offScreen = Vector2.Zero;
			}

			// Take the tile, check if it actually exists
			Point p = new Point(i, j);
			Tile tile = Main.tile[p.X, p.Y];
			if (tile == null || !tile.HasTile) {
				return;
			}

			// 获取 the initial draw parameters
			Texture2D texture = RelicTexture.Value;

			int frameY = tile.TileFrameX / FrameWidth; // Picks the frame 在 sheet based 在 placeStyle 的 item
			Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, frameY);

			Vector2 origin = frame.Size() / 2f;
			Vector2 worldPos = p.ToWorldCoordinates(24f, 64f);

			Color color = Lighting.GetColor(p.X, p.Y);

			bool direction = tile.TileFrameY / FrameHeight != 0; // This is related 到 alternate tile data we registered before
			SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			// Some math magic to make it smoothly move up and down over time
			const float TwoPi = (float)Math.PI * 2f;
			float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
			Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, offset * 4f);

			// 绘制 the main texture
			spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

			// 绘制 the periodic glow effect
			float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
			Color effectColor = color;
			effectColor.A = 0;
			effectColor = effectColor * 0.1f * scale;
			for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI)) {
				spriteBatch.Draw(texture, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
			}
		}
	}

	// 如果 you want to make more relics but do not use the Item.placeStyle approach, you can use inheritance to avoid using duplicate code:
	// Your tile code would then inherit 从 MinionBossRelic class (which you should make abstract) and should look like this:
	/*
	public class MyBossRelic : MinionBossRelic
	{
		public override string RelicTextureName => "ExampleMod/Content/Tiles/Furniture/MyBossRelic";

		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
		}
	}
	*/

	// Your item code would then just use the MyBossRelic tile type, and keep placeStyle on 0
	// textures for MyBossRelic item/tile have to be supplied separately
}
