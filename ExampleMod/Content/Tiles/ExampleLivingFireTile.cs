using ExampleMod.Content.Dusts;
using ExampleMod.Content.Items.Placeable;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Tiles
{
	public class ExampleLivingFireTile : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileLighted[Type] = true; // This tells 游戏 that our 图格 produces light.

			// Normally, non-solid tiles can不 placed on other non-solid tiles. This set allows that.
			// This set includes Cobwebs, 硬币 Piles, Living Fire Blocks, Smoke Blocks, and Bubble Blocks.
			TileID.Sets.CanPlaceNextToNonSolidTile[Type] = true;

			DustType = ModContent.DustType<Sparkle>(); // 设置 the dust 类型.

			// 在这里 we set the 地图 颜色 到 same 颜色 as the light 颜色.
			// 我们 are accessing a 变量 that we defined inside 的 项 so we don't 必须 repeat entering the values.
			AddMapEntry(new Color(ExampleLivingFire.LightColor));

			// 有 4 frames of 动画 for our 纹理.
			// 纹理 360 pixels tall / 4 frames of 动画 = 90.
			AnimationFrameHeight = 90;
		}
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
			// 在这里 we set the strength 的 light th在 图格 produces.
			// 我们 are accessing a 变量 that we defined inside 的 项 so we don't 必须 repeat entering the values.
			r = ExampleLivingFire.LightColor.X;
			g = ExampleLivingFire.LightColor.Y;
			b = ExampleLivingFire.LightColor.Z;
		}
		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
			// Living Fire Blocks are drawn 2 pixels lower so th在y sink in到 图格 below it.
			offsetY = 2;
		}

		public override void AnimateTile(ref int frame, ref int frameCounter) {
			// 在这里 is where the tiles are animated.
			// Since we are just mimicking an existing 图格, 我们可以 just use the same 帧 值.
			frame = Main.tileFrame[TileID.LivingFire];

			/* This is how it 将 done manually, spending 5 ticks on each of 4 frames, looping.
			if (++frameCounter >= 5) {
				frameCounter = 0;
				frame = ++frame % 4;
			}
			*/
		}
	}
}