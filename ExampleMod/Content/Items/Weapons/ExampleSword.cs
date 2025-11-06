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
			Item.width = 40; // The item texture's width.
			Item.height = 40; // The item texture's height.

			Item.useStyle = ItemUseStyleID.Swing; // The useStyle 的 Item.
			Item.useTime = 20; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
			Item.useAnimation = 20; // The time span 的 using animation 的 weapon, suggest setting it the same as useTime.
			Item.autoReuse = true; // Whether the weapon 可以 used more than once automatically by holding the use button.

			Item.DamageType = DamageClass.Melee; // Whether your item is part 的 melee class.
			Item.damage = 50; // The damage your item deals.
			Item.knockBack = 6; // The force of knockback 的 weapon. Maximum is 20
			Item.crit = 6; // The critical strike chance the weapon has. The player, 默认情况下, has a 4% critical strike chance.

			Item.value = Item.buyPrice(gold: 1); // The value 的 weapon in copper coins.
			Item.rare = ModContent.RarityType<ExampleModRarity>(); // Give this item our custom rarity.
			Item.UseSound = SoundID.Item1; // The sound when the weapon is being used.
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) {
			if (Main.rand.NextBool(3)) {
				// Emit dusts when the sword is swung
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Dusts.Sparkle>());
			}
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
			// Inflict the OnFire debuff for 1 second onto any NPC/Monster that this hits.
			// 60 frames = 1 second
			target.AddBuff(BuffID.OnFire, 60);
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
