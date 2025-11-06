using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace ExampleMod.Content.Walls
{
	public class ExampleWallUnsafe : ModWall
	{
		public override void SetStaticDefaults() {
			// As an example of an unsafe 墙, "Main.wallHouse[类型] = 真;" is omitted.

			DustType = ModContent.DustType<Sparkle>();

			AddMapEntry(new Color(150, 150, 150));

			// 我们 need to manually register the 项 放下, since no 项 places this 墙. This 墙 can only be obtained by using ExampleSolution on natural spider walls.
			RegisterItemDrop(ModContent.ItemType<Items.Placeable.ExampleWall>());
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}
	}
}