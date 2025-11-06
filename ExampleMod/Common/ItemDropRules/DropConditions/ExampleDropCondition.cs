using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace ExampleMod.Common.ItemDropRules.DropConditions
{
	// 非常简单的掉落条件：在白天掉落
	public class ExampleDropCondition : IItemDropRuleCondition
	{
		private static LocalizedText Description;

		public ExampleDropCondition() {
			Description ??= Language.GetOrRegister("Mods.ExampleMod.DropConditions.Example");
		}

		public bool CanDrop(DropAttemptInfo info) {
			return Main.dayTime;
		}

		public bool CanShowItemDropInUI() {
			return true;
		}

		public string GetConditionDescription() {
			return Description.Value;
		}
	}
}
