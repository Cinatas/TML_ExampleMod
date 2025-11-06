using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Items.ExampleDamageClass
{
	// This 类 handles 每个thing for our custom 伤害 类
	// Any 类 that we wish to be using our custom 伤害 类 will derive from this 类, 代替 ModItem
	public abstract class ExampleDamageItem : ModItem
	{
		public override bool CloneNewInstances => true;

		// 自定义 items should override this to set their defaults
		public virtual void SafeSetDefaults() {
		}

		// By making the override sealed, we 防止 derived classes from further overriding the 方法 and enforcing the use of SafeSetDefaults()
		// We do this to ensure th在 vanilla 伤害 types are always set to 假, which makes the custom 伤害 类型 work
		public sealed override void SetDefaults() {
			SafeSetDefaults();
			// all vanilla 伤害 types 必须 假 for custom 伤害 types to work
			item.melee = false;
			item.ranged = false;
			item.magic = false;
			item.thrown = false;
			item.summon = false;
		}

		// As a modder, you could also opt to make these overrides also sealed. Up 到 modder
		public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat) {
			add += ExampleDamagePlayer.ModPlayer(player).exampleDamageAdd;
			mult *= ExampleDamagePlayer.ModPlayer(player).exampleDamageMult;
		}

		public override void GetWeaponKnockback(Player player, ref float knockback) {
			// 添加s knockback bonuses
			knockback += ExampleDamagePlayer.ModPlayer(player).exampleKnockback;
		}

		public override void GetWeaponCrit(Player player, ref int crit) {
			// 添加s crit bonuses
			crit += ExampleDamagePlayer.ModPlayer(player).exampleCrit;
		}

		// Because we want the 伤害 工具提示 to show our custom 伤害, we 需要 modify it
		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			// 获取 the vanilla 伤害 工具提示
			TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.mod == "Terraria");
			if (tt != null) {
				// 我们想要 to grab the last word 的 工具提示, 即 the translated word for '伤害' (取决于 what language the 玩家 is using)
				// So we 拆分 the 字符串 by whitespace, and grab the last word 从 returned arrays to get the 伤害 word, and the first to get the 伤害 shown 在 工具提示
				string[] splitText = tt.text.Split(' ');
				string damageValue = splitText.First();
				string damageWord = splitText.Last();
				// 更改 the 工具提示 文本
				tt.text = damageValue + " example " + damageWord;
			}
		}
	}
}
