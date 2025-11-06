using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Items.ExampleDamageClass
{
	// This class handles everything for our custom damage class
	// Any class that we wish to be using our custom damage class will derive from this class, instead of ModItem
	public abstract class ExampleDamageItem : ModItem
	{
		public override bool CloneNewInstances => true;

		// 自定义 items should override this to set their defaults
		public virtual void SafeSetDefaults() {
		}

		// By making the override sealed, we prevent derived classes from further overriding the method and enforcing the use of SafeSetDefaults()
		// We do this to ensure th在 vanilla damage types are always set to false, which makes the custom damage type work
		public sealed override void SetDefaults() {
			SafeSetDefaults();
			// all vanilla damage types 必须 false for custom damage types to work
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

		// Because we want the damage tooltip to show our custom damage, we need to modify it
		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			// 获取 the vanilla damage tooltip
			TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.mod == "Terraria");
			if (tt != null) {
				// We want to grab the last word 的 tooltip, 即 the translated word for 'damage' (depending on what language the player is using)
				// So we split the string by whitespace, and grab the last word 从 returned arrays to get the damage word, and the first to get the damage shown 在 tooltip
				string[] splitText = tt.text.Split(' ');
				string damageValue = splitText.First();
				string damageWord = splitText.Last();
				// 更改 the tooltip text
				tt.text = damageValue + " example " + damageWord;
			}
		}
	}
}
