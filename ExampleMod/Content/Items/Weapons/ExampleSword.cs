using ExampleMod.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleSword : ModItem
	{
		public override void SetDefaults() {
			Item.width = 40; // The 项 纹理's 宽度.
			Item.height = 40; // The 项 纹理's 高度.

			Item.useStyle = ItemUseStyleID.Swing; // The useStyle 的 项.
			Item.useTime = 20; // The 时间 span of 使用 武器. Remember in terraria, 60 frames is a second.
			Item.useAnimation = 20; // The 时间 span 的 using 动画 的 武器, suggest 设置 it the same as useTime.
			Item.autoReuse = true; // Whether the 武器 可以 used 超过 once automatically by holding the use 按钮.

			Item.DamageType = DamageClass.Melee; // Whether your 项 is part 的 melee 类.
			Item.damage = 50; // The 伤害 your 项 deals.
			Item.knockBack = 6; // The force of knockback 的 武器. 最大 is 20
			Item.crit = 6; // The critical strike 概率 the 武器 has. The 玩家, 默认情况下, has a 4% critical strike 概率.

			Item.value = Item.buyPrice(gold: 1); // The 值 的 武器 in 铜币 coins.
			Item.rare = ModContent.RarityType<ExampleModRarity>(); // Give this 项 our custom 稀有度.
			Item.UseSound = SoundID.Item1; // The 声音 when the 武器 is being used.
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) {
			if (Main.rand.NextBool(3)) {
				// Emit dusts when the sword is swung
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Dusts.Sparkle>());
			}
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
			// Inflict the OnFire 减益 for 1 second onto 任何 NPC/Monster that this hits.
			// 60 frames = 1 second
			target.AddBuff(BuffID.OnFire, 60);
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
