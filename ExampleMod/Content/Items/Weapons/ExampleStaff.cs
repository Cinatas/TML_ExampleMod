using ExampleMod.Content.Projectiles;
using ExampleMod.Content.Tiles.Furniture;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	// 示例Staff is a typical staff. Staffs and other shooting weapons are very similar, this example serves mainly to show what makes staffs unique from other items.
	// Staff sprites, by convention, are angled to 点 up and 到 右. "项.staff[类型] = 真;" is essential for correctly drawing staffs.
	// Staffs use 魔力 and shoot a specific 弹幕 代替 using ammo. 项.DefaultToStaff takes care of that.
	public class ExampleStaff : ModItem
	{
		public override void SetStaticDefaults() {
			Item.staff[Type] = true; // 这使 the useStyle animate as a staff 代替 as a gun.
		}

		public override void SetDefaults() {
			// 默认ToStaff handles 设置 各种 项 values that magic staff weapons use.
			// 悬停 over DefaultToStaff in Visual Studio to read the documentation!
			Item.DefaultToStaff(ModContent.ProjectileType<SparklingBall>(), 16, 25, 12);

			// 自定义ize the UseSound. DefaultToStaff sets UseSound to SoundID.Item43, but we want SoundID.Item20
			Item.UseSound = SoundID.Item20;

			// 设置 伤害 and knockBack
			Item.SetWeaponValues(20, 5);

			// 设置 稀有度 and 值
			Item.SetShopValues(ItemRarityColor.Green2, 10000);
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<ExampleWorkbench>()
				.Register();
		}
	}
}