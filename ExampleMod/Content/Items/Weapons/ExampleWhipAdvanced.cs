using ExampleMod.Content.Buffs;
using ExampleMod.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleWhipAdvanced : ModItem
	{
		// texture doesn't have the same name as the item, so this property points to it.
		public override string Texture => "ExampleMod/Content/Items/Weapons/ExampleWhip";

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExampleWhipAdvancedDebuff.TagDamagePercent);

		public override void SetDefaults() {
			// 调用 this method to quickly set some 的 properties below.
			//Item.DefaultToWhip(ModContent.ProjectileType<ExampleWhipProjectileAdvanced>(), 20, 2, 4);

			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.damage = 20;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Green;

			Item.shoot = ModContent.ProjectileType<ExampleWhipProjectileAdvanced>();
			Item.shootSpeed = 4;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.UseSound = SoundID.Item152;
			Item.channel = true; // This is used 对于 charging functionality. Remove it if your whip shouldn't be chargeable.
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}

		// 使 the whip receive melee prefixes
		public override bool MeleePrefix() {
			return true;
		}
	}
}
