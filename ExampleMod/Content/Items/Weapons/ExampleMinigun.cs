using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleMinigun : ModItem
	{
		public override void SetDefaults() {
			// Modders can use 项.DefaultToRangedWeapon to quickly set m任何 common properties, 例如: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee.
			// 参见 ExampleGun.SetDefaults to see comments explaining those properties
			Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Bullet, 5, 16f, true);

			// 项.SetWeaponValues can quickly set 伤害, knockBack, and crit
			Item.SetWeaponValues(11, 1f);

			Item.width = 54; // Hitbox 宽度 的 项.
			Item.height = 22; // Hitbox 高度 的 项.
			Item.rare = ItemRarityID.Green; // The 颜色 th在 项's 名称 将 in-game.
			Item.UseSound = SoundID.Item11; // The 声音 that this 项 plays when used.
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}

		// following 方法 gives this gun a 38% 概率 to not consume ammo
		public override bool CanConsumeAmmo(Item ammo, Player player) {
			return Main.rand.NextFloat() >= 0.38f;
		}

		// following 方法 allows this gun to shoot when having no ammo, 只要 the 玩家 has 至少 10 example items 在ir 库存.
		// gun will then shoot as if the default ammo for it, 在这种情况下 the musket ball, is being used.
		public override bool NeedsAmmo(Player player) {
			return player.CountItem(ModContent.ItemType<ExampleItem>(), 10) < 10;
		}

		// following 方法 makes the gun slightly inaccurate
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
		}

		// 此方法 lets you adjust 位置 的 gun 在 玩家's hands. Play 与se values until it looks good with your graphics.
		public override Vector2? HoldoutOffset() {
			return new Vector2(-6f, -2f);
		}
	}
}
