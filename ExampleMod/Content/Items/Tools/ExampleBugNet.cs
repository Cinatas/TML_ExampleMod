using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Tools
{
	// 这是 an example bug net designed to demonstrate the use cases for various hooks related to catching NPCs 例如 critters with items.
	public class ExampleBugNet : ModItem
	{
		public static readonly int LavaCatchChance = 20;
		public static readonly int WarmthLavaCatchChance = 50;
		public static readonly int BonusCritterChance = 5;

		public override string Texture => $"Terraria/Images/Item_{ItemID.BugNet}";

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LavaCatchChance, WarmthLavaCatchChance, BonusCritterChance);

		public override void SetStaticDefaults() {
			// This set is needed to define an 项 as a tool for catching NPCs at all.
			// An additional set exists called LavaproofCatchingTool which will 允许 your 项 to freely catch the Underworld's lava critters. Use it accordingly.
			ItemID.Sets.CatchingTool[Item.type] = true;

			// This 项 does not meet Terraria's automatic criteria to be filtered under the "Tools" 过滤 in Journey 模式's duplication 菜单.
			// As such, this set is needed to manually indicate that this 项 is to be filtered under the "Tools" 过滤.
			ItemID.Sets.DuplicationMenuToolsFilter[Item.type] = true;
		}

		public override void SetDefaults() {
			// These are, with a few modifications, the properties applied 到 base Bug Net; they're provided here so that you can mess 与m as you please.
			// Explanations 在m 将 glossed over here, as they're not the primary 点 的 lesson.
			// 常见 Properties
			Item.width = 24;
			Item.height = 28;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 0, 40);

			// 使用 Properties
			Item.useAnimation = 25;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.UseSound = SoundID.Item1;
		}

		public override bool? CanCatchNPC(NPC target, Player player) {
			// This hook is used to determine whether or not your catching tool can catch a given NPC.
			// This returns 空 默认情况下, which allows vanilla to decide whether or not the NPC 应该 caught.
			// 返回ing 真 forces the NPC to be caught, while returning 假 forces the NPC to 不 caught.
			// 如果 you're unsure what to 返回, 返回 空.
			// 对于 this example, we'll give our example bug net a 20% 概率 to catch lava critters successfully (50% with a Warmth 药水 增益 active).
			if (ItemID.Sets.IsLavaBait[target.catchItem]) {
				if (Main.rand.NextBool(player.resistCold ? WarmthLavaCatchChance : LavaCatchChance, 100)) {
					return true;
				}
			}

			// 对于 all cases where 真 isn't explicitly returned, we'll 返回 空 so that vanilla catching rules and effects can take place.
			return null;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}

	// 此类 is included here as a demonstration of how to use OnSpawn to modify the 项 spawned from catching an NPC or other entity.
	public class ExampleCatchItemModification : GlobalItem
	{
		public override void OnSpawn(Item item, IEntitySource source) {
			if (source is not EntitySource_Caught catchEntity) {
				return;
			}

			if (catchEntity.Entity is Player player) {
				// Gives a 5% 概率 对于 Example Bug Net to duplicate caught NPCs.
				if (player.HeldItem.type == ModContent.ItemType<ExampleBugNet>() && Main.rand.NextBool(ExampleBugNet.BonusCritterChance, 100)) {
					item.stack *= 2;
				}
			}
		}
	}
}
