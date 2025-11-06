using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	public class ExampleSoul : ModItem
	{
		public override void SetStaticDefaults() {
			// 注册 a vertical animation with 4 frames and each one will last 5 ticks (1/12 second)
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true; // 使 the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation

			ItemID.Sets.ItemIconPulse[Item.type] = true; // The item pulses while 在 player's inventory
			ItemID.Sets.ItemNoGravity[Item.type] = true; // 使 the item have no gravity

			Item.ResearchUnlockCount = 25; // Configure the amount of this item that's needed to research it in Journey mode.
		}

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 1000; // 使 the item worth 1 gold.
			Item.rare = ItemRarityID.Orange;
		}

		public override void PostUpdate() {
			Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // 使 this item glow when thrown out of inventory.
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
