using ExampleMod.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	// 示例 Advanced Flail is a complete adaption of Ball O' Hurt. The 弹幕 has the complete code needed to customize all aspects 的 flail. See ExampleFlail for a simpler example 即 less customizable. 
	public class ExampleAdvancedFlail : ModItem
	{
		public override void SetStaticDefaults() {
			// This line will make the 伤害 shown 在 工具提示 twice the actual 项.伤害. This 乘数 is 用于 adjust 对于 dynamic 伤害 capabilities 的 弹幕.
			// 当 thrown directly at enemies, the flail 弹幕 will deal double 项.伤害, matching the 工具提示, but deals normal 伤害 in other modes.
			ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
		}

		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the 项 (swinging, holding out, etc.)
			Item.useAnimation = 45; // The 项's use 时间 in ticks (60 ticks == 1 second.)
			Item.useTime = 45; // The 项's use 时间 in ticks (60 ticks == 1 second.)
			Item.knockBack = 5.5f; // The knockback of your flail, this is dynamically adjusted 在 弹幕 code.
			Item.width = 32; // Hitbox 宽度 的 项.
			Item.height = 32; // Hitbox 高度 的 项.
			Item.damage = 15; // The 伤害 of your flail, this is dynamically adjusted 在 弹幕 code.
			Item.noUseGraphic = true; // 这使 sure the 项 does not get shown when the 玩家 swings his hand
			Item.shoot = ModContent.ProjectileType<ExampleAdvancedFlailProjectile>(); // The flail 弹幕
			Item.shootSpeed = 12f; // The 速度 的 弹幕 measured in pixels per 帧.
			Item.UseSound = SoundID.Item1; // The 声音 that this 项 makes when used
			Item.rare = ItemRarityID.Green; // The 颜色 的 名称 of your 项
			Item.value = Item.sellPrice(gold: 1, silver: 50); // Sells for 1 金币 50 银币
			Item.DamageType = DamageClass.MeleeNoSpeed; // Deals melee 伤害
			Item.channel = true;
			Item.noMelee = true; // 这使 sure the 项 does not deal 伤害 从 swinging 动画
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