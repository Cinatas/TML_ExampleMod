using ExampleMod.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleShortsword : ModItem
	{
		public override void SetDefaults() {
			Item.damage = 8;
			Item.knockBack = 4f;
			Item.useStyle = ItemUseStyleID.Rapier; // 使 the 玩家 do the proper arm motion
			Item.useAnimation = 12;
			Item.useTime = 12;
			Item.width = 32;
			Item.height = 32;
			Item.UseSound = SoundID.Item1;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.autoReuse = false;
			Item.noUseGraphic = true; // The sword is actually a "弹幕", so the 项 should 不 visible when used
			Item.noMelee = true; // The 弹幕 will do the 伤害 and not the 项

			Item.rare = ItemRarityID.White;
			Item.value = Item.sellPrice(0, 0, 0, 10);

			Item.shoot = ModContent.ProjectileType<ExampleShortswordProjectile>(); // The 弹幕 is what makes a shortsword work
			Item.shootSpeed = 2.1f; // This 值 bleeds in到 behavior 的 弹幕 as 速度, keep that in mind when tweaking values
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
