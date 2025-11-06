using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	public class ExampleSandBlock : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 100;

			// 设置 the SandgunAmmoProjectileData to your sandgun 弹幕 with a 奖励 伤害 of 10
			ItemID.Sets.SandgunAmmoProjectileData[Type] = new(ModContent.ProjectileType<Projectiles.ExampleSandBallGunProjectile>(), 10);
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.ExampleSand>());
			Item.width = 12;
			Item.height = 12;
			Item.ammo = AmmoID.Sand;
			// 项.shoot and 项.伤害 are not used for sand ammo by convention. They would result in undesireable 项 tooltips.
			// ItemID.Sets.SandgunAmmoProjectileData is used instead.
			Item.notAmmo = true;
		}

		public override void AddRecipes() {
			CreateRecipe(10)
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}