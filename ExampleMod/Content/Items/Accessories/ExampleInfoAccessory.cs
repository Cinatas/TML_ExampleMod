using ExampleMod.Common.Players;
using ExampleMod.Content.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	/// <summary>
	/// ModItem that is coupled with <seealso cref="ExampleInfoDisplay"/> and <seealso cref="ExampleInfoDisplayPlayer"/> to show
	/// off how to add a new info accessory (such as a Radar, Lifeform Analyzer, etc.)
	/// </summary>
	public class ExampleInfoAccessory : ModItem
	{
		public override void SetStaticDefaults() {
			// 我们 want the information benefits of this accessory to work while in the void bag in order to keep
			// it in line with the vanilla accessories; This is the default behavior.
			// 如果 you DON'T want your info accessory to work in the void bag, then add: ItemID.Sets.WorksInVoidBag[Type] = false;
		}

		public override void SetDefaults() {
			// 我们 don't need to add anything particularly unique for the stats of this item; so let's just clone the Radar.
			Item.CloneDefaults(ItemID.Radar);
		}

		// 这是 the main hook that allows for our info display to actually work with this accessory. 
		public override void UpdateInfoAccessory(Player player) {
			player.GetModPlayer<ExampleInfoDisplayPlayer>().showMinionCount = true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<ExampleWorkbench>()
				.Register();
		}
	}
}
