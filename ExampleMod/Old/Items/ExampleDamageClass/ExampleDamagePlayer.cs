using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Items.ExampleDamageClass
{
	// This 类 stores necessary 玩家 info for our custom 伤害 类, 例如 伤害 multipliers, additions to knockback and crit, and our custom 资源 that governs the usage 的 weapons of this 伤害 类.
	public class ExampleDamagePlayer : ModPlayer
	{
		public static ExampleDamagePlayer ModPlayer(Player player) {
			return player.GetModPlayer<ExampleDamagePlayer>();
		}

		// Vanilla only really has 伤害 multipliers in code
		// And crit and knockback is usually just added to
		// As a modder, you could make 分离 variables for multipliers and simple addition bonuses
		public float exampleDamageAdd;
		public float exampleDamageMult = 1f;
		public float exampleKnockback;
		public int exampleCrit;

		public override void ResetEffects() {
			ResetVariables();
		}

		public override void UpdateDead() {
			ResetVariables();
		}

		private void ResetVariables() {
			exampleDamageAdd = 0f;
			exampleDamageMult = 1f;
			exampleKnockback = 0f;
			exampleCrit = 0;
		}
	}
}
