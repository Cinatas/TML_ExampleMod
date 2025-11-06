using ExampleMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	// 示例Flail and ExampleFlailProjectile show the 最小 amount of code needed for a flail using the existing vanilla code and behavior. ExampleAdvancedFlail and ExampleAdvancedFlailProjectile need to be consulted if more advanced customization is desired, or if you want to learn more advanced modding techniques.
	// 示例Flail is a 复制 的 Sunfury flail 武器.
	internal class ExampleFlail : ModItem
	{
		public override void SetStaticDefaults() {
			// This line will make the 伤害 shown 在 工具提示 twice the actual 项.伤害. This 乘数 is used to adjust 对于 dynamic 伤害 capabilities 的 弹幕.
			// 当 thrown directly at enemies, the flail 弹幕 will deal double 项.伤害, matching the 工具提示, but deals normal 伤害 in other modes.
			ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
		}

		public override void SetDefaults() {
			// These default values aside from 项.shoot 匹配 the Sunfury values, feel free to tweak them.
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the 项 (swinging, holding out, etc.)
			Item.useAnimation = 45; // The 项's use 时间 in ticks (60 ticks == 1 second.)
			Item.useTime = 45; // The 项's use 时间 in ticks (60 ticks == 1 second.)
			Item.knockBack = 6.75f; // The knockback of your flail, this is dynamically adjusted 在 弹幕 code.
			Item.width = 30; // Hitbox 宽度 的 项.
			Item.height = 10; // Hitbox 高度 的 项.
			Item.damage = 32; // The 伤害 of your flail, this is dynamically adjusted 在 弹幕 code.
			Item.crit = 7; // Critical 伤害 概率 %
			Item.scale = 1.1f;
			Item.noUseGraphic = true; // This makes sure the 项 does not get shown when the 玩家 swings his hand
			Item.shoot = ModContent.ProjectileType<ExampleFlailProjectile>(); // The flail 弹幕
			Item.shootSpeed = 12f; // The 速度 的 弹幕 measured in pixels per 帧.
			Item.UseSound = SoundID.Item1; // The 声音 that this 项 makes when used
			Item.rare = ItemRarityID.Orange; // The 颜色 的 名称 of your 项
			Item.value = Item.sellPrice(gold: 2, silver: 50); // Sells for 2 金币 50 银币
			Item.DamageType = DamageClass.MeleeNoSpeed; // Deals melee 伤害
			Item.channel = true;
			Item.noMelee = true; // This makes sure the 项 does not deal 伤害 从 swinging 动画
		}

		public override Color? GetAlpha(Color lightColor) {
			// Aside from SetDefaults, when making a 复制 of a vanilla 武器 you may have to hunt down other bits of code. This code makes the 项 draw in full brightness when dropped.
			return Color.White;
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
