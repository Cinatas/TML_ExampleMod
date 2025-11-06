using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.DamageClasses
{
	public class ExampleDamageClass : DamageClass
	{
		// 这是 an example 伤害 类 designed to demonstrate all the current functionality 的 feature and explain how to create one of your own, should you need one.
		// 对于 information about how to apply stat bonuses to specific 伤害 classes, please instead refer to ExampleMod/Content/Items/Accessories/ExampleStatBonusAccessory.
		public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
			// 此方法 lets you make your 伤害 类 benefit from other classes' stat bonuses 默认情况下, 以及 as universal stat bonuses.
			// 要 briefly summarize the two nonstandard 伤害 类 names used by DamageClass:
			// 默认 is, you guessed it, the default 伤害 类. It doesn't 缩放 off of 任何 类-specific stat bonuses or universal stat bonuses.
			// 有 a 数字 of items and projectiles that use this, 例如 thrown waters and the Bone Glove's bones.
			// Generic, 在 other hand, scales off of all universal stat bonuses and nothing else; it's the base 伤害 类 upon which all others that aren't Default are built.
			if (damageClass == DamageClass.Generic)
				return StatInheritanceData.Full;

			return new StatInheritanceData(
				damageInheritance: 0f,
				critChanceInheritance: 0f,
				attackSpeedInheritance: 0f,
				armorPenInheritance: 0f,
				knockbackInheritance: 0f
			);
			// Now, what exactly did we just do, you might ask? Well, let's see here...
			// StatInheritanceData is a struct which you'll 需要 返回 one of for 任何 given outcome this 方法.
			// Normally, the latter 的se two 将 written as "StatInheritanceData.None", rather than being typed out by hand...
			// ...but 对于 sake of clarity, we've written it out and labeled each 参数 in 顺序; they 应该 self-explanatory.
			// 要 explain how these 返回 values work, each one behaves like a 百分比, with 0f being 0%, 1f being 100%, and so on.
			// 返回 值 indicates how much your 类 will 缩放 off 的 stat in question for whatever 伤害 类(es) you've returned it for.
			// 如果 you create a StatInheritanceData without 任何 parameters, all 的m 将 set to 1f.
			// 对于 example, if we propose a hypothetical alternate 返回 for DamageClass.Ranged...
			/*
			if (damageClass == DamageClass.Ranged)
				return new StatInheritanceData(
					damageInheritance: 1f,
					critChanceInheritance: -1f,
					attackSpeedInheritance: 0.4f,
					armorPenInheritance: 2.5f,
					knockbackInheritance: 0f
				);
			*/
			// This would 允许 our custom 类 to benefit 从 following ranged stat bonuses:
			// - 伤害, at 100% effectiveness
			// - 攻击 速度, at 40% effectiveness
			// - Crit 概率, at -100% effectiveness (this means 任何thing that raises ranged crit 概率 specifically will lower the crit 概率 of our custom 类 by the same amount)
			// - 护甲 penetration, at 250% effectiveness

			// CAUTION: There is no hardcap on what you can set these to. Please be aware and advised that whatever you set them to may have unintended consequences,
			// and that we are NOT responsible for 任何 temporary or permanent 伤害 ca用于 you, your character, or your 世界 因此 of your morbid curiosity.
			// 要 refer to a non-vanilla 伤害 类 对于se sorts of things, use "ModContent.GetInstance<TargetDamageClassHere>()" 代替 "DamageClass.XYZ".
		}

		public override bool GetEffectInheritance(DamageClass damageClass) {
			// 此方法 allows you to make your 伤害 类 benefit from and be 能够 激活 other classes' effects (e.g. Spectre bolts, Magma Stone) 基于 what returns 真.
			// 注意 that unlike our stat inheritance methods up above, you do not 需要 account for universal bonuses in this 方法.
			// 对于 this example, we'll make our 类 能够 激活 melee- and magic-specifically effects.
			if (damageClass == DamageClass.Melee)
				return true;
			if (damageClass == DamageClass.Magic)
				return true;

			return false;
		}

		public override void SetDefaultStats(Player player) {
			// 此方法 lets you set default statistical modifiers for your example 伤害 类.
			// Here, we'll make our example 伤害 类 have more critical strike 概率 and 护甲 penetration than normal.
			player.GetCritChance<ExampleDamageClass>() += 4;
			player.GetArmorPenetration<ExampleDamageClass>() += 10;
			// These sorts of modifiers also exist for 伤害 (GetDamage), knockback (GetKnockback), and 攻击 速度 (GetAttackSpeed).
			// You'll see these used all around in 引用 to vanilla classes and our example 类 here. Familiarize yourself 与m.
		}

		// This 属性 lets you decide whether or not your 伤害 类 can use standard critical strike calculations.
		// 注意 that 设置 it to 假 will also 防止 the critical strike 概率 工具提示 line from being shown.
		// This prevention will overrule 任何thing set by ShowStatTooltipLine, so be careful!
		public override bool UseStandardCritCalcs => true;

		public override bool ShowStatTooltipLine(Player player, string lineName) {
			// 此方法 lets you 防止 certain common statistical 工具提示 lines from appearing on items associated with this DamageClass.
			// four line names you can use are "伤害", "CritChance", "速度", and "Knockback". All four cases default to 真, and 因此 将 shown. 例如...
			if (lineName == "Speed")
				return false;

			return true;
			// PLEASE BE AWARE that this hook will 不 here forever; only until an upcoming revamp to tooltips as a whole comes around.
			// Once this happens, a better, more versatile explanation of how to pull this off 将 showcased, and this hook 将 removed.
		}
	}
}