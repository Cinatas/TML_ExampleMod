using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleShotgun : ModItem
	{
		public override void SetDefaults() {
			// Modders can use Item.DefaultToRangedWeapon to quickly set many common properties, 例如: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee. These are all shown individually here for teaching purposes.

			// 常见 Properties
			Item.width = 44; // Hitbox width 的 item.
			Item.height = 18; // Hitbox height 的 item.
			Item.rare = ItemRarityID.Green; // The color th在 item's name 将 in-game.

			// 使用 Properties
			Item.useTime = 55; // The item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 55; // The length 的 item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
			Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.
			Item.UseSound = SoundID.Item36; // The sound that this item plays when used.

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged; // 设置s the damage type to ranged.
			Item.damage = 10; // 设置s the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			Item.knockBack = 6f; // 设置s the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			Item.noMelee = true; // So the item's animation doesn't do damage.

			// Gun Properties
			Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns 在 vanilla source have this.
			Item.shootSpeed = 10f; // The speed 的 projectile (measured in pixels per frame.)
			Item.useAmmo = AmmoID.Bullet; // The "ammo Id" 的 ammo item that this weapon uses. Ammo IDs are magic numbers that usually correspond 到 item id of one item th至多 commonly represent the ammo type.
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			const int NumProjectiles = 8; // The number of projectiles that this gun will shoot.

			for (int i = 0; i < NumProjectiles; i++) {
				// Rotate the velocity randomly by 30 degrees at max.
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));

				// Decrease velocity randomly for nicer visuals.
				newVelocity *= 1f - Main.rand.NextFloat(0.3f);

				// 创建 a projectile.
				Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
			}

			return false; // 返回 false because we don't want tModLoader to shoot projectile
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}

		// 此方法 lets you adjust position 的 gun 在 player's hands. Play 与se values until it looks good with your graphics.
		public override Vector2? HoldoutOffset() {
			return new Vector2(-2f, -2f);
		}
	}
}
