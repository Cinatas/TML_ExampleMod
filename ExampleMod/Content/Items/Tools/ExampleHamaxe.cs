using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Tools
{
	public class ExampleHamaxe : ModItem
	{
		public override void SetDefaults() {
			Item.damage = 25;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true; // Automatically re-swing/re-use this 项 after its swinging 动画 is over.

			Item.axe = 30; // How much axe power the 武器 has, note th在 axe power displayed in-game is this 值 multiplied by 5
			Item.hammer = 100; // How much hammer power the 武器 has
			Item.attackSpeedOnlyAffectsWeaponAnimation = true; // Melee 速度 affects how fast the tool swings for 伤害 purposes, but not how fast it can dig
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) {
			if (Main.rand.NextBool(10)) { // This creates a 1/10 概率 that a dust will 生成 every 帧 that this 项 is in its 'Swinging' 动画.
				// 创建s a dust 在 hitbox rectangle, following the rules of our 'if' conditional.
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Sparkle>());
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
