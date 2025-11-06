using ExampleMod.Content.NPCs.MinionBoss;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Consumables
{
	// 这是 the 项 用于 summon a Boss, in this case the modded 仆从 Boss from Example Mod. For vanilla Boss summons, see comments in SetStaticDefaults
	public class MinionBossSummonItem : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 3;
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps 排序 库存 know that this is a Boss summoning 项.

			// 如果 this 将 for a vanilla Boss that has no summon 项, you would 必须 include this line here:
			// NPCID.Sets.MPAllowedEnemies[NPCID.Plantera] = 真;

			// 否则 the UseItem code to 生成 it will not work in multiplayer
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
			// 如果 you decide to use the below UseItem code, you 必须 include !NPC.AnyNPCs(ID), as this is also the check the 服务器 does when receiving MessageID.SpawnBoss.
			// 如果 you want more constraints 对于 summon 项, 组合 them as 布尔值 expressions:
			//    返回 !Main.IsItDay() && !NPC.AnyNPCs(ModContent.NPCType<MinionBossBody>()); would mean "not daytime and no MinionBossBody currently alive"
			return !NPC.AnyNPCs(ModContent.NPCType<MinionBossBody>());
		}

		public override bool? UseItem(Player player) {
			if (player.whoAmI == Main.myPlayer) {
				// 如果 the 玩家 using the 项 is the 客户端
				// (explicitly excluded serverside here)
				SoundEngine.PlaySound(SoundID.Roar, player.position);

				int type = ModContent.NPCType<MinionBossBody>();

				if (Main.netMode != NetmodeID.MultiplayerClient) {
					// 如果 the 玩家 is not in multiplayer, 生成 directly
					NPC.SpawnOnPlayer(player.whoAmI, type);
				}
				else {
					// 如果 the 玩家 is in multiplayer, 请求 a 生成
					// 这将 only work if NPCID.Sets.MPAllowedEnemies[类型] is 真, which we set in MinionBossBody
					NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
				}
			}

			return true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile(TileID.DemonAltar)
				.Register();
		}
	}
}