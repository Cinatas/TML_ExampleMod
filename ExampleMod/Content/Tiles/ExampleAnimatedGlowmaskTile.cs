using ExampleMod.Content.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Content.Tiles
{
	public class ExampleAnimatedGlowmaskTile : ModTile
	{
		private Asset<Texture2D> glowTexture;

		// 如果 you 想要 know more about tiles, please follow this link
		// https://github.com/tModLoader/tModLoader/wiki/Basic-图格
		public override void SetStaticDefaults() {
			// This changes a Framed 图格 to a FrameImportant 图格
			// 对于 modders, just remember to set this to 真 when you make a 图格 that uses a TileObjectData
			// Or basically all tiles that aren't like dirt, ores, or other basic building tiles
			Main.tileFrameImportant[Type] = true;
			// 使用 this to utilize an existing template
			// names of styles are self explanatory usually (you can see all existing templates 在 link mentioned earlier)
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			// 之前 adding the new 图格 you can make some changes to newTile like 高度, 原点 and etc.
			// Changing the 高度 because the template is for 1x2 not 1x3
			TileObjectData.newTile.Height = 3;
			// 修改 which part 的 图格 is centered 在 鼠标, in 图格 coordinates, 从 顶部 右 corner
			TileObjectData.newTile.Origin = new Point16(1, 2);
			// 设置ting the 高度 的 tiles individually for each
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
			// 最后 adding newTile
			TileObjectData.addTile(Type);

			// 添加MapEntry is for 设置 the 颜色 and optional 文本 associated 与 图格 when viewed 在 地图
			AddMapEntry(new Color(75, 139, 166));

			// 高度 of a 分组 of 动画 frames for this 图格
			// 默认s to 0, which disables animations
			AnimationFrameHeight = 56;

			glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow");
		}

		public override void AnimateTile(ref int frame, ref int frameCounter) {
			// 我们 can change frames manually, but since we are just simulating a different 图格, we can just use the same 值
			frame = Main.tileFrame[TileID.LunarMonolith];
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
			Tile tile = Main.tile[i, j];

			// 如果 you are using ModTile.SpecialDraw or PostDraw or PreDraw, use this snippet and add zero to all calls to spriteBatch.Draw
			// reason for this is to accommodate the shift in drawing coordinates that occurs when using the different Lighting 模式
			// Press Shift+F9 to change lighting modes quickly to 验证 your code works for all lighting modes
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

			// Because 高度 of third 图格 is different we change it
			int height = tile.TileFrameY % AnimationFrameHeight == 36 ? 18 : 16;

			// 偏移 along the Y axis depending 在 current 帧
			int frameYOffset = Main.tileFrame[Type] * AnimationFrameHeight;

			// 首先ly we draw the original 纹理 然后 glow mask 纹理
			spriteBatch.Draw(
				TextureAssets.Tile[Type].Value,
				new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
				new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, height),
				Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);
			// 确保 to draw with 颜色.White or 至少 a 颜色 即 fully opaque
			// Achieve opaqueness by increasing the alpha 通道 closer to 255. (lowering closer to 0 will achieve transparency)
			spriteBatch.Draw(
				glowTexture.Value,
				new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
				new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, height),
				Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			// 返回 假 to 停止 vanilla draw
			return false;
		}
	}

	internal class ExampleAnimatedGlowmaskTileItem : ModItem
	{
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.VoidMonolith);
			Item.createTile = ModContent.TileType<ExampleAnimatedGlowmaskTile>();
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
