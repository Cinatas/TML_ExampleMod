using ExampleMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleGun : ModItem
	{
		public override void SetDefaults() {
			// Modders can use 项.DefaultToRangedWeapon to quickly set m任何 common properties, 例如: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee. These are all shown individually here for teaching purposes.

			// 常见 Properties
			Item.width = 62; // Hitbox 宽度 的 项.
			Item.height = 32; // Hitbox 高度 的 项.
			Item.scale = 0.75f;
			Item.rare = ItemRarityID.Green; // The 颜色 th在 项's 名称 将 in-game.

			// 使用 Properties
			Item.useTime = 8; // The 项's use 时间 in ticks (60 ticks == 1 second.)
			Item.useAnimation = 8; // The 长度 的 项's use 动画 in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the 项 (swinging, holding out, etc.)
			Item.autoReuse = true; // Whether or not you can hold 点击 to automatically use it again.

			// 声音 that this 项 plays when used.
			Item.UseSound = new SoundStyle($"{nameof(ExampleMod)}/Assets/Sounds/Items/Guns/ExampleGun") {
				Volume = 0.9f,
				PitchVariance = 0.2f,
				MaxInstances = 3,
			};

			// 武器 Properties
			Item.DamageType = DamageClass.Ranged; // 设置s the 伤害 类型 to ranged.
			Item.damage = 20; // 设置s the 项's 伤害. 注意 projectiles shot by this 武器 will use its and the used ammunition's 伤害 added together.
			Item.knockBack = 5f; // 设置s the 项's knockback. 注意 projectiles shot by this 武器 will use its and the used ammunition's knockback added together.
			Item.noMelee = true; // So the 项's 动画 doesn't do 伤害.

			// Gun Properties
			Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns 在 vanilla source have this.
			Item.shootSpeed = 16f; // The 速度 的 弹幕 (measured in pixels per 帧.)
			Item.useAmmo = AmmoID.Bullet; // The "ammo ID" 的 ammo 项 that this 武器 uses. Ammo IDs are magic numbers that usually correspond 到 项 ID of one 项 th至多 commonly represent the ammo 类型.
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
			return new Vector2(2f, -2f);
		}

		//TODO: 移动 this to a more specifically named example. Say, a paint gun?
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// Every 弹幕 shot from this gun has a 1/3 概率 of being an ExampleInstancedProjectile
			if (Main.rand.NextBool(3)) {
				type = ModContent.ProjectileType<ExampleInstancedProjectile>();
			}
		}

		/*
		* Feel free to uncomment any 的 examples below to see wh在y do
		*/

		// What if I wanted it to work like Uzi, replacing regular bullets with High 速度 Bullets?
		// Uzi/Molten Fury style: 替换 normal Bullets with High 速度
		/*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			if (type == ProjectileID.Bullet) { // or ProjectileID.WoodenArrowFriendly
				type = ProjectileID.BulletHighVelocity; // or ProjectileID.FireArrow;
			}
		}*/

		// What if I wanted 多个 projectiles in a even spread? (Vampire Knives)
		// Even Arc style: Multiple 弹幕, Even Spread
		/*public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			float numberProjectiles = 3 + Main.rand.Next(3); // 3, 4, or 5 shots
			float rotation = MathHelper.ToRadians(45);

			position += Vector2.Normalize(velocity) * 45f;
			velocity *= 0.2f; // Slow the 弹幕 down to 1/5th 速度 so 我们可以 see it. This is only here because this example shares ModItem.SetDefaults code with other examples. If you are making your own 武器 just change 项.shootSpeed as normal.

			for (int i = 0; i < numberProjectiles; i++) {
				Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))); // Watch out for dividing by 0 if there is only 1 弹幕.
				Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
			}

			return false; // 返回 假 to 停止 vanilla from calling 弹幕.NewProjectile.
		}*/

		// How can I make the shots appear out 的 muzzle exactly?
		// 另外, when I do this, how do I 防止 shooting through tiles?
		/*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
				position += muzzleOffset;
			}
		}*/

		// How can I get a "Clockwork Assault Rifle" 效果?
		// 3 round burst, only consume 1 ammo for burst. 延迟 between bursts, use reuseDelay
		// 使 the following changes to SetDefaults():
		/*
			item.useAnimation = 12;
			item.useTime = 4; // one third of useAnimation
			item.reuseDelay = 14;
			item.consumeAmmoOnLastShotOnly = true;
		*/

		// How can I shoot 2 different projectiles 在 same 时间?
		/*public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// 在这里 we manually 生成 the 2nd 弹幕, manually specifying the 弹幕 类型 that we wish to shoot.
			Projectile.NewProjectile(source, position, velocity, ProjectileID.GrenadeI, damage, knockback, player.whoAmI);

			// By returning 真, the vanilla behavior will take place, which will shoot the 1st 弹幕, the one determined by the ammo.
			return true;
		}*/

		// How can I choose between 几个 projectiles randomly?
		/*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// 在这里 we randomly set 类型 to 任一 the original (as defined by the ammo), a vanilla 弹幕, or a mod 弹幕.
			type = Main.rand.Next(new int[] { type, ProjectileID.GoldenBullet, ModContent.ProjectileType<Projectiles.ExampleBullet>() });
		}*/
	}
}
