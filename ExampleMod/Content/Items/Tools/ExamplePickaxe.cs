using ExampleMod.Content.Dusts;
using ExampleMod.Content.EmoteBubbles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Tools
{
	public class ExamplePickaxe : ModItem
	{
		public override void SetDefaults() {
			Item.damage = 20;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(gold: 1); // 购买 this 项 for one 金币 - change 金币 to any 硬币 and change the 值 to any 数字 <= 100
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;

			Item.pick = 220; // How strong the pickaxe is, see https://terraria.wiki.gg/wiki/Pickaxe_power for a 列表 of common values
			Item.attackSpeedOnlyAffectsWeaponAnimation = true; // Melee 速度 affects how fast the tool swings for 伤害 purposes, but not how fast it can dig
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) {
			if (Main.rand.NextBool(10)) {
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<ExampleCustomDrawDust>());
			}
		}

		public override void UseAnimation(Player player) {
			// Randomly causes the 玩家 to use Example Pickaxe Emote when using the 项
			if (Main.myPlayer == player.whoAmI && player.ItemTimeIsZero && Main.rand.NextBool(60)) {
				EmoteBubble.MakePlayerEmote(player, ModContent.EmoteBubbleType<ExamplePickaxeEmote>());
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
