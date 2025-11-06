using ExampleMod.Items;
using ExampleMod.Tiles;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.Projectiles
{
	public class ExampleBehindTilesProjectile : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_3";

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Ghost Shuriken");
			ProjectileID.Sets.DontAttachHideToAlpha[projectile.type] = true; // projectiles with hide but without this will draw 在 lighting values 的 owner player.
		}

		public override void SetDefaults() {
			projectile.CloneDefaults(ProjectileID.Shuriken);
			aiType = ProjectileID.Shuriken;
			projectile.hide = true; // 防止s projectile from being drawn normally. Use in conjunction with DrawBehind.
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.timeLeft = 60;
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI) {
			// 添加 this projectile 到 list of projectiles that 将 drawn BEFORE tiles and NPC are drawn. This makes the projectile appear to be BEHIND the tiles and NPC.
			drawCacheProjsBehindNPCsAndTiles.Add(index);
		}
	}
	// This .cs file has 2 classes in it, 即 totally fine. (What is important is that namespace+classname is unique. Remember that autoloaded textures follow the namespace+classname convention 以及.)
	// This is an approach you can take to fit your organization style.
	public class ExampleBehindTilesProjectileItem : ModItem
	{
		// 使用 this to use Vanilla textures. The number corresponds 到 ItemID 的 vanilla item.
		public override string Texture => "Terraria/Item_42";

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Ghost Shuriken");
		}

		public override void SetDefaults() {
			item.CloneDefaults(ItemID.Shuriken);
			item.shoot = ProjectileType<ExampleBehindTilesProjectile>();
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Shuriken, 10);
			recipe.AddIngredient(ItemType<ExampleItem>(), 1);
			recipe.AddTile(TileType<ExampleWorkbench>());
			recipe.SetResult(this, 10);
			recipe.AddRecipe();
		}
	}
}
