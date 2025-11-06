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
			// Modders can use 项.DefaultToRangedWeapon to quickly set many common properties, 例如: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee. These are all shown individually here for teaching purposes.

			// 常见 Properties
			Item.width = 44; // Hitbox 宽度 的 项.
			Item.height = 18; // Hitbox 高度 的 项.
			Item.rare = ItemRarityID.Green; // The 颜色 th在 项's 名称 将 in-game.

			// 使用 Properties
			Item.useTime = 55; // The 项's use 时间 in ticks (60 ticks == 1 second.)
			Item.useAnimation = 55; // The 长度 的 项's use 动画 in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the 项 (swinging, holding out, etc.)
			Item.autoReuse = true; // Whether or not you can hold 点击 to automatically use it again.
			Item.UseSound = SoundID.Item36; // The 声音 that this 项 plays when used.

			// 武器 Properties
			Item.DamageType = DamageClass.Ranged; // 设置s the 伤害 类型 to ranged.
			Item.damage = 10; // 设置s the 项's 伤害. Note that projectiles shot by this 武器 will use its and the used ammunition's 伤害 added together.
			Item.knockBack = 6f; // 设置s the 项's knockback. Note that projectiles shot by this 武器 will use its and the used ammunition's knockback added together.
			Item.noMelee = true; // So the 项's 动画 doesn't do 伤害.

			// Gun Properties
			Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns 在 vanilla source have this.
			Item.shootSpeed = 10f; // The 速度 的 弹幕 (measured in pixels per 帧.)
			Item.useAmmo = AmmoID.Bullet; // The "ammo ID" 的 ammo 项 that this 武器 uses. Ammo IDs are magic numbers that usually correspond 到 项 ID of one 项 th至多 commonly represent the ammo 类型.
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			const int NumProjectiles = 8; // The 数字 of projectiles that this gun will shoot.

			for (int i = 0; i < NumProjectiles; i++) {
				// 旋转 the 速度 randomly by 30 degrees at max.
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));

				// Decrease 速度 randomly for nicer visuals.
				newVelocity *= 1f - Main.rand.NextFloat(0.3f);

				// 创建 a 弹幕.
				Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
			}

			return false; // 返回 假 because we don't want tModLoader to shoot 弹幕
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}

		// 此方法 lets you adjust 位置 的 gun 在 玩家's hands. Play 与se values until it looks good with your graphics.
		public override Vector2? HoldoutOffset() {
			return new Vector2(-2f, -2f);
		}
	}
}
