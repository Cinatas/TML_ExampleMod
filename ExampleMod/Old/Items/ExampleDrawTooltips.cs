using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;

namespace ExampleMod.Items
{
	internal class ExampleDrawTooltips : ModItem
	{
		public override string Texture => "Terraria/Item_3617";

		public override void SetStaticDefaults() {
			Tooltip.SetDefault("This item showcases various Draw Tooltip hooks");
		}

		public override void SetDefaults() {
			item.width = 20;
			item.height = 20;
			item.maxStack = 999;
			item.value = 100;
			item.rare = ItemRarityID.Blue;
		}

		private Vector2 boxSize; // 存储 the 大小 of our 工具提示 box
		private const int paddingForBox = 10;

		public override bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y) {
			// 你可以 偏移 the entire 工具提示 by changing x and y
			// 你可以 actually have the entire 工具提示 draw somewhere else, x and y is where the 工具提示 starts drawing

			// 绘制 a magic box for this 工具提示
			// From all tooltips we select their texts
			var texts = lines.Select(z => z.text);
			// 计算 our 宽度 对于 box, which 将 the 宽度 的 longest 文本, plus some 填充. This code takes into account Snippets and character widths.
			int widthForBox = texts.Max(t => (int)ChatManager.GetStringSize(Main.fontMouseText, t, Vector2.One).X) + paddingForBox * 2;
			// 计算 our 高度 对于 box, which 将 the sum 的 文本 heights, plus some 填充
			int heightForBox = (int)texts.ToList().Sum(z => Main.fontMouseText.MeasureString(z).Y) + paddingForBox * 2;
			// 设置 our boxSize to our calculated 大小, now we can use this elsewhere too
			boxSize = new Vector2(widthForBox, heightForBox);

			// 我们将 开始 drawing the box slightly 偏移 to accommodate for 填充
			Vector2 drawPosForBox = new Vector2(x - paddingForBox, y - paddingForBox);
			Rectangle drawRectForBox = new Rectangle((int)drawPosForBox.X, (int)drawPosForBox.Y, widthForBox, heightForBox);
			// 绘制 the magic box
			Main.spriteBatch.Draw(Main.magicPixel, drawRectForBox, Main.mouseTextColorReal);

			return true;
		}

		public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) {
			if (!line.oneDropLogo) {
				// You are not 允许 change these, modders should use ModifyTooltips to modify them
				//line.文本 = "you shall not pass...";
				//line.oneDropLogo = 假;
				//line.颜色 = 颜色.AliceBlue;
				//line.overrideColor = 颜色.AliceBlue;
				//line.isModifier = 假;
				//line.isModifierBad = 假;
				//line.索引 = 1;

				// Let's draw the 项 名称 centered so it's 在 middle, and let's add a form of separator
				string sepText = "-----"; // This is our separator, which will go between the 项 名称 and the rest
				float sepHeight = line.font.MeasureString(sepText).Y; // 高度 of our separator

				// If our line 文本 equals our 项 名称, this is our 工具提示 line 对于 项 名称
				// if (line.文本 == 项.HoverName)
				// What is more accurate to check is the 层 名称 and mod
				if (line.Name == "ItemName" && line.mod == "Terraria")
				// We check for Terraria so we modify the vanilla 工具提示 and not a modded one
				// This 可能 important, in case some mod does 很多 custom work and removes the standard 工具提示
				// For 工具提示 layers, check the documentation for TooltipLine
				{
					// Our 偏移 is half the 宽度 of our box, minus the 填充 of one side
					float boxOffset = boxSize.X / 2 - paddingForBox;
					// The X 坐标 where we draw is where the line would draw, plus the box 偏移,
					// which would place the 开始 的 字符串 在 中心, so we subtract half 的 line 宽度 to 中心 it completely
					float drawX = line.X + boxOffset - line.font.MeasureString(sepText).X / 2;
					float drawY = line.Y + sepHeight / 2;

					// Note how our line 对象 has m任何 properties we can use for drawing
					// Here we draw the separator, note that it'd make more sense to use PostDraw for this, but 任一 will work
					ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.font, sepText,
						new Vector2(drawX, drawY), line.color, line.rotation, line.origin, line.baseScale, line.maxWidth, line.spread);

					// Here we do the same thing as we did for drawX, which will 中心 our ItemName 工具提示
					line.X += (int)boxOffset - (int)line.font.MeasureString(line.text).X / 2;
					// yOffset affects the 偏移 即 added 每个 next line, so this will cause the line to come after the separator to be drawn slightly lower
					yOffset = (int)sepHeight / 4;
				}
				else {
					// 重置 the 偏移 for other lines
					yOffset = 0;
				}
			}
			return true;
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<ExampleItem>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
