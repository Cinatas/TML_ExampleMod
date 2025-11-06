using ExampleMod.Content.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Content.Tiles
{
	internal class ExampleAnimatedTile : ModTile
	{
		// 如果 you 想要 know more about tiles, please follow this link
		// https://github.com/tModLoader/tModLoader/wiki/Basic-图格
		public override void SetStaticDefaults() {
			// 如果 a 图格 is a light source
			Main.tileLighted[Type] = true;
			// This changes a Framed 图格 to a FrameImportant 图格
			// 对于 modders, just remember to set this to 真 when you make a 图格 that uses a TileObjectData
			// Or basically all tiles that aren't like dirt, ores, or other basic building tiles
			Main.tileFrameImportant[Type] = true;
			// 设置 to 真 if you'd like your 图格 to die if hit by lava
			Main.tileLavaDeath[Type] = true;
			// 使用 this to utilize an existing template
			// names of styles are self explanatory usually (you can see all existing templates 在 link mentioned earlier)
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			// This last call adds a new 图格
			// 之前 that, you can make some changes to newTile like 高度, 原点 and etc.
			TileObjectData.addTile(Type);

			// 添加MapEntry is for 设置 the 颜色 and optional 文本 associated 与 图格 when viewed 在 地图
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(238, 145, 105), name);

			// 不能 use this since 纹理 is vertical
			// AnimationFrameHeight = 56;
		}

		// Our textures 动画 frames are arranged horizontally, 即n't typical, so here we specify animationFrameWidth which we use later in AnimateIndividualTile
		private readonly int animationFrameWidth = 18;

		// 此方法 allows you to determine how much light this 方块 emits
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
			r = 0.93f;
			g = 0.11f;
			b = 0.12f;
		}

		// 此方法 allows you to determine whether or not the 图格 will draw itself flipped 在 世界
		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) {
			// Flips the 精灵 if x coord is odd. Makes the 图格 more interesting
			if (i % 2 == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset) {
			// Tweak the 帧 drawn by x 位置 so tiles next to each other are off-同步 and look much more interesting
			int uniqueAnimationFrame = Main.tileFrame[Type] + i;
			if (i % 2 == 0)
				uniqueAnimationFrame += 3;
			if (i % 3 == 0)
				uniqueAnimationFrame += 3;
			if (i % 4 == 0)
				uniqueAnimationFrame += 3;
			uniqueAnimationFrame %= 6;

			// frameYOffset = modTile.AnimationFrameHeight * Main.tileFrame[类型] will already be set before this hook is called
			// But we have a horizontal animated 纹理, so we use frameXOffset 代替 frameYOffset
			frameXOffset = uniqueAnimationFrame * animationFrameWidth;
		}

		// 此方法 allows you to change the 声音 a 图格 makes when hit
		public override bool KillSound(int i, int j, bool fail) {
			// Play the glass shattering 声音 instead 的 normal digging 声音 if the 图格 is destroyed on this hit
			if (!fail) {
				SoundEngine.PlaySound(SoundID.Shatter, new Vector2(i, j).ToWorldCoordinates());
				return false;
			}
			return base.KillSound(i, j, fail);
		}

		// 待办事项： It's better to have an actual 类 for this example, 代替 comments

		// Below is an example completely manually drawing a 图格. It shows some interesting concepts that 可能 useful f或更多 advanced things
		/*public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
			// 代替 of SetSpriteEffects
			// Flips the 精灵 if x coord is odd. Makes the 图格 more interesting
			SpriteEffects effects = SpriteEffects.None;
			if (i % 2 == 1)
				effects = SpriteEffects.FlipHorizontally;

			// 代替 of AnimateIndividualTile
			// Tweak the 帧 drawn by x 位置 so tiles next to each other are off-同步 and look much more interesting
			int uniqueAnimationFrame = Main.tileFrame[Type] + i % 6;
			if (i % 2 == 0)
				uniqueAnimationFrame += 3;
			if (i % 3 == 0)
				uniqueAnimationFrame += 3;
			if (i % 4 == 0)
				uniqueAnimationFrame += 3;
			uniqueAnimationFrame %= 6;

			int frameXOffset = uniqueAnimationFrame * animationFrameWidth;


			Tile tile = Main.tile[i, j];
			Texture2D texture = TextureAssets.Tile[Type].Value;

			// 如果 you are using ModTile.SpecialDraw or PostDraw or PreDraw, use this snippet and add zero to all calls to spriteBatch.Draw
			// reason for this is to accommodate the shift in drawing coordinates that occurs when using the different Lighting 模式
			// Press Shift+F9 to change lighting modes quickly to 验证 your code works for all lighting modes
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

			Main.spriteBatch.Draw(
				texture,
				new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
				new Rectangle(tile.frameX + frameXOffset, tile.frameY, 16, 16),
				Lighting.GetColor(i, j), 0f, default, 1f, effects, 0f);

			return false; // 返回 假 to 停止 vanilla draw
		}*/

		public override void AnimateTile(ref int frame, ref int frameCounter) {
			/*
			// Spend 9 ticks on 每个 6 frames, looping
			frameCounter++;
			if (frameCounter >= 9) {
				frameCounter = 0;
				if (++frame >= 6) {
					frame = 0;
				}
			}

			// Or, more compactly:
			if (++frameCounter >= 9) {
				frameCounter = 0;
				frame = ++frame % 6;
			}*/

			// Above code works, but since we are just mimicking another 图格, we can just use the same 值
			frame = Main.tileFrame[TileID.FireflyinaBottle];
		}
	}

	internal class ExampleAnimatedTileItem : ModItem
	{
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.FireflyinaBottle);
			Item.createTile = ModContent.TileType<ExampleAnimatedTile>();
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
