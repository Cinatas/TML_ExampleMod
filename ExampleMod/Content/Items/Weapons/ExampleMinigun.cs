using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleMinigun : ModItem
	{
		public override void SetDefaults() {
			// Modders can use Item.DefaultToRangedWeapon to quickly set many common properties, 例如: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee.
			// 参见 ExampleGun.SetDefaults to see comments explaining those properties
			Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Bullet, 5, 16f, true);

			// Item.SetWeaponValues can quickly set damage, knockBack, and crit
			Item.SetWeaponValues(11, 1f);

			Item.width = 54; // Hitbox width 的 item.
			Item.height = 22; // Hitbox height 的 item.
			Item.rare = ItemRarityID.Green; // The color th在 item's name 将 in-game.
			Item.UseSound = SoundID.Item11; // The sound that this item plays when used.
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}

		// following method gives this gun a 38% chance to not consume ammo
		public override bool CanConsumeAmmo(Item ammo, Player player) {
			return Main.rand.NextFloat() >= 0.38f;
		}

		// following method allows this gun to shoot when having no ammo, as long as the player has 至少 10 example items 在ir inventory.
		// gun will then shoot as if the default ammo for it, in this case the musket ball, is being used.
		public override bool NeedsAmmo(Player player) {
			return player.CountItem(ModContent.ItemType<ExampleItem>(), 10) < 10;
		}

		// following method makes the gun slightly inaccurate
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
		}

		// 此方法 lets you adjust position 的 gun 在 player's hands. Play 与se values until it looks good with your graphics.
		public override Vector2? HoldoutOffset() {
			return new Vector2(-6f, -2f);
		}
	}
}
