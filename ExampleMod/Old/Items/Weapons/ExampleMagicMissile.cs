using ExampleMod.Tiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.Items.Weapons
{
	public class ExampleMagicMissile : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("This magic weapon shoots missiles that follow your cursor."
				+ "\nIncreased mana usage during the day, decreased mana usage at night.");
		}

		public override void SetDefaults() {
			item.damage = 25;
			item.magic = true;
			item.mana = 14;
			item.width = 26;
			item.height = 26;
			item.useTime = 15;
			item.useAnimation = 15;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.noMelee = true;
			item.channel = true; //通道 以便 you can held the 武器 [Important]
			item.knockBack = 8;
			item.value = Item.sellPrice(silver: 50);
			item.rare = ItemRarityID.Orange;
			item.UseSound = SoundID.Item9;
			item.shoot = ProjectileType<Projectiles.MagicMissile>();
			item.shootSpeed = 10f;
		}

		// This 项's 魔力 usage changes through the day, peaking at 1.5x 魔力 usage at noon, and 0.5x 魔力 usage at midnight.
		// Thanks to chikenbones 对于 帮助 在 calculations
		public override void ModifyManaCost(Player player, ref float reduce, ref float mult) {
			double currentTime = Main.time;
			// The 时间 at which it changes from day to night and vice versa.
			double maxTime = Main.dayTime ? Main.dayLength : Main.nightLength;
			// More 魔力 during day, less at night
			int direction = Main.dayTime ? 1 : -1;
			// Sine goes from 0 to 1 to 0 over a period of pi, so we 匹配 that 到 长度 的 day/night.
			float timeMult = (float)Math.Sin(currentTime / maxTime * Math.PI);
			// Then we multiply by 方向 so it goes between 1 and -1 through the entire day, then multiply by 0.5 and add 1 to make it go between 1.5 and 0.5.
			timeMult = 1 + timeMult * direction * 0.5f;
			// Last, we multiply the current 魔力 成本 乘数 的 项 by our 乘数.
			mult *= timeMult;
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<ExampleItem>(), 20);
			recipe.AddTile(TileType<ExampleWorkbench>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}