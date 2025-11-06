using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Content.Tiles.Furniture
{
	public class ExampleClock : ModTile
	{
		public override void SetStaticDefaults() {
			// Properties
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.Clock[Type] = true;

			DustType = ModContent.DustType<Sparkle>();
			AdjTiles = new int[] { TileID.GrandfatherClocks };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16 };
			TileObjectData.addTile(Type);

			// Etc
			AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.GrandfatherClock"));
		}

		public override bool RightClick(int x, int y) {
			string text = "AM";
			// 获取 current weird 时间
			double time = Main.time;
			if (!Main.dayTime) {
				// if it's night add this 数字
				time += 54000.0;
			}

			// Divide by seconds in a day * 24
			time = (time / 86400.0) * 24.0;
			// Dunno why we're taking 19.5. Something about hour formatting
			time = time - 7.5 - 12.0;
			// 格式 in readable 时间
			if (time < 0.0) {
				time += 24.0;
			}

			if (time >= 12.0) {
				text = "PM";
			}

			int intTime = (int)time;
			// 获取 the decimal points of 时间.
			double deltaTime = time - intTime;
			// multiply them by 60. Minutes, probably
			deltaTime = (int)(deltaTime * 60.0);
			// This could easily be replaced by deltaTime.ToString()
			string text2 = string.Concat(deltaTime);
			if (deltaTime < 10.0) {
				// if deltaTime is eg "1" (which would cause 时间 to 显示 as HH:M 代替 HH:MM)
				text2 = "0" + text2;
			}

			if (intTime > 12) {
				// 这是 for AM/PM 时间 rather than 24hour 时间
				intTime -= 12;
			}

			if (intTime == 0) {
				// 0AM = 12AM
				intTime = 12;
			}

			// Whack it all together to get a HH:MM 格式
			Main.NewText($"Time: {intTime}:{text2} {text}", 255, 240, 20);
			return true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}
	}
}