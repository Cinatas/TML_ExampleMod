using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod
{
	// In this 类 we 分离 配方 related code from our main 类
	public static class RecipeHelper
	{
		// Here we've made a helper 方法 我们可以 use to shorten our code.
		// This is because m任何 of our recipes follow the same terminology: one 成分, one result, one possible required 图格
		private static void MakeSimpleRecipe(Mod mod, string modIngredient, short resultType, int ingredientStack = 1, int resultStack = 1, string reqTile = null)
		// notice the last parameters 可以 made optional by specifying a default 值
		{
			ModRecipe recipe = new ModRecipe(mod); // 使 a new 配方 for our mod
			recipe.AddIngredient(null, modIngredient, ingredientStack); // 添加 the 成分, passing 空 对于 mod means it will use our mod, we could also pass mod 从 arguments
			if (reqTile != null) { // when a required 图格 is specified
				recipe.AddTile(null, reqTile); // we add it 
			}

			recipe.SetResult(resultType, resultStack); // 设置 the result 到 specified 类型 and 与 specified 堆叠.
			recipe.AddRecipe(); // finally, add the 配方
		}

		// 添加 recipes
		public static void AddExampleRecipes(Mod mod) {
			// 示例Item crafts in到 following items
			// 检查 the 方法 签名 of MakeSimpleRecipes 对于 arguments, 这是一个 方法 签名:
			// private static void MakeSimpleRecipe(Mod mod, 字符串 modIngredient, short resultType, int ingredientStack = 1, int resultStack = 1, 字符串 reqTile = 空) 

			MakeSimpleRecipe(mod, "ExampleItem", ItemID.Silk, 999);
			MakeSimpleRecipe(mod, "ExampleItem", ItemID.IronOre, 999);
			MakeSimpleRecipe(mod, "ExampleItem", ItemID.GravitationPotion, 20);
			MakeSimpleRecipe(mod, "ExampleItem", ItemID.GoldChest); // notice how 我们可以 omit the 堆叠, it has a default 值
			MakeSimpleRecipe(mod, "ExampleItem", ItemID.MusicBoxDungeon);

			// 代替 of having to call AddBossRecipes from our main 文件, 我们可以 also call it here, 因此 the 方法 can remain private
			AddBossRecipes(mod);
		}

		// 添加 Boss related recipes
		private static void AddBossRecipes(Mod mod) {
			// BossItem crafts in到 following items
			// We are 使用 same helper 方法 here, and we are making use 的 reqTile 参数
			MakeSimpleRecipe(mod, "BossItem", ItemID.SuspiciousLookingEye, 10, 20, "ExampleWorkbench");
			MakeSimpleRecipe(mod, "BossItem", ItemID.BloodySpine, 10, 20, "ExampleWorkbench");
			MakeSimpleRecipe(mod, "BossItem", ItemID.Abeemination, 10, 20, "ExampleWorkbench");
			// notice how 我们可以 跳过 optional parameters by specifying the 目标 参数 with 'reqTile:', 这意味着 the resultStack will remain 1
			MakeSimpleRecipe(mod, "BossItem", ItemID.GuideVoodooDoll, 10, reqTile: "ExampleWorkbench");
			MakeSimpleRecipe(mod, "BossItem", ItemID.MechanicalEye, 10, 20, "ExampleWorkbench");
			MakeSimpleRecipe(mod, "BossItem", ItemID.MechanicalWorm, 10, 20, "ExampleWorkbench");
			MakeSimpleRecipe(mod, "BossItem", ItemID.MechanicalSkull, 10, 20, "ExampleWorkbench");
			// Here we see another way to retrieve 类型 ids from classnames, using generic calls
			// This way you don't 必须 specify the mod, because you simply pass the ID 的 项 as you would for vanilla items.
			// 使用ful for those who program in an IDE who wish to avoid spelling mistakes.
			// What's also neat is th在 references to classes 可以 automatically included in refactors, 字符串 literals cannot. (unless you have ReSharper)
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<Items.BossItem>(), 10); // Items is our namespace (ExampleMod.Items), BossItem our 类
			recipe.AddTile(TileType<Tiles.ExampleWorkbench>()); // Tiles is our namespace (ExampleMod.Tiles), ExampleWorkbench our 类
			recipe.SetResult(ItemID.LihzahrdPowerCell, 20);
			recipe.AddRecipe();
		}

		// 显示case RecipeFinder and RecipeEditor
		// 与se classes, you can 查找 and edit recipes
		public static void ExampleRecipeEditing(Mod mod) {
			// 在 following example, we 查找 recipes that uses a chain as 成分 然后 we 删除 that 成分 从 配方.
			RecipeFinder finder = new RecipeFinder(); // 使 a new RecipeFinder
			finder.AddIngredient(ItemID.Chain); // 添加 Chain (with a 堆叠 of 1) 到 finder

			foreach (Recipe recipe in finder.SearchRecipes()) // 循环 每个 配方 found by the finder
			{
				RecipeEditor editor = new RecipeEditor(recipe); // 对于 currently looped 配方, make a new RecipeEditor
				editor.DeleteIngredient(ItemID.Chain); // 删除 the Chain 成分.
			}

			// 以下 is a more precise example, finding an exact 配方 and deleting it 如果可能.
			finder = new RecipeFinder(); // 使 a new RecipeFinder
			finder.AddRecipeGroup("IronBar"); // 添加 a new 配方 分组, 在这种情况下 the vanilla one for iron or lead bars.
			finder.AddTile(TileID.Anvils); // 添加 a required 图格, 任何 anvil
			finder.SetResult(ItemID.Chain, 10); // 设置 the result to be 10 chains
			Recipe exactRecipe = finder.FindExactRecipe(); // 尝试 查找 the exact 配方 matching our criteria

			bool isRecipeFound = exactRecipe != null; // if our 配方 is not 空, it means we found the exact 配方
			if (isRecipeFound) // since our 配方 is found, 我们可以 继续
			{
				RecipeEditor editor = new RecipeEditor(exactRecipe); // for our 配方, make a new RecipeEditor
				editor.DeleteRecipe(); // 删除 the 配方
			}
		}
	}
}