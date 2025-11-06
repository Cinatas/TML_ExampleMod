using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleJoustingLance : ModItem
	{
		public override void SetDefaults() {
			// 一个 special method that sets a variety of item parameters that make the item act like a spear weapon.
			// 要 see everything DefaultToSpear() does, right click the method in Visual Studios and choose "Go To Definition" (or press F12). You can also hover over DefaultToSpear to see the documentation.
			// shoot speed will affect how far away the projectile spawns 从 player's hand.
			// 如果 you are using the custom AI in your projectile (and not aiStyle 19 and AIType = ProjectileID.JoustingLance), the standard value is 1f.
			// 如果 you are using aiStyle 19 and AIType = ProjectileID.JoustingLance, then multiply the value by about 3.5f.
			Item.DefaultToSpear(ModContent.ProjectileType<Projectiles.ExampleJoustingLanceProjectile>(), 1f, 24);

			Item.DamageType = DamageClass.MeleeNoSpeed; // We need to use MeleeNoSpeed here so that attack speed doesn't effect our held projectile.

			Item.SetWeaponValues(56, 12f, 0); // A special method that sets the damage, knockback, and bonus critical strike chance.

			Item.SetShopValues(ItemRarityColor.LightRed4, Item.buyPrice(0, 6)); // A special method that sets the rarity and value.

			Item.channel = true; // Channel is important for our projectile.

			// This will make sure our projectile completely disappears on hurt.
			// It's not enough just to stop the channel, as the lance can still deal damage while being stowed
			// 如果 two players charge at each other, the first one to hit should cancel the other's lance
			Item.StopAnimationOnHurt = true;
		}

		// This will allow our Jousting Lance to receive the same modifiers as melee weapons.
		public override bool MeleePrefix() {
			return true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>(5)
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}