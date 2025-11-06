using ExampleMod.Common.Configs;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Wings)]
	public class ExampleWings : ModItem
	{
		// 要 see how this 配置 选项 was added, see ExampleModConfig.cs
		// This code allows users to toggle loading this content via a 配置. Another common usage of IsLoadingEnabled 将 to use ModLoader.HasMod to check if another mod is enabled or not.
		// Feel free to 删除 this 方法 in your own Wings if using this as a template, it is superfluous.
		public override bool IsLoadingEnabled(Mod mod) {
			return ModContent.GetInstance<ExampleModConfig>().ExampleWingsToggle;
		}

		public override void SetStaticDefaults() {
			// These wings use the same values as the solar wings
			// Fly 时间: 180 ticks = 3 seconds
			// Fly 速度: 9
			// Acceleration 乘数: 2.5
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(180, 9f, 2.5f);
		}

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 20;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) {
			ascentWhenFalling = 0.85f; // Falling glide 速度
			ascentWhenRising = 0.15f; // Rising 速度
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 3f;
			constantAscend = 0.135f;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1)) // Places this 配方 before any wing so every wing stays together 在 制作 菜单.
				.Register();
		}
	}
}
