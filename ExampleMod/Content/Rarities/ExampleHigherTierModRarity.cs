using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Rarities
{
	public class ExampleHigherTierModRarity : ModRarity
	{
		public override Color RarityColor => new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));

		public override int GetPrefixedRarity(int offset, float valueMult) {
			if (offset < 0) { // If the 偏移 is -1 or -2 (a negative 修饰符).
				return ModContent.RarityType<ExampleModRarity>(); // 使 the 稀有度 of items that have this 稀有度 with a negative 修饰符 the lower tier one.
			}

			return Type; // no 'higher' tier to go to, so 返回 the 类型 of this 稀有度.
		}
	}
}
