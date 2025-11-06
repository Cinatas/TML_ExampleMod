using ExampleMod.Common.Players;
using ExampleMod.Content.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	/// <summary>
	/// ModItem 即 coupled with <seealso cref="ExampleInfoDisplay"/> and <seealso cref="ExampleInfoDisplayPlayer"/> to show
	/// off how to add a new info 饰品 (例如 a Radar, Lifeform Analyzer, etc.)
	/// </summary>
	public class ExampleInfoAccessory : ModItem
	{
		public override void SetStaticDefaults() {
			// 我们 want the information benefits of this 饰品 to work while 在 void bag in 顺序 to keep
			// it in line 与 vanilla accessories; This is the default behavior.
			// 如果 you DON'T want your info 饰品 to work 在 void bag, then add: ItemID.Sets.WorksInVoidBag[类型] = 假;
		}

		public override void SetDefaults() {
			// 我们 don't 需要 add 任何thing particularly unique 对于 stats of this 项; so let's just clone the Radar.
			Item.CloneDefaults(ItemID.Radar);
		}

		// 这是 the main hook that allows for our info 显示 to actually work with this 饰品. 
		public override void UpdateInfoAccessory(Player player) {
			player.GetModPlayer<ExampleInfoDisplayPlayer>().showMinionCount = true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<ExampleWorkbench>()
				.Register();
		}
	}
}
