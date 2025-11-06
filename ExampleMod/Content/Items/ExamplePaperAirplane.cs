using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	public class ExamplePaperAirplane : ModItem
	{
		public override void SetDefaults() {
			Item.width = 22; // The 项 纹理's 宽度
			Item.height = 16; // The 项 纹理's 高度

			Item.value = Item.sellPrice(0, 0, 10); // The 值 的 项. In this case, 10 银币. 项.buyPrice & 项.sellPrice are helper methods that returns costs in 铜币 coins based on 铂金币/金币/银币/铜币 arguments provided to it.

			Item.DefaultToThrownWeapon(ModContent.ProjectileType<Projectiles.ExamplePaperAirplaneProjectile>(), 17, 5f); // A special 方法 that sets a variety of 项 parameters that make the 项 act like a throwing 武器.

			// above 项.DefaultToThrownWeapon() does the following. Uncomment these if you don't want to use the above 方法 or want to change something about it.
			// 项.autoReuse = 假;
			// 项.useStyle = ItemUseStyleID.Swing;
			// 项.useAnimation = 17;
			// 项.useTime = 17;
			// 项.shoot = ModContent.ProjectileType<Projectiles.ExamplePaperAirplaneProjectile>();
			// 项.shootSpeed = 5f;
			// 项.noMelee = 真;
			// 项.DamageType = DamageClass.Ranged;
			// 项.consumable = 真;
			// 项.maxStack = 项.CommonMaxStack;

			Item.SetWeaponValues(4, 2f); // A special 方法 that sets the 伤害, knockback, and 奖励 critical strike 概率.

			// above 项.SetWeaponValues() does the following. Uncomment these if you don't want to use the above 方法.
			// 项.伤害 = 4;
			// 项.knockBack = 2;
			// 项.crit = 0; // Even though this says 0, this is more like "奖励 critical strike 概率". All weapons have a base critical strike 概率 of 4.
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe(10)
				.AddIngredient(ModContent.ItemType<ExampleItem>())
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
