using ExampleMod.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	// 示例 Advanced Flail is a complete adaption of Ball O' Hurt. The Projectile has the complete code needed to customize all aspects 的 flail. See ExampleFlail for a simpler example 即 less customizable. 
	public class ExampleAdvancedFlail : ModItem
	{
		public override void SetStaticDefaults() {
			// This line will make the damage shown 在 tooltip twice the actual Item.damage. This multiplier is used to adjust 对于 dynamic damage capabilities 的 projectile.
			// 当 thrown directly at enemies, the flail projectile will deal double Item.damage, matching the tooltip, but deals normal damage in other modes.
			ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
		}

		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
			Item.useAnimation = 45; // The item's use time in ticks (60 ticks == 1 second.)
			Item.useTime = 45; // The item's use time in ticks (60 ticks == 1 second.)
			Item.knockBack = 5.5f; // The knockback of your flail, this is dynamically adjusted 在 projectile code.
			Item.width = 32; // Hitbox width 的 item.
			Item.height = 32; // Hitbox height 的 item.
			Item.damage = 15; // The damage of your flail, this is dynamically adjusted 在 projectile code.
			Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
			Item.shoot = ModContent.ProjectileType<ExampleAdvancedFlailProjectile>(); // The flail projectile
			Item.shootSpeed = 12f; // The speed 的 projectile measured in pixels per frame.
			Item.UseSound = SoundID.Item1; // The sound that this item makes when used
			Item.rare = ItemRarityID.Green; // The color 的 name of your item
			Item.value = Item.sellPrice(gold: 1, silver: 50); // Sells for 1 gold 50 silver
			Item.DamageType = DamageClass.MeleeNoSpeed; // Deals melee damage
			Item.channel = true;
			Item.noMelee = true; // This makes sure the item does not deal damage 从 swinging animation
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}