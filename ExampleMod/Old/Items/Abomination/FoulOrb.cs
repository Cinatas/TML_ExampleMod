using ExampleMod.NPCs.Abomination;
using ExampleMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.Items.Abomination
{
	//imported from my tAPI mod because I'm lazy
	public class FoulOrb : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("The underworld would like this.");
			ItemID.Sets.SortingPriorityBossSpawns[item.type] = 13; // This helps 排序 库存 know this is a Boss summoning 项.
		}

		public override void SetDefaults() {
			item.width = 20;
			item.height = 20;
			item.maxStack = 20;
			item.rare = ItemRarityID.Cyan;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.UseSound = SoundID.Item44;
			item.consumable = true;
		}

		// We use the CanUseItem hook to 防止 a 玩家 from using this 项 while the Boss is present 在 世界.
		public override bool CanUseItem(Player player) {
			// "玩家.ZoneUnderworldHeight" could also be written as "玩家.位置.Y / 16f > Main.maxTilesY - 200"
			return NPC.downedPlantBoss && player.ZoneUnderworldHeight && !NPC.AnyNPCs(NPCType<NPCs.Abomination.Abomination>()) && !NPC.AnyNPCs(NPCType<CaptiveElement>()) && !NPC.AnyNPCs(NPCType<CaptiveElement2>());
		}

		public override bool UseItem(Player player) {
			NPC.SpawnOnPlayer(player.whoAmI, NPCType<NPCs.Abomination.Abomination>());
			SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.BeetleHusk);
			recipe.AddIngredient(ItemType<ScytheBlade>());
			recipe.AddIngredient(ItemType<Icicle>());
			recipe.AddIngredient(ItemType<Bubble>());
			recipe.AddIngredient(ItemID.Ectoplasm, 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<BossItem>(), 10);
			recipe.AddTile(TileType<ExampleWorkbench>());
			recipe.SetResult(this, 20);
			recipe.AddRecipe();
		}
	}
}