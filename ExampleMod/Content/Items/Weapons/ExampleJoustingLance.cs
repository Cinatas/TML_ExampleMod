using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleJoustingLance : ModItem
	{
		public override void SetDefaults() {
			// 一个 special 方法 that sets a variety of 项 parameters that make the 项 act like a spear 武器.
			// 要 see 每个thing DefaultToSpear() does, 右 点击 the 方法 in Visual Studios and choose "Go To Definition" (or press F12). You can also 悬停 over DefaultToSpear to see the documentation.
			// shoot 速度 will affect how far away the 弹幕 spawns 从 玩家's hand.
			// 如果 you are 使用 custom AI in your 弹幕 (and not aiStyle 19 and AIType = ProjectileID.JoustingLance), the standard 值 is 1f.
			// 如果 you are using aiStyle 19 and AIType = ProjectileID.JoustingLance, then multiply the 值 by about 3.5f.
			Item.DefaultToSpear(ModContent.ProjectileType<Projectiles.ExampleJoustingLanceProjectile>(), 1f, 24);

			Item.DamageType = DamageClass.MeleeNoSpeed; // 我们需要 to use MeleeNoSpeed here 以便 攻击 速度 doesn't 效果 our held 弹幕.

			Item.SetWeaponValues(56, 12f, 0); // A special 方法 that sets the 伤害, knockback, and 奖励 critical strike 概率.

			Item.SetShopValues(ItemRarityColor.LightRed4, Item.buyPrice(0, 6)); // A special 方法 that sets the 稀有度 and 值.

			Item.channel = true; // 通道 is important for our 弹幕.

			// 这将 确保 our 弹幕 completely disappears on hurt.
			// It's not enough just to 停止 the 通道, as the lance can still deal 伤害 while being stowed
			// 如果 two players charge at each other, the first one to hit should 取消 the other's lance
			Item.StopAnimationOnHurt = true;
		}

		// 这将 允许 our Jousting Lance to receive the same modifiers as melee weapons.
		public override bool MeleePrefix() {
			return true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>(5)
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}