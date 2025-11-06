using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Content.Tiles
{
	// This 图格 serves as a showcase for TileObjectData.
	// 在 particular, this contrived example shows how styles are laid out 在 spritesheet when multiple styles, multiple alternate placements, 随机 style 范围, animations, and toggle states are all desired.
	// 如果 you place this 图格, you'll noticed that it has both 左 and 右 variants depending 在 玩家 方向. You'll also notice th在re are 4 随机 style variations for 左 and 右. Once placed, the 图格 will animate through 3 frames of 动画. 右 clicking 在 图格 will change the 图格 to an "off" 状态, halting the 动画 and showing the 4th 帧 of 动画. There are 4 图格 styles contained in this example 以及.
	// StyleMultiplier section 的 图格 wiki 页面, https://github.com/tModLoader/tModLoader/wiki/Basic-图格#stylemultiplier, has a simpler visualization only showing alternate placements and 随机 style variations.
	// Please experiment by placing this 图格 using both the "TileObjectData Showcase Style 3 - ExampleBlock" 项 and one 的 other TileObjectData Showcase items. This 图格 anchors to specific tiles 到 左 and 右, you'll need to place this 图格 between pillars of those specific tiles. By doing this you 应该 able to visualize the full potential of TileObjectData.
	// Not many tiles will require such complicated 布局, but this serves as example of how each feature affects the resulting spritesheet.
	// Since this 图格 is "StyleHorizontal = 真", styles 在 spritesheet are positioned 左 to 右. Each alternate placement and
	// 随机 style are also placed in-line 与 styles. Toggled states and animations are placed vertically below their corresponding placement. 在 corresponding spritesheet, the styles, alternate placements, and 动画 frames are all labeled to make this 布局 clearer.
	// 之后 reaching the wrap 限制, subsequent styles are placed 在 next 行.
	public class TileObjectDataShowcase : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;

			// 首先 setup a basic 2x2 图格.
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinateHeights = [16, 16];
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinatePadding = 0; // This is used to keep the spritesheet legible for this example.

			// These define how multiple styles and alternate placements 将 located 在 spritesheet
			TileObjectData.newTile.StyleMultiplier = 8; // Each style will occupy 8 placement styles
			TileObjectData.newTile.RandomStyleRange = 4; // We have a 左 and 右 placement, each has 4 随机 varieties. Look for "Alt 0", "Alt 1", "Alt 2", and "Alt 3" 在 spritesheet.
			TileObjectData.newTile.StyleWrapLimit = 16; // We will wrap 到 next line 在 纹理 after 16 placement styles, or 2 styles.
			TileObjectData.newTile.StyleLineSkip = 4; // This gives extra lines 在 spritesheet for 动画 or 图格 states.

			// 在这里 we declare th在 图格 将 placeable when facing 左.
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			// 图格 only anchors between specific tiles 到 左 and 右. These are defined in each Subtile below.
			TileObjectData.newTile.AnchorLeft = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);

			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(PostPlaceMethod, -1, 0, true); // Just for fun.

			// Now we make a 复制 newTile to populate an alternate placement. This faces 右 instead of 左.
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(4); // This alternate starts at placement style 4 because the 左 alternate has 4 随机 placements. These alternate placements 将 "Alt 4", "Alt 5", "Alt 6", and "Alt 7" 在 spritesheet.

			// These additional alternates reuse the same placement styles 的 the normal placement and alternate placements above, but have a different 原点 to make placing the 图格 easier. The 图格 placement preview will seem to "snap" to valid locations. This is completely optional and serves as an example of how multiple alternates can share placement styles. 与se additional alternates, the 玩家 can positi在 图格 by the 底部 左 or 底部 右 corner 的 图格. Try it out for yourself in-game to see. This is similar to how doors 可以 placed by placing the 鼠标 in any 的 3 tiles of a doorway.
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 1);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.newAlternate.Origin = new Point16(1, 1);
			TileObjectData.addAlternate(4);

			// Next, we initialize subtiles. These are 图格 style specific 图格 properties.
			// Each 的se subtiles define their own AnchorAlternateTiles 数组 to showcase this capability, but subtiles are typically just used for water and lava behaviors. This 图格 anchors 到se specific tiles 到 左 and 右, meaning that pillars of those tiles are needed to place this 图格 in到 世界.
			// Look for "Sty 0", "Sty 1", and "Sty 2" 在 spritesheet.
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LinkedAlternates = true;
			TileObjectData.newSubTile.AnchorAlternateTiles = [TileID.Gold];
			TileObjectData.addSubTile(0);

			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LinkedAlternates = true;
			TileObjectData.newSubTile.AnchorAlternateTiles = [TileID.Silver];
			TileObjectData.addSubTile(1);

			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LinkedAlternates = true;
			TileObjectData.newSubTile.AnchorAlternateTiles = [TileID.Copper];
			TileObjectData.addSubTile(2);

			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LinkedAlternates = true;
			TileObjectData.newSubTile.AnchorAlternateTiles = [ModContent.TileType<ExampleBlock>()];
			TileObjectData.addSubTile(3);

			TileObjectData.addTile(Type);

			// 我们 can automatically set the 动画 帧 高度 from CoordinateFullHeight for any typical 图格 that uses the expected 布局.
			AnimationFrameHeight = TileObjectData.GetTileData(Type, 0).CoordinateFullHeight;
		}

		// 显示s various info about the 图格 placement in chat.
		private int PostPlaceMethod(int x, int y, int type, int style, int direction, int alternate) {
			// 注意 that alternate here is the alternate 索引, not the alternate placement style. We'll use some math to calculate the 随机 偏移 and placement style values
			var tileData = TileObjectData.GetTileData(type, style, alternate);

			int alternatePlacement = -1;
			int unused = -1;
			TileObjectData.GetTileInfo(Main.tile[x, y], ref unused, ref alternatePlacement);

			Main.NewText($"Style: {style}, Alternate Offset: {tileData.Style}, Random Offset: {alternatePlacement - tileData.Style}, Placement Style: {alternatePlacement}, Full Placement Style: {style * tileData.StyleMultiplier + alternatePlacement}, Direction: {direction}, Alternate Index: {alternate}, Origin: ({tileData.Origin.X}, {tileData.Origin.Y})");

			return 0;
		}

		// 当 this 图格 is 右 clicked, it changes to a new 状态 by changing TileFrameY. This "off" 状态 is the "Fra 3" sprites 在 spritesheet.
		public override bool RightClick(int i, int j) {
			SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));

			// This math finds the 顶部 左 corner 的 图格.
			Tile tile = Main.tile[i, j];
			int topX = i - (tile.TileFrameX % 256) % 32 / 16;
			int topY = j - (tile.TileFrameY % 128) % 32 / 16;

			// 96 is the Y 位置 的 "Fra 3" sprites 在 spritesheet. (32 * 3)
			short frameAdjustment = (short)(tile.TileFrameY % 128 >= 96 ? -96 : 96);

			for (int x = topX; x < topX + 2; x++) {
				for (int y = topY; y < topY + 2; y++) {
					Main.tile[x, y].TileFrameY += frameAdjustment;
				}
			}

			if (Main.netMode != NetmodeID.SinglePlayer) {
				NetMessage.SendTileSquare(-1, topX, topY, 2, 2);
			}

			return true;
		}

		public override void AnimateTile(ref int frame, ref int frameCounter) {
			// 循环 between frames 0, 1, and 2 every 16 ticks
			if (++frameCounter >= 16) {
				frameCounter = 0;
				frame = ++frame % 3;
			}
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset) {
			var tile = Main.tile[i, j];
			// 如果 the 图格 is "on", then the 图格 will animate between the "Fra 0", "Fra 1", and "Fra 2" sprites. This is already applied because we set AnimationFrameHeight in SetStaticDefaults and adjust 帧 in AnimateTile.

			// 如果 the 图格 is "off", however, then we set frameYOffset to 0 to 禁用 the automatic 动画 for this specific 图格. 96 is the Y 位置 的 "Fra 3" sprites 在 spritesheet. (32 * 3)
			if (tile.TileFrameY % 128 >= 96) {
				frameYOffset = 0;
			}
		}
	}

	// These items place the 4 styles of this showcase 图格. Experiment with placing these items to see how the 图格 works.
	public class TileObjectDataShowcaseStyle0 : ModItem
	{
		public override string Texture => $"ExampleMod/Content/Tiles/TileObjectDataShowcaseItemA";

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<TileObjectDataShowcase>(), 0);
		}
	}

	public class TileObjectDataShowcaseStyle1 : ModItem
	{
		public override string Texture => $"ExampleMod/Content/Tiles/TileObjectDataShowcaseItemA";

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<TileObjectDataShowcase>(), 1);
		}
	}

	public class TileObjectDataShowcaseStyle2 : ModItem
	{
		public override string Texture => $"ExampleMod/Content/Tiles/TileObjectDataShowcaseItemA";

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<TileObjectDataShowcase>(), 2);
		}
	}

	public class TileObjectDataShowcaseStyle3 : ModItem
	{
		public override string Texture => $"ExampleMod/Content/Tiles/TileObjectDataShowcaseItemB";

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<TileObjectDataShowcase>(), 3);
		}
	}
}
