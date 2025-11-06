using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace ExampleMod.Content.Rarities
{
	public class ExampleModRarity : ModRarity
	{
		public override Color RarityColor => new Color(200, 215, 230);

		public override int GetPrefixedRarity(int offset, float valueMult) {
			if (offset > 0) { // If the 偏移 is 1 or 2 (a positive 修饰符).
				return ModContent.RarityType<ExampleHigherTierModRarity>(); // 使 the 稀有度 of items that have this 稀有度 with a positive 修饰符 the higher tier one.
			}

			return Type; // no 'lower' tier to go to, so 返回 the 类型 of this 稀有度.
		}
	}
}
