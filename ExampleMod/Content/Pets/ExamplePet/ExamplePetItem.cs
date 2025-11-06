using ExampleMod.Content.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Pets.ExamplePet
{
	public class ExamplePetItem : ModItem
	{
		// Names and descriptions of all ExamplePetX classes are defined using .hjson files 在 Localization folder
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.ZephyrFish); // Copy the Defaults 的 Zephyr Fish Item.

			Item.shoot = ModContent.ProjectileType<ExamplePetProjectile>(); // "Shoot" your pet projectile.
			Item.buffType = ModContent.BuffType<ExamplePetBuff>(); // 应用 buff upon usage 的 Item.
		}

        public override bool? UseItem(Player player)
        {
			if (player.whoAmI == Main.myPlayer) {
				player.AddBuff(Item.buffType, 3600);
			}
   			return true;
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
