using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	// 这是 an example gun designed to best demonstrate the 各种 tML hooks that 可以 used for ammo-related specifications.
	// ammo wiki guide, https://github.com/tModLoader/tModLoader/wiki/Basic-Ammo, is a good 资源 for learning how the ammo system works.
	public class ExampleSpecificAmmoGun : ModItem
	{
		public static readonly int FreeAmmoChance1 = 20;
		public static readonly int FreeAmmoChance2 = 63;
		public static readonly int FreeAmmoChance3 = 36;
		public static readonly int AmmoUseDamageBoost = 20;

		private bool consumptionDamageBoost = false;

		public override string Texture => "ExampleMod/Content/Items/Weapons/ExampleGun"; //TODO: 删除 when 精灵 is made for this

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FreeAmmoChance1, FreeAmmoChance2, FreeAmmoChance3, AmmoUseDamageBoost);

		public override void SetDefaults() {
			// Modders can use 项.DefaultToRangedWeapon to quickly set m任何 common properties, 例如: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee. These are all shown individually here for teaching purposes.

			// 常见 Properties
			Item.width = 62; // Hitbox 宽度 的 项.
			Item.height = 32; // Hitbox 高度 的 项.
			Item.scale = 0.75f;
			Item.rare = ItemRarityID.Green; // The 颜色 th在 项's 名称 将 in-game.

			// 使用 Properties
			Item.useTime = 5; // The 项's use 时间 in ticks (60 ticks == 1 second.)
			Item.useAnimation = 15; // The 长度 的 项's use 动画 in ticks (60 ticks == 1 second.)
			Item.reuseDelay = 5; // The amount of 时间 the 项 waits between use animations (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the 项 (swinging, holding out, etc.)
			Item.autoReuse = true; // Whether or not you can hold 点击 to automatically use it again.
			Item.UseSound = SoundID.Item11;

			// 武器 Properties
			Item.DamageType = DamageClass.Ranged; // 设置s the 伤害 类型 to ranged.
			Item.damage = 20; // 设置s the 项's 伤害. Note that projectiles shot by this 武器 will use its and the used ammunition's 伤害 added together.
			Item.knockBack = 5f; // 设置s the 项's knockback. Note that projectiles shot by this 武器 will use its and the used ammunition's knockback added together.
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

		public override Vector2? HoldoutOffset() {
			return new Vector2(2f, -2f);
		}

		public override void UpdateInventory(Player player) {
			consumptionDamageBoost = false;
		}

		public override bool? CanChooseAmmo(Item ammo, Player player) {
			// CanChooseAmmo allows ammo to be chosen or denied independently 的 useAmmo 字段's restrictions.
			// (Its sister hook, CanBeChosenAsAmmo, is called 在 ammo, and has the same 函数.)
			// This returns 空 默认情况下, which simply picks the ammo 基于 whether or not ammo.ammo == 武器.useAmmo.
			// 返回ing 真 will forcibly 允许 an ammo to be used; returning 假 will forcibly 拒绝 it.
			// 对于 this example, we'll forcefully 拒绝 Cursed Bullets from being used as ammunition, but 否则 make no changes 到 ammo pool.
			if (ammo.type == ItemID.CursedBullet)
				return false;

			// This code would 允许 this 武器 to use 箭 ammo. A modder making such a 武器 would 想要 make sure the 项 工具提示 informs the 用户 about these ammo irregularities.
			//if (ammo.ammo == AmmoID.箭) {
			//	返回 真;

			// Oh, and a word of advice: always default to returning 空, as per the above.
			// 默认ing to returning 真 or 假 may have unintended consequences on what you can or can't use as ammo.
			return null;
		}

		public override bool CanConsumeAmmo(Item ammo, Player player) {
			// CanConsumeAmmo allows ammo to be conserved or consumed 取决于 各种 conditions.
			// (Its sister hook, CanBeConsumedAsAmmo, is called 在 ammo, and has the same 函数.)
			// This returns 真 默认情况下; returning 假 for 任何 reason will 防止 ammo consumption.
			// 注意 that returning 真 does NOT 允许 you to force ammo consumption; this currently requires use of IL editing or detours.

			// 对于 this example, the first shot will have a 20% 概率 to conserve ammo...
			// ...the second shot will have a 63% 概率 to conserve ammo...
			// ...and the third shot will have a 36% 概率 to conserve ammo.
			if (player.ItemUsesThisAnimation == 0)
				return Main.rand.NextFloat() >= FreeAmmoChance1 / 100f;
			else if (player.ItemUsesThisAnimation == 1)
				return Main.rand.NextFloat() >= FreeAmmoChance2 / 100f;
			else if (player.ItemUsesThisAnimation == 2)
				return Main.rand.NextFloat() >= FreeAmmoChance3 / 100f;

			return true;
		}

		public override void OnConsumeAmmo(Item ammo, Player player) {
			// OnConsumeAmmo allows you to make things happen when ammo is successfully consumed.
			// (Its sister hook, OnConsumedAsAmmo, is called 在 ammo, and has the same 函数.)
			// Here, we'll set a bool to 真 which dictates whether or not the next shot should receive a 伤害 奖励.
			// 这使 it 以便 shots which do consume ammunition gain a 伤害 奖励 in exchange for that consumption.
			consumptionDamageBoost = true;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			if (consumptionDamageBoost) {
				damage = damage * (100 + AmmoUseDamageBoost) / 100;
			}
		}
	}
}
