using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	// 示例WaterTorch is very similar to ExampleTorch, except it 可以 used and placed underwater, similar to Coral Torch.
	// comments in this 文件 will focus 在 differences.
	// Both place the same 图格, but a different 图格 style. The ExampleWaterTorch 图格 style has custom code seen 在 ExampleTorch ModTile.
	public class ExampleWaterTorch : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 100;

			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
			ItemID.Sets.SingleUseInGamepad[Type] = true;
			ItemID.Sets.Torches[Type] = true;
			ItemID.Sets.WaterTorches[Type] = true; // The TileObjectData.newSubTile code 在 ExampleTorch ModTile is required 以及 to make a water torch.
		}

		public override void SetDefaults() {
			// 代替 of placing style 0, style 1 is placed. The allowWaterPlacement 参数 is 真, which will set 项.noWet to 假, allowing the 项 to be held underwater.
			Item.DefaultToTorch(ModContent.TileType<Tiles.ExampleTorch>(), 1, true);
			Item.value = 50;
		}

		public override void HoldItem(Player player) {
			if (Main.rand.NextBool(player.itemAnimation > 0 ? 7 : 30)) {
				Dust dust = Dust.NewDustDirect(new Vector2(player.itemLocation.X + (player.direction == -1 ? -16f : 6f), player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<Sparkle>(), 0f, 0f, 100);
				if (!Main.rand.NextBool(3)) {
					dust.noGravity = true;
				}

				dust.velocity *= 0.3f;
				dust.velocity.Y -= 1.5f;
				dust.position = player.RotatedRelativePoint(dust.position);
			}

			// 创建 a greenish (0.5, 1.5, 0.5) light 在 torch's approximate 位置, when the 项 is held.
			Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

			Lighting.AddLight(position, 0.5f, 1.5f, 0.5f);
		}

		public override void PostUpdate() {
			// 创建 a greenish (0.5, 1.5, 0.5) light when the 项 is in 世界, even if underwater.
			Lighting.AddLight(Item.Center, 0.5f, 1.5f, 0.5f);
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleTorch>()
				.AddIngredient(ItemID.Gel)
				.Register();
		}
	}
}
