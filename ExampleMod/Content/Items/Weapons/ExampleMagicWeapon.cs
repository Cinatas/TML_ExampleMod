using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleMagicWeapon : ModItem
	{
		public override void SetDefaults() {
			// 默认ToStaff handles 设置 various 项 values that magic staff weapons use.
			// 悬停 over DefaultToStaff in Visual Studio to read the documentation!
			// Shoot a black bolt, also known as the 弹幕 shot 从 onyx blaster.
			Item.DefaultToStaff(ProjectileID.BlackBolt, 7, 20, 11);
			Item.width = 34;
			Item.height = 40;
			Item.UseSound = SoundID.Item71;

			// 一个 special 方法 that sets the 伤害, knockback, and 奖励 critical strike 概率.
			// This 武器 has a crit of 32% 即 added 到 players default crit 概率 of 4%
			Item.SetWeaponValues(25, 6, 32);

			Item.SetShopValues(ItemRarityColor.LightRed4, 10000);
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}

		public override void ModifyManaCost(Player player, ref float reduce, ref float mult) {
			// 我们 can use ModifyManaCost to dynamically adjust the 魔力 成本 of this 项, similar to how Space Gun works 与 Meteor 护甲 set.
			// 参见 ExampleHood to see how accessories give the reduce 魔力 成本 效果.
			if (player.statLife < player.statLifeMax2 / 2) {
				mult *= 0.5f; // Half the 魔力 成本 when at low 生命值. Make sure to use multiplication 与 mult 参数.
			}
		}
	}
}
