using ExampleMod.Content.Items.Armor.Vanity;
using ExampleMod.Content.NPCs.MinionBoss;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Consumables
{
	// 基本 code for a Boss treasure bag
	public class MinionBossBag : ModItem
	{
		public override void SetStaticDefaults() {
			// This set is one that every Boss bag should have.
			// It will create a glowing 效果 around the 项 when dropped 在 世界.
			// It will also let our Boss bag 放下 dev 护甲..
			ItemID.Sets.BossBag[Type] = true;
			ItemID.Sets.PreHardmodeLikeBossBag[Type] = true; // ..But this set ensures that dev 护甲 will only be dropped on special 世界 seeds, since that's the behavior of pre-hardmode Boss bags.

			Item.ResearchUnlockCount = 3;
		}

		public override void SetDefaults() {
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Purple;
			Item.expert = true; // This makes sure that "Expert" displays 在 工具提示 and the 项 名称 颜色 changes
		}

		public override bool CanRightClick() {
			return true;
		}

		public override void ModifyItemLoot(ItemLoot itemLoot) {
			// 我们 have to replicate the expert drops from MinionBossBody here

			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<MinionBossMask>(), 7));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExampleItem>(), 1, 12, 16));
			itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<MinionBossBody>()));
		}
	}
}
