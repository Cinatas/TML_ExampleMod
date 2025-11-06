using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Tools
{
	// Magic Mirror is one 的 only vanilla items that does its action somewhere other than the 开始 of its 动画, 即 why we use code in UseStyle NOT UseItem.
	// 它可能 prove a useful guide for ModItems with similar behaviors.
	internal class ExampleMagicMirror : ExampleItem
	{
		private static readonly Color[] itemNameCycleColors = {
			new Color(254, 105, 47),
			new Color(190, 30, 209),
			new Color(34, 221, 151),
			new Color(0, 106, 185),
		};

		public override string Texture => $"Terraria/Images/Item_{ItemID.IceMirror}"; // Copies the 纹理 对于 Ice Mirror, make your own 纹理 if need be.

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.IceMirror); // Copies the defaults 从 Ice Mirror.
			Item.color = Color.Violet; // 设置s the 项 颜色
		}

		// 使用Style is called each 帧 th在 项 is being actively used.
		public override void UseStyle(Player player, Rectangle heldItemFrame) {
			// Each 帧, make some dust
			if (Main.rand.NextBool()) {
				Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, Color.White, 1.1f); // 使 dust 从 玩家's 位置 and copies the hitbox of which the dust may 生成. Change these arguments 如果需要.
			}

			// This sets up the itemTime correctly.
			if (player.itemTime == 0) {
				player.ApplyItemTime(Item);
			}
			else if (player.itemTime == player.itemTimeMax / 2) {
				// This code runs once halfway through the useTime 的 项. You'll notice with magic mirrors you are still holding the 项 for 一点 bit after you've teleported.

				// 使 dust 70 times for a cool 效果.
				for (int d = 0; d < 70; d++) {
					Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 150, default, 1.5f);
				}

				// This code releases all grappling hooks and kills/despawns them.
				player.RemoveAllGrapplingHooks();

				// actual 方法 that moves the 玩家 back to bed/生成.
				player.Spawn(PlayerSpawnContext.RecallFromItem);

				// 使 dust 70 times for a cool 效果. This dust is the dust 在 destination.
				for (int d = 0; d < 70; d++) {
					Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, default, 1.5f);
				}
			}
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			// This code shows using 颜色.Lerp,  Main.GameUpdateCount, and the modulo operator (%) to do a neat 效果 cycling between 4 custom colors.
			int numColors = itemNameCycleColors.Length;

			foreach (TooltipLine line2 in tooltips) {
				if (line2.Mod == "Terraria" && line2.Name == "ItemName") {
					float fade = (Main.GameUpdateCount % 60) / 60f;
					int index = (int)((Main.GameUpdateCount / 60) % numColors);
					int nextIndex = (index + 1) % numColors;

					line2.OverrideColor = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);
				}
			}
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
