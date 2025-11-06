using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ExampleMod.Content.Projectiles.Rockets;

namespace ExampleMod.Content.Items.Ammo
{
	public class ExampleRocket : ModItem
	{
		// 火箭 Ammo is a little weird and does not work the same as bullets or arrows.
		// Rockets I through IV have four versions: normal 火箭, 手榴弹, Proximity 地雷, and Snowman 火箭.
		// 此示例 is a clone of 火箭 I.

		public override void SetStaticDefaults() {
			AmmoID.Sets.IsSpecialist[Type] = true; // This 项 将nefit 从 Shroomite Helmet.

			// 这是 where we tell the game which 弹幕 to 生成 when using this 火箭 as ammo with certain launchers.
			// This specific 火箭 ammo is like 火箭 I's.
			AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.RocketLauncher].Add(Type, ModContent.ProjectileType<ExampleRocketProjectile>());
			AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.GrenadeLauncher].Add(Type, ModContent.ProjectileType<ExampleGrenadeProjectile>());
			AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.ProximityMineLauncher].Add(Type, ModContent.ProjectileType<ExampleProximityMineProjectile>());
			AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.SnowmanCannon].Add(Type, ModContent.ProjectileType<ExampleSnowmanRocketProjectile>());
			// 我们 also need to say which 类型 of Celebration Mk2 rockets to use.
			// Celebration Mk 2 only has four types of rockets. Change the 弹幕 to 匹配 your ammo 类型.
			// 火箭 I like   == ProjectileID.Celeb2Rocket
			// 火箭 II like  == ProjectileID.Celeb2RocketExplosive
			// 火箭 III like == ProjectileID.Celeb2RocketLarge
			// 火箭 IV like  == ProjectileID.Celeb2RocketExplosiveLarge
			AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.Celeb2].Add(Type, ProjectileID.Celeb2Rocket);
			// Celebration and Electrosphere Launcher will always use their own projectiles no matter which 火箭 you use as ammo.
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 14;
			Item.damage = 40;
			Item.knockBack = 4f;
			Item.consumable = true;
			Item.DamageType = DamageClass.Ranged;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.buyPrice(copper: 50);
			Item.ammo = AmmoID.Rocket; // The ammo 类型 is 火箭 Ammo
			// 不像 other ammo, we don't set 项.shoot 到 弹幕 for 火箭 ammo due 到 logic involved.
			// AmmoID.Sets.SpecificLauncherAmmoProjectileMatches is used to determine the 弹幕 spawned based 在 武器.
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe(100)
				.AddIngredient(ItemID.RocketI, 100)
				.AddIngredient<ExampleItem>()
				.AddTile(TileID.Anvils)
				.AddCondition(Condition.NpcIsPresent(NPCID.Cyborg))
				.Register();
		}
	}
}
