using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Items
{
	public class Infinity : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Never run out of anything!");
		}

		public override void SetDefaults() {
			item.width = 34;
			item.height = 18;
			item.accessory = true;
			item.value = Item.sellPrice(gold: 2);
			item.rare = ItemRarityID.Lime;
			item.expert = true;
			item.expertOnly = true; // 使 it so the 项's 饰品 effects only work in expert 模式
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetModPlayer<ExamplePlayer>().infinity = true;
		}

		public override Color? GetAlpha(Color lightColor) {
			return Color.White; // So the 项's 精灵 isn't affected by light
		}

		// This gives the 项 an outline that constantly changes 颜色
		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			Texture2D texture = Main.itemTexture[item.type];
			Vector2 position = item.position - Main.screenPosition + new Vector2(item.width / 2, item.height - texture.Height * 0.5f + 2f);
			// We redraw the 项's 精灵 4 times, 每次 shifted 2 pixels on each 方向, using Main.DiscoColor to give it the 颜色 changing 效果
			for (int i = 0; i < 4; i++) {
				Vector2 offsetPositon = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i) * 2;
				spriteBatch.Draw(texture, position + offsetPositon, null, Main.DiscoColor, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
			}
			// 返回 真 so the original 精灵 is drawn 右 after
			return true;
		}

		// Same as above but for drawing inside the 玩家's 库存
		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
			Texture2D texture = Main.itemTexture[item.type];
			for (int i = 0; i < 4; i++) {
				Vector2 offsetPositon = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i) * 2;
				spriteBatch.Draw(texture, position + offsetPositon, null, Main.DiscoColor, 0, origin, scale, SpriteEffects.None, 0f);
			}
			return true;
		}
	}

	public class InfinityGlobalItem : GlobalItem
	{
		public override bool ConsumeItem(Item item, Player player) {
			return !player.GetModPlayer<ExamplePlayer>().infinity;
		}

		public override bool ConsumeAmmo(Item item, Player player) {
			return !player.GetModPlayer<ExamplePlayer>().infinity;
		}

		// Replenishes the 魔力 的 玩家 h一旦 they need some, by exactly the amount they need, and stops the 魔力 flower from triggering.
		public override void OnMissingMana(Item item, Player player, int neededMana) {
			if (player.GetModPlayer<ExamplePlayer>().infinity) {
				player.statMana += neededMana;
			}
		}
	}
}
