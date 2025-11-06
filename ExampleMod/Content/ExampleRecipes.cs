using ExampleMod.Common;
using ExampleMod.Content.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content
{
	// 此类 contains thoughtful examples of 项 配方 creation.
	// Recipes are explained in detail 在 https://github.com/tModLoader/tModLoader/wiki/Basic-Recipes and https://github.com/tModLoader/tModLoader/wiki/Intermediate-Recipes wiki pages. Please visit the wiki to learn more about recipes 如果有的话thing is unclear.
	public class ExampleRecipes : ModSystem
	{
		// 一个 place to store the 配方 分组 so 我们可以 easily use it later
		public static RecipeGroup ExampleRecipeGroup;

		public override void Unload() {
			ExampleRecipeGroup = null;
		}

		public override void AddRecipeGroups() {
			// 创建 a 配方 分组 and store it
			// Language.GetTextValue("LegacyMisc.37") is the word "Any" in English, and the corresponding word in other languages
			ExampleRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ModContent.ItemType<Items.ExampleItem>())}",
				ModContent.ItemType<Items.ExampleItem>(), ModContent.ItemType<Items.ExampleDataItem>());

			// 要 avoid 名称 collisions, when a modded items is the iconic or 1st 项 in a 配方 分组, 名称 the 配方 分组: ModName:ItemName
			RecipeGroup.RegisterGroup("ExampleMod:ExampleItem", ExampleRecipeGroup);

			// 添加 an 项 to an existing Terraria recipeGroup. ExampleCritterItem isn't 金币 but it serves as an example for this.
			RecipeGroup.recipeGroups[RecipeGroupID.GoldenCritter].ValidItems.Add(ModContent.ItemType<ExampleCritterItem>());

			// While an "IronBar" 分组 exists, "SilverBar" does not. tModLoader will 合并 配方 groups registered 与 same 名称, so if you are registering a 配方 分组 with a vanilla 项 as the 1st 项, you can register it using just the internal 项 名称 if you anticipate other mods wanting to use this 配方 分组 对于 same concept. By doing this, 多个 mods can add 到 same 分组 without extra effort. 在这种情况下 we are adding a SilverBar 分组. Don't store the RecipeGroup 实例, it might 不 used, use the same nameof(ItemID.ItemName) or RecipeGroupID returned from RegisterGroup when using 配方.AddRecipeGroup instead.
			RecipeGroup SilverBarRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}",
			ItemID.SilverBar, ItemID.TungstenBar, ModContent.ItemType<Items.Placeable.ExampleBar>());
			RecipeGroup.RegisterGroup(nameof(ItemID.SilverBar), SilverBarRecipeGroup);
		}

		public override void AddRecipes() {
			////////////////////////////////////////////////////////////////////////////////////
			// following basic 配方 makes 999 ExampleItems out of 1 stone 方块. //
			////////////////////////////////////////////////////////////////////////////////////

			Recipe recipe = Recipe.Create(ModContent.ItemType<Items.ExampleItem>(), 999);
			// This adds a 要求 of 1 stone 方块 到 配方.
			recipe.AddIngredient(ItemID.StoneBlock);
			// 当 you're done, call this to register the 配方.
			recipe.Register();

			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// following 配方 showcases and explains all methods (functions) present on 配方, and uses an 'advanced' style called 'chaining'. //
			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			// reason why the said chaining works is that all methods on 配方, 与 exception of Register(), 返回 its own 实例,
			// which lets you call subsequent methods on that 返回 值, without having to 类型 a local 变量's 名称.
			// 当 using chaining, 注意 only the last line is 应该 have a semicolon (;).

			var resultItem = ModContent.GetInstance<Items.ExampleItem>();

			// 开始 a new 配方.
			resultItem.CreateRecipe()
				// 添加s a Vanilla 成分.
				// Look up ItemIDs: https://github.com/tModLoader/tModLoader/wiki/Vanilla-Content-IDs#项-ids
				// 要 specify 超过 one 成分 类型, use 多个 配方.AddIngredient() calls.
				.AddIngredient(ItemID.StoneBlock)
				// An optional 2nd 参数 will specify a 堆叠 的 项. Any calls to 任何 AddIngredient overload without a 堆叠 值 在 结束 will have the 堆叠 default to 1.
				.AddIngredient(ItemID.Acorn, 10)
				// 我们 can also specify the current 项 as an 成分
				.AddIngredient(resultItem)
				// 添加s a Mod 成分. Do not attempt ItemID.ExampleSword, it's not how it works.
				.AddIngredient<Items.Weapons.ExampleSword>()
				// An alternate 字符串-based approach 到 above. Try to only use it for other mods' items, because it's slower.
				.AddIngredient(Mod, "ExampleSword")

				// RecipeGroups 允许 you create a 配方 that accepts items from a 分组 of similar ingredients. 例如, all varieties of Wood are 在 vanilla "Wood" 分组
				// 检查 here for other vanilla groups: https://github.com/tModLoader/tModLoader/wiki/Intermediate-Recipes#using-existing-recipegroups
				.AddRecipeGroup(RecipeGroupID.Wood)
				// Just like with AddIngredient, there's a 堆叠 参数 with a default 值 of 1.
				.AddRecipeGroup(RecipeGroupID.IronBar, 2)
				// 在这里 is using a mod 配方 分组. Check out AddRecipeGroups() to see how to register a 配方 分组.
				.AddRecipeGroup(ExampleRecipeGroup, 2)
				// An alternate 字符串-based approach 到 above. Try to only use it for other mods' groups, because it's slower.
				.AddRecipeGroup("Wood")
				.AddRecipeGroup("ExampleMod:ExampleItem", 2)

				// 添加s a vanilla 图格 要求.
				// 要 specify a 制作 station, specify a 图格. Look up TileIDs: https://github.com/tModLoader/tModLoader/wiki/Vanilla-图格-IDs
				.AddTile(TileID.WorkBenches)
				// 添加s a mod 图格 要求. To specify 超过 one 制作 station, use 多个 配方.AddTile() calls.
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				// An alternate 字符串-based approach 到 above. Try to only use it for other mods' tiles, because it's slower.
				.AddTile(Mod, "ExampleWorkbench")

				// 添加s pre-defined conditions. These 3 lines 组合 to make so th在 配方 必须 crafted in desert waters at night.
				.AddCondition(Condition.InDesert)
				.AddCondition(Condition.NearWater)
				.AddCondition(Condition.TimeNight)
				// 添加s a custom 条件, th在 玩家 必须 at <1/2 生命值 对于 配方 to work.
				// 键 used here is defined in 'Localization/*.hjson' files.
				// second 参数 uses a lambda expression to create a delegate, you can learn more about lambdas in Google.
				.AddCondition(Language.GetOrRegister("Mods.ExampleMod.Conditions.LowHealth"), () => Main.LocalPlayer.statLife < Main.LocalPlayer.statLifeMax / 2)
				// 添加s a custom 条件 that 可以 reused in other recipes easily because it is stored in a static 类. This is the recommended approach for custom conditions: https://github.com/tModLoader/tModLoader/wiki/Intermediate-Recipes#custom-conditions
				.AddCondition(ExampleConditions.InExampleBiome)

				// 当 you're done, call this to register the 配方. Note th在re's a semicolon 在 结束 的 chain.
				.Register();

			// 在 addition 到se methods, there are also methods relating to shimmer decrafting. See ShimmerShowcase.cs for that.

			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// following 配方 showcases and explains cloning recipes and how they can modified to differ 从 original recipes they came from. //
			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			// 如果 you 想要 make a 复制 of an existing 配方 with a slight difference, you can use Mod.CloneRecipe to create a clone of that 配方.
			// clone will inherit all 的 original 配方's properties except the 所有者 mod 将 this mod. You can change the clone as you see fit.
			// 如果 you 想要 make 多个 variations of a 配方 in your mod, it 可能 easier to use a helper 方法 代替 cloning.
			// 确保 to not use 配方 cloning for situations that are better served by properly using AdjTiles, 配方 Groups, or faking 各种 配方 conditions.

			// 开始 by creating a 配方 you 想要 复制.
			Recipe baseRecipe = Recipe.Create(ModContent.ItemType<Items.ExampleItem>(), 10);
			baseRecipe.AddIngredient(ItemID.Wood, 10)
				.AddIngredient(ItemID.CopperCoin)
				.AddCondition(Condition.InBeach)
				.AddCondition(Condition.TimeDay)
				.Register();

			// 开始 a new 配方 by cloning another 配方.
			Recipe clonedRecipe = baseRecipe.Clone()
				// 我们 can new properties to this 配方 without affecting the one we cloned from.
				.AddIngredient(ItemID.SilverCoin)
				.AddTile(TileID.Anvils);

			// 我们 can also 删除 properties from recipes like specific ingredients or conditions.
			clonedRecipe.RemoveIngredient(ItemID.CopperCoin);
			clonedRecipe.RemoveCondition(Condition.InBeach);

			// 当 you're done, call this to register the 配方.
			clonedRecipe.Register();

			// Recipes can also contain custom 项 consumption logic, 类似于 how the Alchemy 表格 causes 药水 recipes to consume less ingredients: See https://github.com/tModLoader/tModLoader/wiki/Intermediate-Recipes#custom-项-consumption f或更多 information.
			// 此示例 requires the Chain 项 as an 成分, but the DontConsumeChain ConsumeItemCallback causes the Chain to 不 consumed
			Recipe.Create(ItemID.AlphabetStatueJ)
				.AddIngredient(ItemID.StoneBlock, 10)
				.AddIngredient(ItemID.Chain)
				.AddConsumeItemCallback(ExampleRecipeCallbacks.DontConsumeChain)
				.AddTile(TileID.HeavyWorkBench)
				.Register();

			// Recipes can also run custom code after being crafted: See https://github.com/tModLoader/tModLoader/wiki/Intermediate-Recipes#custom-配方-craft-behavior f或更多 information.
			// 此示例 runs code that might 生成 fireworks when the 配方 is crafted.
			Recipe.Create(ItemID.AlphabetStatueZ)
				.AddIngredient(ItemID.StoneBlock, 10)
				.AddIngredient(ItemID.Chain)
				.AddOnCraftCallback(ExampleRecipeCallbacks.RandomlySpawnFireworks)
				.AddTile(TileID.HeavyWorkBench)
				.Register();
		}

		public override void PostAddRecipes() {
			for (int i = 0; i < Recipe.numRecipes; i++) {
				Recipe recipe = Main.recipe[i];

				// All recipes that require wood will now need 100% more
				if (recipe.TryGetIngredient(ItemID.Wood, out Item ingredient)) {
					ingredient.stack *= 2;
				}
			}
		}
	}
}
