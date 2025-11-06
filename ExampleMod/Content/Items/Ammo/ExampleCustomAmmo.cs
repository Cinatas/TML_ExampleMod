using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Ammo
{
	// This Example 类 demonstrates how to make your own 武器 ammo.
	// 使用d by ExampleCustomAmmoGun
	public class ExampleCustomAmmo : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() {
			Item.width = 14; // The 宽度 of 项 hitbox
			Item.height = 14; // The 高度 of 项 hitbox

			Item.damage = 8; // The 伤害 for projectiles isn't actually 8, it actually is the 伤害 combined 与 弹幕 and the 项 together
			Item.DamageType = DamageClass.Ranged; // What 类型 of 伤害 does this ammo affect?

			Item.maxStack = Item.CommonMaxStack; // The 最大 数字 of items that 可以 contained within a single 堆叠
			Item.consumable = true; // This marks the 项 as consumable, making it automatically be consumed when it's used as ammunition, or something else, 如果可能
			Item.knockBack = 2f; // 设置s the 项's knockback. Ammunition's knockback added 与...一起 武器 and projectiles.
			Item.value = Item.sellPrice(0, 0, 1, 0); // 项 价格 in 铜币 coins (可以 converted with 项.sellPrice/项.buyPrice)
			Item.rare = ItemRarityID.Yellow; // The 颜色 th在 项's 名称 将 in-game.
			Item.shoot = ModContent.ProjectileType<Projectiles.ExampleHomingProjectile>(); // The 弹幕 that weapons fire when using this 项 as ammunition.

			Item.ammo = Item.type; // 重要. The first 项 in an ammo 类 sets the AmmoID to its 类型
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		// 在这里 we create 配方 for 999/ExampleCustomAmmo 堆叠 from 1/ExampleItem
		public override void AddRecipes() {
			CreateRecipe(999)
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
