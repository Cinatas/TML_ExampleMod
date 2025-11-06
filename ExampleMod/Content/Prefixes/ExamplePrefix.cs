using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Prefixes
{
	// 此类 serves as an example for declaring item 'prefixes', or 'modifiers' in other words.
	public class ExamplePrefix : ModPrefix
	{
		// 我们 declare a custom *virtual* property here, so that another type, ExampleDerivedPrefix, could override it and change the effective power for itself.
		public virtual float Power => 1f;

		// 更改 your category this way, defaults to PrefixCategory.Custom. Affects which items can get this prefix.
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		// 参见 documentation for vanilla weights and more information.
		// 在 case of multiple prefixes with similar functions this 可以 used with a switch/case to provide different chances for different prefixes
		// Note: a weight of 0f might still be rolled. See CanRoll to exclude prefixes.
		// Note: if you use PrefixCategory.Custom, actually use ModItem.ChoosePrefix instead.
		public override float RollChance(Item item) {
			return 5f;
		}

		// 确定s if it can roll at all.
		// 使用 this to control if a prefix 可以 rolled or not.
		public override bool CanRoll(Item item) {
			return true;
		}

		// 使用 this function to modify these stats for items which have this prefix:
		// Damage Multiplier, Knockback Multiplier, Use Time Multiplier, Scale Multiplier (Size), Shoot Speed Multiplier, Mana Multiplier (Mana cost), Crit Bonus.
		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
			damageMult *= 1f + 0.20f * Power;
		}

		// 修改 the cost of items with this modifier with this function.
		public override void ModifyValue(ref float valueMult) {
			valueMult *= 1f + 0.05f * Power;
		}

		// 这是 used to modify most other stats of items which have this modifier.
		public override void Apply(Item item) {
			//
		}

		// This prefix doesn't affect any non-standard stats, so these additional tooltiplines aren't actually necessary, but this pattern 可以 followed for a prefix that does affect other stats.
		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
			// Due to inheritance, this code runs 例如Prefix and ExampleDerivedPrefix. We add 2 tooltip lines, the first is the typical prefix tooltip line showing the stats boost, while the other is just some additional flavor text.

			// localization key for Mods.ExampleMod.Prefixes.PowerTooltip uses a special format that will automatically prefix + or - 到 value.
			// This shared localization is formatted 与 Power value, resulting in different text 例如Prefix and ExampleDerivedPrefix.
			// This results in "+1 Power" 例如Prefix and "+2 Power" 例如DerivedPrefix.
			// Power isn't an actual stat, the effects of Power are already shown 在 "+X% damage" tooltip, so this example is purely educational.
			yield return new TooltipLine(Mod, "PrefixWeaponAwesome", PowerTooltip.Format(Power)) {
				IsModifier = true, // 设置s the color 到 positive modifier color.
			};
			// This localization is not shared 与 inherited classes. ExamplePrefix and ExampleDerivedPrefix have their own translations for this line.
			yield return new TooltipLine(Mod, "PrefixWeaponAwesomeDescription", AdditionalTooltip.Value) {
				IsModifier = true,
			};
			// 如果 possible and suitable, try to reuse the name identifier and translation value of Terraria prefixes. 例如, this code uses the vanilla translation 对于 word defense, resulting in "-5 defense". Note 即ModifierBad is used for this bad modifier.
			/*yield return new TooltipLine(Mod, "PrefixAccDefense", "-5" + Lang.tip[25].Value) {
				IsModifier = true,
				IsModifierBad = true,
			};*/
		}

		// PowerTooltip is shared between ExamplePrefix and ExampleDerivedPrefix. 
		public static LocalizedText PowerTooltip { get; private set; }

		// 添加itionalTooltip shows off how to do the inheritable localized properties approach. This is necessary this this example uses inheritance and we want different translations for each inheriting class. https://github.com/tModLoader/tModLoader/wiki/Localization#inheritable-localized-properties
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			// this.GetLocalization is not used here because we want to use a shared key
			PowerTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(PowerTooltip)}");
			// This seemingly useless code is required to properly register the key for AdditionalTooltip
			_ = AdditionalTooltip;
		}
	}
}
