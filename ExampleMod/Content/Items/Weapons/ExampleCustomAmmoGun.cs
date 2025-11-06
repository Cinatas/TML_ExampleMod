using ExampleMod.Content.Items.Ammo;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	// 这是 an example showing how to create a 武器 that fires custom ammunition
	// most important 属性 is "项.useAmmo". It tells you which 项 to use as ammo.
	// 你 can see the 描述 of other parameters 在 ExampleGun 类 and at https://github.com/tModLoader/tModLoader/wiki/项-类-Documentation
	public class ExampleCustomAmmoGun : ModItem
	{
		public override void SetDefaults() {
			Item.width = 42; // The 宽度 of 项 hitbox
			Item.height = 30; // The 高度 of 项 hitbox

			Item.autoReuse = true;  // Whether or not you can hold 点击 to automatically use it again.
			Item.damage = 12; // 设置s the 项's 伤害. 注意 projectiles shot by this 武器 will use its and the used ammunition's 伤害 added together.
			Item.DamageType = DamageClass.Ranged; // What 类型 of 伤害 does this 项 affect?
			Item.knockBack = 4f; // 设置s the 项's knockback. 注意 projectiles shot by this 武器 will use its and the used ammunition's knockback added together.
			Item.noMelee = true; // So the 项's 动画 doesn't do 伤害.
			Item.rare = ItemRarityID.Yellow; // The 颜色 th在 项's 名称 将 in-game.
			Item.shootSpeed = 10f; // The 速度 的 弹幕 (measured in pixels per 帧.)
			Item.useAnimation = 35; // The 长度 的 项's use 动画 in ticks (60 ticks == 1 second.)
			Item.useTime = 35; // The 项's use 时间 in ticks (60 ticks == 1 second.)
			Item.UseSound = SoundID.Item11; // The 声音 that this 项 plays when used.
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the 项 (swinging, holding out, shoot, etc.)
			Item.value = Item.buyPrice(gold: 1); // The 值 的 武器 in 铜币 coins

			// 自定义 ammo and shooting homing projectiles
			Item.shoot = ModContent.ProjectileType<Projectiles.ExampleHomingProjectile>();
			Item.useAmmo = ModContent.ItemType<ExampleCustomAmmo>(); // Restrict the 类型 of ammo the 武器 can use, so th在 武器 cannot use other ammos
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
