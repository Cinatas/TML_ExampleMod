using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ExampleMod.Common
{
	public static class ExampleRecipeCallbacks
	{
		// ConsumeItemCallbacks - These are used to adjust the 数字 of ingredients consumed by recipes, similar to Alchemy 表格 - See https://github.com/tModLoader/tModLoader/wiki/Intermediate-Recipes#custom-项-consumption
		public static void DontConsumeChain(Recipe recipe, int type, ref int amount) {
			if (type == ItemID.Chain) {
				amount = 0;
			}
		}
		// 其他 ConsumeItemCallback 方法...

		// OnCraftCallbacks - These are used to run code after a 配方 is crafted - See https://github.com/tModLoader/tModLoader/wiki/Intermediate-Recipes#custom-配方-craft-behavior
		public static void RandomlySpawnFireworks(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack) {
			if (Main.rand.NextBool(3)) {
				int fireworkProjectile = ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4);
				Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), Main.LocalPlayer.Top, new Microsoft.Xna.Framework.Vector2(0, -Main.rand.NextFloat(2f, 4f)).RotatedByRandom(0.3f), fireworkProjectile, 0, 0, Main.myPlayer);

				Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), ItemID.Confetti, 5);
			}
		}
		// 其他 OnCraftCallback 方法...
	}
}
