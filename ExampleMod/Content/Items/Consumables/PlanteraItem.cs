using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Consumables
{
	// 这是 the 项 used to summon a Boss, in this case the vanilla Plantera Boss.
	public class PlanteraItem : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 3;
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps 排序 库存 know that this is a Boss summoning 项.

			// 这是 set to 真 for all NPCs that 可以 summoned via an 项 (calling NPC.SpawnOnPlayer). If this is for a modded Boss,
			// write this 在 bosses 文件 instead
			NPCID.Sets.MPAllowedEnemies[NPCID.Plantera] = true;
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 20;
			Item.value = 100;
			Item.rare = ItemRarityID.Blue;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
		}

		public override bool CanUseItem(Player player) {
			// 如果 you decide to use the below UseItem code, you have to include !NPC.AnyNPCs(ID), as this is also the check the 服务器 does when receiving MessageID.SpawnBoss
			return Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && !NPC.AnyNPCs(NPCID.Plantera);
		}

		public override bool? UseItem(Player player) {
			if (player.whoAmI == Main.myPlayer) {
				// 如果 the 玩家 using the 项 is the 客户端
				// (explicitly excluded serverside here)
				SoundEngine.PlaySound(SoundID.Roar, player.position);

				int type = NPCID.Plantera;

				if (Main.netMode != NetmodeID.MultiplayerClient) {
					// 如果 the 玩家 is not in multiplayer, 生成 directly
					NPC.SpawnOnPlayer(player.whoAmI, type);
				}
				else {
					// 如果 the 玩家 is in multiplayer, 请求 a 生成
					// This will only work if NPCID.Sets.MPAllowedEnemies[类型] is 真, which we set in this 类 above
					NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
				}
			}

			return true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}