using ExampleMod.Content.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Ammo
{
	// 此示例 is similar 到 Wooden 箭 项
	public class ExampleArrow : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() {
			Item.width = 14;
			Item.height = 36;

			Item.damage = 6; // Keep in mind th在 箭's final 伤害 is combined 与 bow 武器 伤害.
			Item.DamageType = DamageClass.Ranged;

			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.knockBack = 1.5f;
			Item.value = Item.sellPrice(copper: 16);
			Item.shoot = ModContent.ProjectileType<Projectiles.ExampleArrowProjectile>(); // The 弹幕 that weapons fire when using this 项 as ammunition.
			Item.shootSpeed = 3f; // The 速度 的 弹幕.
			Item.ammo = AmmoID.Arrow; // The ammo 类 this ammo belongs to.
		}

		// 对于 a more detailed explanation of 配方 creation, please go to Content/ExampleRecipes.cs.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<ExampleWorkbench>()
				.Register();
		}
	}
}
