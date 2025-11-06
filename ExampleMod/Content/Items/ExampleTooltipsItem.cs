using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	public class ExampleTooltipsItem : ModItem
	{
		public override void SetStaticDefaults() {
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(30, 4));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true; // 使 the 项 have an 动画 while in 世界 (not held.). Use in combination with RegisterItemAnimation

			ItemID.Sets.ItemNoGravity[Item.type] = true;
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 20;
			Item.value = Item.sellPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
		}

		public override Color? GetAlpha(Color lightColor) {
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			// 在这里 we add a tooltipline that will later be removed, showcasing how to 删除 tooltips from an 项
			var line = new TooltipLine(Mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Face", "I'm feeling just fine!") {
				OverrideColor = new Color(100, 100, 255)
			};
			tooltips.Add(line);

			// 在这里 we give the 项 名称 a rainbow 效果.
			foreach (TooltipLine line2 in tooltips) {
				if (line2.Mod == "Terraria" && line2.Name == "ItemName") {
					line2.OverrideColor = Main.DiscoColor;
				}
			}

			// 在这里 we will hide all tooltips whose 称号 结束 with ':RemoveMe'
			// One like 即 added 在 开始 of this 方法
			foreach (var l in tooltips) {
				if (l.Name.EndsWith(":RemoveMe")) {
					l.Hide();
				}
			}

			// Another 方法 of hiding 可以 done if you 想要 hide just one line.
			// tooltips.FirstOrDefault(x => x.Mod == "ExampleMod" && x.名称 == "Verbose:RemoveMe")?.Hide();
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}