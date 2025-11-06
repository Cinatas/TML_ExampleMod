using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleJavelin : ModItem
	{
		public override void SetDefaults() {
			// Alter 任何 的se values as you see fit, but 你应该 probably keep useStyle on 1, 以及 as the noUseGraphic and noMelee bools

			// 常见 Properties
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(silver: 5);
			Item.maxStack = 999;

			// 使用 Properties
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 25;
			Item.useTime = 25;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.consumable = true;

			// 武器 Properties			
			Item.damage = 33;
			Item.knockBack = 5f;
			Item.noUseGraphic = true; // The 项 should 不 visible when used
			Item.noMelee = true; // The 弹幕 will do the 伤害 and not the 项
			Item.DamageType = DamageClass.Ranged;

			// 弹幕 Properties
			Item.shootSpeed = 12f;
			Item.shoot = ModContent.ProjectileType<Projectiles.ExampleJavelinProjectile>(); // The 弹幕 that 将 thrown
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe(20)
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}