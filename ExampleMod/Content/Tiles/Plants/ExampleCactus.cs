using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace ExampleMod.Content.Tiles
{
	public class ExampleCactus : ModCactus
	{
		private Asset<Texture2D> texture;
		private Asset<Texture2D> fruitTexture;

		public override void SetStaticDefaults() {
			// 使 Example Cactus grow on ExampleSand. You will 需要 use ExampleSolution to convert regular sand since ExampleCactus will not grow naturally yet.
			GrowsOnTileId = new int[1] { ModContent.TileType<ExampleSand>() };
			texture = ModContent.Request<Texture2D>("ExampleMod/Content/Tiles/Plants/ExampleCactus");
			fruitTexture = ModContent.Request<Texture2D>("ExampleMod/Content/Tiles/Plants/ExampleCactus_Fruit");
		}

		public override Asset<Texture2D> GetTexture() => texture;

		// This 将 where the Cactus Fruit 纹理 would go, if we had one.
		public override Asset<Texture2D> GetFruitTexture() => fruitTexture;
	}
}