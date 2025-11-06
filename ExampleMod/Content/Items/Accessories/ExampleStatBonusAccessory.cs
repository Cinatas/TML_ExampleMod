using ExampleMod.Common.Players;
using ExampleMod.Content.DamageClasses;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	public class ExampleStatBonusAccessory : ModItem
	{
		// By declaring these here, changing the values will alter the 效果, and the 工具提示
		public static readonly int AdditiveDamageBonus = 25;
		public static readonly int MultiplicativeDamageBonus = 12;
		public static readonly int BaseDamageBonus = 4;
		public static readonly int FlatDamageBonus = 5;
		public static readonly int MeleeCritBonus = 10;
		public static readonly int RangedAttackSpeedBonus = 15;
		public static readonly int MagicArmorPenetration = 5;
		public static readonly int ExampleKnockback = 100;
		public static readonly int AdditiveCritDamageBonus = 20;

		// Insert the 修饰符 values in到 工具提示 localization. More info on this approach 可以 found 在 wiki: https://github.com/tModLoader/tModLoader/wiki/Localization#绑定-values-to-localizations
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AdditiveDamageBonus, MultiplicativeDamageBonus, BaseDamageBonus, FlatDamageBonus, MeleeCritBonus, RangedAttackSpeedBonus, MagicArmorPenetration, ExampleKnockback, AdditiveCritDamageBonus);

		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			// 获取Damage returns a 引用 到 specified 伤害 类' 伤害 StatModifier.
			// Since it doesn't 返回 a 值, but a 引用 to it, you can freely modify it with mathematics operators (+, -, *, /, etc.).
			// StatModifier is a structure that separately holds float additive and multiplicative modifiers, 以及 as base 伤害 and flat 伤害.
			// 当 StatModifier is applied to a 值, its additive modifiers are applied before multiplicative ones.
			// Base 伤害 is added directly 到 武器's base 伤害 and is affected by 伤害 bonuses, while flat 伤害 is applied after all other calculations.
			// 在 this case, we're doing a 数字 of things:
			// - Adding 25% 伤害, additively. This is the typical "X% 伤害 increase" that accessories use, use this one.
			// - Adding 12% 伤害, multiplicatively. This 效果 is almost never used in Terraria, typically you 想要 use the additive 乘数 above. It is extremely hard to correctly balance the game with multiplicative bonuses.
			// - Adding 4 base 伤害.
			// - Adding 5 flat 伤害.
			// Since we're using DamageClass.Generic, these bonuses apply to ALL 伤害 the 玩家 deals.
			player.GetDamage(DamageClass.Generic) += AdditiveDamageBonus / 100f;
			player.GetDamage(DamageClass.Generic) *= 1 + MultiplicativeDamageBonus / 100f;
			player.GetDamage(DamageClass.Generic).Base += BaseDamageBonus;
			player.GetDamage(DamageClass.Generic).Flat += FlatDamageBonus;

			// 获取Crit, similarly to GetDamage, returns a 引用 到 specified 伤害 类' crit 概率.
			// 在 this case, we're adding 10% crit 概率, but only 对于 melee DamageClass (as such, only melee weapons will receive this 奖励).
			// NOTE: Once all crit calculations are complete, a 武器 or 类' total crit 概率 is typically cast to an int. Plan 相应地.
			player.GetCritChance(DamageClass.Melee) += MeleeCritBonus;

			// 获取AttackSpeed is functionally identical to GetDamage and GetKnockback; it's for 攻击 速度.
			// 在 this case, we'll make ranged weapons 15% faster to use overall.
			// NOTE: Zero or a negative 值 as the result 的se calculations will throw an exception. Plan 相应地.
			player.GetAttackSpeed(DamageClass.Ranged) += RangedAttackSpeedBonus / 100f;

			// 获取ArmorPenetration is functionally identical to GetCritChance, but 对于 护甲 penetration stat instead.
			// 在 this case, we'll add 5 护甲 penetration to magic weapons.
			// NOTE: Once all 护甲 pen calculations are complete, the final 护甲 pen amount is cast to an int. Plan 相应地.
			player.GetArmorPenetration(DamageClass.Magic) += MagicArmorPenetration;

			// 获取Knockback is functionally identical to GetDamage, but 对于 knockback stat instead.
			// 在 this case, we're adding 100% knockback additively, but only for our custom example DamageClass (as such, only our example 类 weapons will receive this 奖励).
			player.GetKnockback<ExampleDamageClass>() += ExampleKnockback / 100f;

			player.GetModPlayer<ExampleDamageModificationPlayer>().AdditiveCritDamageBonus += AdditiveCritDamageBonus / 100f;
			// Some effects are applied in ExampleStatBonusAccessoryPlayer below.
			player.GetModPlayer<ExampleStatBonusAccessoryPlayer>().exampleStatBonusAccessory = true;
		}
	}

	// Some movement effects are not suit能够 be modified in ModItem.UpdateAccessory 由于 how the math is done.
	// ModPlayer.PostUpdateRunSpeeds is suitable 对于se modifications.
	public class ExampleStatBonusAccessoryPlayer : ModPlayer {
		public bool exampleStatBonusAccessory = false;

		public override void ResetEffects() {
			exampleStatBonusAccessory = false;
		}

		public override void PostUpdateRunSpeeds() {
			// 我们 only want our additional changes to apply if ExampleStatBonusAccessory is equipped and not on a 坐骑.
			if (Player.mount.Active || !exampleStatBonusAccessory) {
				return;
			}

			// following modifications are 类似于 Shadow 护甲 set 奖励
			Player.runAcceleration *= 1.75f; // 修改 玩家 run acceleration
			Player.maxRunSpeed *= 1.15f;
			Player.accRunSpeed *= 1.15f;
			Player.runSlowdown *= 1.75f;
		}
	}
}