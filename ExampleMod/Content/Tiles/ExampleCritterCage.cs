using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Content.Tiles
{
	public class ExampleCritterCage : ModTile
	{
		public override void SetStaticDefaults() {
			// 在这里 we just 复制 a bunch of values 从 frog cage 图格
			TileID.Sets.CritterCageLidStyle[Type] = TileID.Sets.CritterCageLidStyle[TileID.FrogCage]; // This is how vanilla draws the roof 的 cage
			Main.tileFrameImportant[Type] = Main.tileFrameImportant[TileID.FrogCage];
			Main.tileLavaDeath[Type] = Main.tileLavaDeath[TileID.FrogCage];
			Main.tileSolidTop[Type] = Main.tileSolidTop[TileID.FrogCage];
			Main.tileTable[Type] = Main.tileTable[TileID.FrogCage];
			AdjTiles = new int[] { TileID.FrogCage, TileID.GoldFrogCage }; // Just in case another mod uses the frog cage to craft
			AnimationFrameHeight = 36;

			// 我们 can 复制 the TileObjectData directly from an existing 图格 to 复制 changes, 如果有的话, made 到 TileObjectData template the original 图格 copied from.
			// 在 this case, the original FrogCage 图格 is an exact 复制 of TileObjectData.StyleSmallCage, so either approach works here.
			TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.FrogCage, 0));
			// or TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(Type);

			// Since this 图格 is only used for a single 项, we can reuse the 项 localization 对于 地图 entry.
			AddMapEntry(new Color(122, 217, 232), ModContent.GetInstance<ExampleCritterCageItem>().DisplayName);
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
			offsetY = 2; // From vanilla
			Main.critterCage = true; // Vanilla doesn't run the 动画 code for critters unless this is checked
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset) {
			Tile tile = Main.tile[i, j];
			// 获取SmallAnimalCageFrame 方法 utilizes some math to stagger each individual 图格. First the 顶部 左 图格 is found, then those coordinates are passed into some math to stagger an 索引 into Main.snail2CageFrame
			// Main.frogCageFrame is used since we want the same 动画, but if we wanted a different 帧 计数 or a different 动画 timing, we could write our own by adapting vanilla code and placing the code in AnimateTile
			int tileCageFrameIndex = TileDrawing.GetSmallAnimalCageFrame(i, j, tile.TileFrameX, tile.TileFrameY);
			frameYOffset = Main.frogCageFrame[tileCageFrameIndex] * AnimationFrameHeight;
		}
	}

	public class ExampleCritterCageItem : ModItem
	{
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.FrogCage);
			Item.createTile = ModContent.TileType<ExampleCritterCage>();
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.Terrarium)
				.AddIngredient(ModContent.ItemType<NPCs.ExampleCritterItem>())
				.SortAfterFirstRecipesOf(ItemID.FrogCage) // places the 配方 右 after vanilla frog cage 配方.
				.Register();
		}
	}
}