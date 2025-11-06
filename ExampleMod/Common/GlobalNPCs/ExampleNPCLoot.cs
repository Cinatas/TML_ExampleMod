using ExampleMod.Common.ItemDropRules.DropConditions;
using ExampleMod.Content.Items;
using ExampleMod.Content.Items.Weapons;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalNPCs
{
	// 此文件展示了你可以使用广泛的 NPC 战利品系统做什么的众多示例。
	// 你可以在 wiki 上找到更多信息：https://github.com/tModLoader/tModLoader/wiki/Basic-NPC-Drops-and-Loot-1.4
	// 尽管此文件是 GlobalNPC，这里的所有内容也可以与 ModNPC 一起使用！请参阅 Content/NPCs 文件夹中的示例。
	public class ExampleNPCLoot : GlobalNPC
	{
		// 修改NPCLoot 使用一个名为 ItemDropDatabase 的独特系统，该系统为许多不同的掉落用例提供了许多不同的规则。
		// 在这里我们将介绍所有这些，以及如何使用它们。
		// 原版中还有大量其他示例！在反编译的原版构建中，GameContent/ItemDropRules/ItemDropDatabase 为每个原版 NPC 添加物品掉落，这可能是一个很好的资源。

		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
			if (!NPCID.Sets.CountsAsCritter[npc.type]) { // 如果 NPC 不是小动物
				// 使其掉落 ExampleItem。
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExampleItem>(), 1));

				// 在旅程模式下以 2/7 的基础概率掉落 ExampleResearchPresent，但仅在旅程模式下
				npcLoot.Add(ItemDropRule.ByCondition(new ExampleJourneyModeDropCondition(), ModContent.ItemType<ExampleResearchPresent>(), chanceDenominator: 7, chanceNumerator: 2));
			}

			// 我们现在将使用向导来解释许多其他类型的掉落规则。
			if (npc.type == NPCID.Guide) {
				// 删除Where 将删除与提供的表达式匹配的任何掉落规则。
				// 要创建自己的表达式来删除原版掉落规则，你通常必须研究添加这些规则的原始源代码。
				npcLoot.RemoveWhere(
					// 如果满足以下条件，则以下表达式返回 真：
					rule => rule is ItemDropWithConditionRule drop // 如果规则是 ItemDropWithConditionRule 实例
						&& drop.itemId == ItemID.GreenCap // And that 实例 drops a green cap&& 放下.itemId == ItemID.GreenCap // And that 实例 drops a green cap 放下.itemId == ItemID.GreenCap // 并且该实例掉落绿色蘑菇
						&& drop.condition is Conditions.NamedNPC npcNameCondition // ..And if its 条件 is that an npc 名称 must 匹配 some 字符串
						&& npcNameCondition.neededName == "Andrew" // And the 条件's 字符串 is "Andrew".&& npcNameCondition.neededName == "Andrew" // And the 条件's 字符串 is "Andrew". npcNameCondition.neededName == "Andrew" // 并且条件的字符串是"Andrew"。
				);

				npcLoot.Add(ItemDropRule.Common(ItemID.GreenCap, 1)); // 结合上面的删除，这使得任何名字的向导都会掉落绿色蘑菇。
			}

			// 编辑现有掉落规则
			if (npc.type == NPCID.BloodNautilus) {
				// 恐惧鹦鹉螺，在代码中称为 BloodNautilus，掉落血腥法杖。掉落率在专家模式下为 100%，在普通模式下为 50%。此示例将更改该比率。
				// 负责此掉落的原版代码是：ItemDropRule.NormalvsExpert(4269, 2, 1)
				// NormalvsExpert 方法创建一个 DropBasedOnExpertMode 规则，该规则由 2 个 CommonDrop 规则组成。我们需要在转换中使用此信息来正确识别要编辑的配方。

				// 有 2 个选项。一个选项是删除原始规则，然后添加回类似的规则。另一个选项是修改现有规则。
				// 最好修改现有规则以保持与其他模组的兼容性。

				// 调整现有规则：将普通模式掉落率从 50% 更改为 33.3%
				foreach (var rule in npcLoot.Get()) {
					// 你必须研究原版代码才能知道要转换为什么对象。
					if (rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule && normalDropRule.itemId == ItemID.SanguineStaff)
						normalDropRule.chanceDenominator = 3;
				}

				// 删除规则，然后添加另一个规则：将普通模式掉落率从 50% 更改为 16.6%
				/*
				npcLoot.RemoveWhere(
					rule => rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule && normalDropRule.itemId == ItemID.SanguineStaff
				);
				npcLoot.Add(ItemDropRule.NormalvsExpert(4269, 6, 1));
				*/
			}
			// 编辑现有掉落规则, but for a Boss
			// 除了此代码之外，我们还在 Common/GlobalItems/BossBagLoot.cs 中执行类似的代码来编辑 Boss 袋战利品。如果你的编辑也应该影响 Boss 袋，请记住两者都要做。
			if (npc.type == NPCID.QueenBee) {
				foreach (var rule in npcLoot.Get()) {
					if (rule is DropBasedOnExpertMode dropBasedOnExpertMode && dropBasedOnExpertMode.ruleForNormalMode is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.BeeGun)) {
						var original = oneFromOptionsDrop.dropIds.ToList();
						original.Add(ModContent.ItemType<Content.Items.Accessories.WaspNest>());
						oneFromOptionsDrop.dropIds = original.ToArray();
					}
				}
			}

			if (npc.type == NPCID.Crimera || npc.type == NPCID.Corruptor) {
				// 在这里我们使用我们自己创建的特殊规则：在白天掉落
				// 以 33% 的概率掉落另一个邪恶生物群系的物品
				int itemType = npc.type == NPCID.Crimera ? ItemID.RottenChunk : ItemID.Vertebrae;
				npcLoot.Add(ItemDropRule.ByCondition(new ExampleDropCondition(), itemType, chanceDenominator: 3));
			}

			// 使用"标准"条件的简单示例
			if (npc.aiStyle == NPCAIStyleID.Slime) {
				npcLoot.Add(ItemDropRule.ByCondition(Condition.TimeDay.ToDropCondition(ShowItemDropInUI.Always), ModContent.ItemType<ExampleSword>()));
			}

			//TODO: Add the rest 的 vanilla 放下 rules!!
		}

		// 修改GlobalLoot 允许你修改每个 NPC 都应该能够掉落的战利品，最好有一个条件。
		// 原版将其用于生物群系钥匙、夜晚/光明之魂以及节日掉落。
		// 修改GlobalLoot 中的任何掉落规则都应该只运行一次。其他所有内容都应该放在 ModifyNPCLoot 中。
		public override void ModifyGlobalLoot(GlobalLoot globalLoot) {
			// 如果 ExampleSoulCondition 为 真，则以 20% 的概率掉落 ExampleSoul。有关如何确定的信息，请参阅 Common/ItemDropRules/DropConditions/ExampleSoulCondition.cs
			globalLoot.Add(ItemDropRule.ByCondition(new ExampleSoulCondition(), ModContent.ItemType<ExampleSoul>(), 5, 1, 1));
		}
	}
}
