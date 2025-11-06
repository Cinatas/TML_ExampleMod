using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace ExampleMod
{
	//ModRecipe 类 is useful 类 that can 帮助 us adding custom 配方 requirements other than materials
	//In this example my 配方 will need specific npc nearby and Eye of Cthulhu defeated
	public class ExampleAdvancedRecipe : ModRecipe
	{
		public int NeededNPCType;
		//范围 of npc 搜索
		private const int Range = 480; //30 tiles -> 30 * 16

		//In constructor (necessary thing), i'll add 参数 where we will specify npc needed
		//Mod 参数 is required here, because ModRecipe itself need it
		//that's why we have ":base(mod)" here to satisfy constructor of ModRecipe
		public ExampleAdvancedRecipe(Mod mod, int NeededNPC) : base(mod) {
			NeededNPCType = NeededNPC;
		}

		//RecipeAvailable is our 目标 here, in here we check our custom requirements
		//Also, RecipeAvailable is called on 客户端, so we can use here Main.LocalPlayer without problems
		public override bool RecipeAvailable() {
			//We will use this bool to determine is there is needed npc nearby
			bool foundNPC = false;
			//First we check does EoC was defeated, if no, we will 返回 假, so 配方 won't be available
			if (!NPC.downedBoss1) {
				return false;
			}
			//If EoC was defeated we will try 查找 out is there is required npc nearby 玩家
			foreach (NPC npc in Main.npc) {
				//If npc isn't active or isn't our needed 类型, we will 跳过 迭代
				if (!npc.active || npc.type != NeededNPCType) {
					continue;
				}
				//Otherwise we will 比较 positions
				if (Main.LocalPlayer.DistanceSQ(npc.Center) <= Range * Range) {
					foundNPC = true;
					break;
				}
			}
			//We don't 需要 check does EoC was defeated, because if it wasn't, code would 返回 earlier
			return foundNPC;
		}

		//OnCraft is called when we create 项
		public override void OnCraft(Item item) {
			//And here 一点 surprise
			Main.LocalPlayer.AddBuff(BuffID.OnFire, 120);
		}
	}

	//Here's the 项 where we will add our 配方
	public class AdvancedRecipeItem : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Advanced Recipe Test Item");
			Tooltip.SetDefault("You need help with creating this!");
		}

		public override string Texture => "ExampleMod/Items/ExampleItem";

		public override void SetDefaults() {
			item.width = 26;
			item.height = 26;
			item.rare = ItemRarityID.Blue;
		}

		//Using our custom 配方 类型
		public override void AddRecipes() {
			ExampleAdvancedRecipe recipe = new ExampleAdvancedRecipe(mod, NPCID.Guide);
			recipe.AddIngredient(ItemID.DirtBlock, 5);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
