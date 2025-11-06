using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	public class ExampleHairDye : ModItem
	{
		public override void SetStaticDefaults() {
			// 避免 loading assets on dedicated servers. They don't use graphics cards.
			if (!Main.dedServ) {
				// following code creates a hair 颜色-returning delegate (anonymous 方法), and associates it with this 项's 类型 ID.
				GameShaders.Hair.BindShader(
					Item.type,
					new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Main.DiscoColor) // 返回ing Main.DiscoColor will make our hair an animated rainbow. You can 返回 任何 颜色 here.
				);
			}

			Item.ResearchUnlockCount = 3;
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 26;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item3;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTurn = true;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.consumable = true;
		}
	}
}