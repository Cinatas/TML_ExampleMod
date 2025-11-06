using ExampleMod.Content.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Pets.ExamplePet
{
	public class ExamplePetItem : ModItem
	{
		// Names and descriptions of all ExamplePetX classes are defined using .hjson files 在 Localization 文件夹
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.ZephyrFish); // 复制 the Defaults 的 Zephyr Fish 项.

			Item.shoot = ModContent.ProjectileType<ExamplePetProjectile>(); // "Shoot" your 宠物 弹幕.
			Item.buffType = ModContent.BuffType<ExamplePetBuff>(); // 应用 增益 upon usage 的 项.
		}

        public override bool? UseItem(Player player)
        {
			if (player.whoAmI == Main.myPlayer) {
				player.AddBuff(Item.buffType, 3600);
			}
   			return true;
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
