using ExampleMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Tools
{
	// 示例FishingRod is a fishing rod 项.
	// code in SetDefaults and the code 设置 lineOriginOffset in ModifyFishingLine is all the 将 needed for a typical working fishing rod 项.
	// All 的 rest 的 code showcases other additional capabilities, 例如 multiple bobbers, custom line colors, and fishing in lava.
	public class ExampleFishingRod : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.CanFishInLava[Item.type] = true; // 允许s the pole to fish in lava
		}

		public override void SetDefaults() {
			// These are copied through the CloneDefaults 方法:
			// 项.宽度 = 24;
			// 项.高度 = 28;
			// 项.useStyle = ItemUseStyleID.Swing;
			// 项.useAnimation = 8;
			// 项.useTime = 8;
			// 项.UseSound = SoundID.Item1;
			Item.CloneDefaults(ItemID.WoodFishingPole);

			Item.fishingPole = 30; // 设置s the poles fishing power
			Item.shootSpeed = 12f; // 设置s the 速度 in which the bobbers are launched. Wooden Fishing Pole is 9f and Golden Fishing Rod is 17f.
			Item.shoot = ModContent.ProjectileType<Projectiles.ExampleBobber>(); // The bobber 弹幕. Note that this 将 overridden by Fishing Bobber accessories if present, so don't assume the bobber spawned is the specified 弹幕. https://terraria.wiki.gg/wiki/Fishing_Bobbers
		}

		// Grants the High 测试 Fishing Line bool if holding the 项.
		// NOTE: Only triggers through the hotbar, not if you hold the 项 by hand outside 的 库存.
		public override void HoldItem(Player player) {
			player.accFishingLine = true;
		}

		// 覆盖s the default shooting 方法 to fire multiple bobbers.
		// NOTE: This will 允许 the fishing rod to summon multiple Duke Fishrons with multiple Truffle Worms 在 库存.
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			int bobberAmount = Main.rand.Next(3, 6); // 3 to 5 bobbers
			float spreadAmount = 75f; // how much the different bobbers are spread out.

			for (int index = 0; index < bobberAmount; ++index) {
				Vector2 bobberSpeed = velocity + new Vector2(Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f, Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f);

				// 生成 new bobbers
				Projectile.NewProjectile(source, position, bobberSpeed, type, 0, 0f, player.whoAmI);
			}
			return false;
		}

		public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor) {
			// 更改 these two values in 顺序 to change the 原点 of where the line is being drawn.
			// This will make it draw 43 pixels 右 and 30 pixels up 从 玩家's 中心, while they are looking 右 and in normal gravity.
			lineOriginOffset = new Vector2(43, -30);

			// 设置s the fishing line's 颜色. Note that this 将 overridden by the colored 字符串 accessories.
			if (bobber.ModProjectile is ExampleBobber exampleBobber) {
				// 示例Bobber has custom code to decide on a line 颜色.
				lineColor = exampleBobber.FishingLineColor;
			}
			else {
				// 如果 the bobber isn't ExampleBobber, a Fishing Bobber 饰品 is in 效果 and we use DiscoColor instead.
				lineColor = Main.DiscoColor;
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>(10)
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}