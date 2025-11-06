using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Tiles
{
	public class ExamplePalmTree : ModPalmTree
	{
		private Asset<Texture2D> texture;
		private Asset<Texture2D> oasisTopsTexture;
		private Asset<Texture2D> topsTexture;

		// 这是 a blind copy-paste from Vanilla's PurityPalmTree settings.
		//TODO: This needs some explanations
		public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings {
			UseSpecialGroups = true,
			SpecialGroupMinimalHueValue = 11f / 72f,
			SpecialGroupMaximumHueValue = 0.25f,
			SpecialGroupMinimumSaturationValue = 0.88f,
			SpecialGroupMaximumSaturationValue = 1f
		};

		public override void SetStaticDefaults() {
			// 使 Example Palm Tree grow on Gold Ore
			GrowsOnTileId = new int[1] { TileID.Gold };
			texture = ModContent.Request<Texture2D>("ExampleMod/Content/Tiles/Plants/ExamplePalmTree");
			oasisTopsTexture = ModContent.Request<Texture2D>("ExampleMod/Content/Tiles/Plants/ExamplePalmOasisTree_Tops");
			topsTexture = ModContent.Request<Texture2D>("ExampleMod/Content/Tiles/Plants/ExamplePalmTree_Tops");
		}

		// 这是 the primary texture 对于 trunk. Branches and foliage use different settings.
		// first row 将 the Ocean textures, the second row 将 Oasis Textures.
		public override Asset<Texture2D> GetTexture() => texture;

		public override int SaplingGrowthType(ref int style) {
			style = 1;
			return ModContent.TileType<Plants.ExampleSapling>();
		}

		// Palm Trees come in an Oasis variant. The Top Textures for it:
		public override Asset<Texture2D> GetOasisTopTextures() => oasisTopsTexture;

		// Palm Trees come in a Beach variant. The Top Textures for it:
		public override Asset<Texture2D> GetTopTextures() => topsTexture;

		public override int DropWood() {
			return ModContent.ItemType<Items.Placeable.ExampleOre>();
		}
	}
}