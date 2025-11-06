using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	/*
	The items in this file showcase customizing the decrafting feature 的 Shimmer liquid.
	默认情况下, Shimmer will transform crafted items back in到ir original recipe ingredients.

	ShimmerShowcaseConditions showcases crimson and corruption specific shimmer decrafting results.

	ShimmerShowcaseCustomShimmerResult showcases both preventing a recipe from being decrafted and specifying a custom shimmer decrafting result.

	To use Shimmer to transform an item into another item instead of decrafting an item, simply set "ItemID.Sets.ShimmerTransformToItem[Type] = ItemType here;" in SetStaticDefaults. ExampleMusicBox and ExampleTorch show examples of this.

	Also note that critter items (Item.makeNPC > 0) will also not attempt to decraft, but will instead transform in到 NPC th在 Item.makeNPC transforms into. NPCID.Sets.ShimmerTransformToNPC sets which NPC an NPC will transform into, see PartyZombie and ExampleCustomAISlimeNPC 例如s of this.
	*/
	public class ShimmerShowcaseConditions : ModItem
	{
		public override string Texture => "ExampleMod/Content/Items/ExampleItem";

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 20;
		}

		public override void AddRecipes() {
			// M任何 items have 多个 recipes. The first added 配方 will usually be used for shimmer decrafting.
			// 配方 decraft conditions 可能 用于 only 允许 decrafting under certain conditions, the first 配方 found that satisfies 所有 it's decraft conditions 将 used.
			// Therefore, this desert-specific example has priority over the 世界 evil examples registered after it.
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddIngredient(ItemID.Cactus)
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.AddDecraftCondition(Condition.InDesert)
				.Register();

			// 在 these 2 examples, decraft conditions are 用于 make the recipes decraftable only 在ir respective 世界 types
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddIngredient(ItemID.RottenChunk)
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.AddDecraftCondition(Condition.CorruptWorld)
				.Register();

			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddIngredient(ItemID.Vertebrae)
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.AddDecraftCondition(Condition.CrimsonWorld)
				.Register();

			// 最后, the ApplyConditionsAsDecraftConditions 方法 可以 用于 quickly mirror 任何 制作 conditions on到 decrafting conditions.
		}
	}

	public class ShimmerShowcaseCustomShimmerResult : ModItem
	{
		public override string Texture => "ExampleMod/Content/Items/ExampleItem";

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 20;
		}

		public override void AddRecipes() {
			// 默认情况下, the first added 配方 将 used for shimmer decrafting. 我们可以 use DisableDecraft() to tell 游戏 to 忽略 this 配方 and use the below 配方 instead.
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddIngredient(ItemID.PadThai)
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.DisableDecraft()
				.Register();

			// 添加CustomShimmerResult 可以 用于 change the decrafting results. Rather that 返回 1 ExampleItem, decrafting this 项 will 返回 1 Rotten Egg and 3 Chain.
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.AddCustomShimmerResult(ItemID.RottenEgg)
				.AddCustomShimmerResult(ItemID.Chain, 3)
				.Register();
		}
	}
}