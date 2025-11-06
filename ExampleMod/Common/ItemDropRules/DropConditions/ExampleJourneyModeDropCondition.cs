using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace ExampleMod.Common.ItemDropRules.DropConditions
{
	// 掉落条件，物品仅在旅程模式下掉落。
	public class ExampleJourneyModeDropCondition : IItemDropRuleCondition
	{
		private static LocalizedText Description;

		public ExampleJourneyModeDropCondition() {
			Description ??= Language.GetOrRegister("Mods.ExampleMod.DropConditions.JourneyMode");
		}

		public bool CanDrop(DropAttemptInfo info) {
			return Main.GameModeInfo.IsJourneyMode;
		}

		public bool CanShowItemDropInUI() {
			return true;
		}

		public string GetConditionDescription() {
			return Description.Value;
		}
	}
}
