using ExampleMod.Content.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	/// <summary>
	/// This 武器 and its corresponding 弹幕 showcase the CloneDefaults() 方法, which allows for cloning of other items.
	/// For this example, we shall 复制 the Meowmere and its projectiles 与 CloneDefaults() 方法, while also changing them slightly.
	/// For a more detailed 描述 of each 项 字段 used here, check out <see cref="ExampleSword" />.
	/// </summary>
	public class ExampleCloneWeapon : ModItem
	{
		public override void SetDefaults() {
			// 此方法 右 here is the backbone of what we're doing here; by using this 方法, we 复制 所有
			// the meowmere's SetDefault stats (例如 项.melee and 项.shoot) on to our 项, so we don't 必须
			// go in到 source and 复制 the stats ourselves. It saves 很多 时间 and looks much cleaner; if you're
			// 将要 复制 the stats of an 项, use CloneDefaults().

			Item.CloneDefaults(ItemID.Meowmere);

			// 之后 CloneDefaults has been called, we can now modify the stats to our wishes, or keep them as they are.
			// 对于 the sake of example, let's swap the vanilla Meowmere 弹幕 shot from our 项 for our own 弹幕 by changing 项.shoot:

			Item.shoot = ModContent.ProjectileType<ExampleCloneProjectile>(); // 记住 that we must use ProjectileType<>() since it is a modded 弹幕!
			// 检查 out ExampleCloneProjectile to see how this 弹幕 is different 从 Vanilla Meowmere 弹幕.

			// While we're at it, let's make our 武器's stats a bit stronger than the Meowmere, which 可以 done
			// by using math on each given stat.

			Item.damage *= 2; // 使 this 武器's 伤害 double the Meowmere's 伤害.
			Item.shootSpeed *= 1.25f; // 使 this 武器's projectiles shoot 25% faster than the Meowmere's projectiles.
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
