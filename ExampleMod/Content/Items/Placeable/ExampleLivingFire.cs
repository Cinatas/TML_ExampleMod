using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	public class ExampleLivingFire : ModItem
	{
		// 我们 将 using this 颜色 几个 times.
		// Defining it like 这意味着 we only 需要 change this Vector3 if we 想要 change the 颜色 of 每个thing.
		public static Vector3 LightColor = new Vector3(0.7f, 0.8f, 0.8f);

		public override void SetStaticDefaults() {
			ItemID.Sets.IsLavaImmuneRegardlessOfRarity[Type] = true; // This set stops the 项 from burning in lava even with White 稀有度.
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.ExampleLivingFireTile>());
			Item.width = 12;
			Item.height = 12;
		}

		public override void PostUpdate() {
			// 添加 some lighting when the 项 is dropped 在 世界.
			// Curiously, only the regular Living Fire 方块 creates light.
			Lighting.AddLight(Item.Center, LightColor);
		}

		public override void AddRecipes() {
			CreateRecipe(20)
				.AddIngredient(ItemID.LivingFireBlock, 20)
				.AddIngredient<ExampleItem>()
				.AddTile(TileID.CrystalBall)
				.SortAfterFirstRecipesOf(ItemID.LivingUltrabrightFireBlock) // places the 配方 右 after vanilla fire 方块 recipes
				.Register();
		}
	}
}