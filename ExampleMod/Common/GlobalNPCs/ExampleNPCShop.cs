using ExampleMod.Content.Items;
using ExampleMod.Content.Items.Consumables;
using ExampleMod.Content.Items.Ammo;
using ExampleMod.Content.Items.Mounts;
using ExampleMod.Content.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalNPCs
{
	class ExampleNPCShop : GlobalNPC
	{
		public override void ModifyShop(NPCShop shop) {
			if (shop.NpcType == NPCID.Dryad) {
				// 向原版 NPC 添加物品很简单：
				// 此物品以正常价格出售。
				shop.Add<ExampleMountItem>();

				// 我们可以使用 shopCustomPrice 和 shopSpecialCurrency 来支持自定义价格和货币。通常商店以 item.value 的价格出售物品。
				// 在 SetupShop 中编辑 item.value 是一种不正确的方法。

				// 此商店条目以 2 个防御者奖章的价格出售。
				shop.Add(new Item(ModContent.ItemType<ExampleMountItem>()) {
					shopCustomPrice = 2,
					shopSpecialCurrency = CustomCurrencyID.DefenderMedals // omit this line if shopCustomPrice should be in regular coins.
				});

				// 此商店条目以我们模组中添加的 3 个自定义货币的价格出售。
				shop.Add(new Item(ModContent.ItemType<ExampleMountItem>()) {
					shopCustomPrice = 2,
					shopSpecialCurrency = ExampleMod.ExampleCustomCurrencyId
				});
			}
			else if (shop.NpcType == NPCID.Wizard) {
				// shopContents.Add(ModContent.ItemType<Infinity>(), ChestLoot.Condition.InExpertMode);
			}
			else if (shop.NpcType == NPCID.Stylist) {
				shop.Add<ExampleHairDye>();
			}
			else if (shop.NpcType == NPCID.BestiaryGirl) {
				shop.Add<ExampleTownPetLicense>(Condition.BestiaryFilledPercent(50));
			}
			else if (shop.NpcType == NPCID.Cyborg) {
				shop.Add<ExampleRocket>(Condition.NpcIsPresent(ModContent.NPCType<ExamplePerson>()));
			}

			// 在商人商店中添加具有复杂条件的新物品的示例。
			// 样式 1 应用检查
			if (shop.FullName != NPCShopDatabase.GetShopName(NPCID.Merchant, "Shop"))
				return;

			// 样式 2 应用检查
			if (shop.NpcType != NPCID.Merchant || shop.Name != "Shop")
				return;

			// 样式 3 应用检查（仅当 NPC 只有一个商店时才有效）
			if (shop.NpcType != NPCID.Merchant)
				return;

			// 将 ExampleTorch 添加到商人，条件是仅在白天出售。让它在火把之后出现
			shop.InsertAfter(ItemID.Torch, ModContent.ItemType<Content.Items.Placeable.ExampleTorch>(), Condition.TimeDay);

			// 隐藏铜镐和铜斧。它们将不再出现在商人商店中
			// 但是，如果物品不在商店中，此方法可能会失败。
			shop.GetEntry(ItemID.CopperAxe).Disable();

			// 禁用物品的更安全方法
			if (shop.TryGetEntry(ItemID.CopperPickaxe, out NPCShop.Entry entry)) {
				entry.Disable();
			}

			// 为蓝色照明弹添加新条件。现在只有当玩家在库存中携带照明枪并且在雪地生物群系中时才会出现
			shop.GetEntry(ItemID.BlueFlare).AddCondition(Condition.InSnow);

			// 让我们添加一个仅在刮风天出现并且 NPC 足够快乐时（可以出售晶塔）的物品
			// 如果满足条件，则将物品添加到商店。
			shop.Add<ExampleItem>(Condition.HappyWindyDay, Condition.HappyEnough);

			// 自定义条件，与上面 ExampleItem 的条件相反。
			var redPotCondition = new Condition("Mods.ExampleMod.Conditions.NotSellingExampleItem", () => !Condition.HappyWindyDay.IsMet() || !Condition.HappyEnough.IsMet());
			// 否则，如果不满足条件，那么让我们检查它是否是 For The Worthy 世界，然后出售红色药水。
			shop.Add(ItemID.RedPotion, redPotCondition, Condition.ForTheWorthyWorld);
		}
	}
}
