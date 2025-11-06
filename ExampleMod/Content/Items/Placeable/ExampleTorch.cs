using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	public class ExampleTorch : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 100;

			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
			ItemID.Sets.SingleUseInGamepad[Type] = true;
			ItemID.Sets.Torches[Type] = true;
		}

		public override void SetDefaults() {
			// 默认ToTorch sets various properties common to torch placing items. 悬停 over DefaultToTorch in Visual Studio to see the specific properties set.
			// Of particular note to torches are 项.holdStyle, 项.flame, and 项.noWet. 
			Item.DefaultToTorch(ModContent.TileType<Tiles.ExampleTorch>(), 0, false);
			Item.value = 50;
		}

		public override void HoldItem(Player player) {
			// This torch can不 used in water, so it shouldn't 生成 particles or light either
			if (player.wet) {
				return;
			}

			// 注意 that due to 生物群系 select torch god's favor, the 玩家 may not actually have an ExampleTorch 在ir 库存 when this hook is called, so no modifications 应该 made 到 项 实例.

			// Randomly 生成 sparkles when the torch is held. Bigger 概率 to 生成 them when swinging the torch.
			if (Main.rand.NextBool(player.itemAnimation > 0 ? 7 : 30)) {
				Dust dust = Dust.NewDustDirect(new Vector2(player.itemLocation.X + (player.direction == -1 ? -16f : 6f), player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<Sparkle>(), 0f, 0f, 100);
				if (!Main.rand.NextBool(3)) {
					dust.noGravity = true;
				}

				dust.velocity *= 0.3f;
				dust.velocity.Y -= 1.5f;
				dust.position = player.RotatedRelativePoint(dust.position);
			}

			// 创建 a white (1.0, 1.0, 1.0) light 在 torch's approximate 位置, when the 项 is held.
			Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

			Lighting.AddLight(position, 1f, 1f, 1f);
		}

		public override void PostUpdate() {
			// 创建 a white (1.0, 1.0, 1.0) light when the 项 is in 世界, and isn't underwater.
			if (!Item.wet) {
				Lighting.AddLight(Item.Center, 1f, 1f, 1f);
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				// .SortAfterFirstRecipesOf(ItemID.Torch) Uncomment this line to have this 配方 appear after the Torch 配方. 
				.Register();
		}
	}
}
