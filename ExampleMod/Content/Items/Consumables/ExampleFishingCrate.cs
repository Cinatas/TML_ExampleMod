using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Consumables
{
	// 基本 code for a fishing crate
	// catch code is in a 分离 ModPlayer 类 (ExampleFishingPlayer)
	// placed 图格 is in a 分离 ModTile 类
	public class ExampleFishingCrate : ModItem
	{
		public override void SetStaticDefaults() {
			// Disclaimer for both 的se sets (as per their docs): They are only checked for vanilla 项 IDs, but for cross-mod purposes it 将 helpful to set them for modded crates too
			ItemID.Sets.IsFishingCrate[Type] = true;
			//ItemID.Sets.IsFishingCrateHardmode[类型] = 真; // This is a crate that mimics a pre-hardmode 生物群系 crate, so this is commented out

			Item.ResearchUnlockCount = 10;
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.ExampleFishingCrate>());
			Item.width = 12; //The hitbox dimensions are intentionally smaller so that it looks nicer when fished up on a bobber
			Item.height = 12;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(0, 2);
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
		}

		public override bool CanRightClick() {
			return true;
		}

		public override void ModifyItemLoot(ItemLoot itemLoot) {
			// 放下 a special 武器/饰品 etc. specific to this crate's theme (i.e. Sky Crate dropping Fledgling Wings or Starfury)
			int[] themedDrops = new int[] {
				ModContent.ItemType<Accessories.ExampleBeard>(),
				ModContent.ItemType<Accessories.ExampleStatBonusAccessory>()
			};
			itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, themedDrops));

			// 放下 coins
			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 4, 5, 13));

			// 放下 pre-hm ores, 与 addition of one from ExampleMod
			IItemDropRule[] oreTypes = new IItemDropRule[] {
				ItemDropRule.Common(ItemID.CopperOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.TinOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.IronOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.LeadOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.SilverOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.TungstenOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.GoldOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.PlatinumOre, 1, 30, 50),
				ItemDropRule.Common(ModContent.ItemType<Placeable.ExampleOre>(), 1, 30, 50),
			};
			itemLoot.Add(new OneFromRulesRule(7, oreTypes));

			// 放下 pre-hm bars (except 铜币/tin), 与 addition of one from ExampleMod
			IItemDropRule[] oreBars = new IItemDropRule[] {
				ItemDropRule.Common(ItemID.IronBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.LeadBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.SilverBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.TungstenBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.GoldBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.PlatinumBar, 1, 10, 21),
				ItemDropRule.Common(ModContent.ItemType<Placeable.ExampleBar>(), 1, 10, 21),
			};
			itemLoot.Add(new OneFromRulesRule(4, oreBars));

			// 放下 an "exploration utility" 药水, 与 addition of one from ExampleMod
			IItemDropRule[] explorationPotions = new IItemDropRule[] {
				ItemDropRule.Common(ItemID.ObsidianSkinPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.SpelunkerPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.HunterPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.GravitationPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.MiningPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.HeartreachPotion, 1, 2, 5),
				ItemDropRule.Common(ModContent.ItemType<Consumables.ExampleBuffPotion>(), 1, 2, 5),
			};
			itemLoot.Add(new OneFromRulesRule(4, explorationPotions));

			// 放下 (pre-hm) 资源 药水
			IItemDropRule[] resourcePotions = new IItemDropRule[] {
				ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 18),
				ItemDropRule.Common(ItemID.ManaPotion, 1, 5, 18),
			};
			itemLoot.Add(new OneFromRulesRule(2, resourcePotions));

			// 放下 (high-结束) bait
			IItemDropRule[] highendBait = new IItemDropRule[] {
				ItemDropRule.Common(ItemID.JourneymanBait, 1, 2, 7),
				ItemDropRule.Common(ItemID.MasterBait, 1, 2, 7),
			};
			itemLoot.Add(new OneFromRulesRule(2, highendBait));
		}
	}
}
