using ExampleMod.Content.Projectiles;
using ExampleMod.Content.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleLastPrism : ModItem
	{
		// 你 can use a vanilla 纹理 for your 项 by 使用 格式: "Terraria/Item_<项 ID>".
		public override string Texture => "Terraria/Images/Item_" + ItemID.LastPrism;
		public static Color OverrideColor = new(122, 173, 255);

		public override void SetDefaults() {
			// 开始 by using CloneDefaults to clone all the basic 项 properties 从 vanilla Last Prism.
			// 对于 example, this copies 精灵 大小, use style, 出售 价格, and the 项 being a magic 武器.
			Item.CloneDefaults(ItemID.LastPrism);
			Item.mana = 4;
			Item.damage = 42;
			Item.shoot = ModContent.ProjectileType<ExampleLastPrismHoldout>();
			Item.shootSpeed = 30f;

			// 更改 the 项's draw 颜色 以便 it is visually distinct 从 vanilla Last Prism.
			Item.color = OverrideColor;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>(10)
				.AddTile<ExampleWorkbench>()
				.Register();
		}

		// Because this 武器 fires a holdout 弹幕, it needs to 方块 usage if its 弹幕 already exists.
		public override bool CanUseItem(Player player) {
			return player.ownedProjectileCounts[ModContent.ProjectileType<ExampleLastPrismHoldout>()] <= 0;
		}
	}
}