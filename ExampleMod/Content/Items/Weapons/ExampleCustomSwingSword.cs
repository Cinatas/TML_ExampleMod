using ExampleMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	// 示例CustomSwingSword is an example of a sword with a custom swing using a held 弹幕
	// 这是 great if you 想要 make melee weapons with complex swing behavior
	public class ExampleCustomSwingSword : ModItem
	{
		public int attackType = 0; // keeps 跟踪 of which 攻击 it is
		public int comboExpireTimer = 0; // 我们想要 the 攻击 pattern to 重置 if the 武器 is not used for certain period of 时间

		public override void SetDefaults() {
			// 常见 Properties
			Item.width = 46;
			Item.height = 48;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ItemRarityID.Green;

			// 使用 Properties
			// 注意 that useTime and useAnimation for this 项 don't actually affect the behavior because the held 弹幕 handles that. 
			// Each 攻击 takes a different amount of 时间 to execute
			// Conforming 到 项 useTime and useAnimation makes it much harder to design
			// 它, however, affect the 项 工具提示, so don't leave it out.
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;

			// 武器 Properties
			Item.knockBack = 7;  // The knockback of your sword, this is dynamically adjusted 在 弹幕 code.
			Item.autoReuse = true; // This determines whether the 武器 has autoswing
			Item.damage = 62; // The 伤害 of your sword, this is dynamically adjusted 在 弹幕 code.
			Item.DamageType = DamageClass.Melee; // Deals melee 伤害
			Item.noMelee = true;  // 这使 sure the 项 does not deal 伤害 从 swinging 动画
			Item.noUseGraphic = true; // 这使 sure the 项 does not get shown when the 玩家 swings his hand

			// 弹幕 Properties
			Item.shoot = ModContent.ProjectileType<ExampleCustomSwingProjectile>(); // The sword as a 弹幕
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// 使用 the shoot 函数, we override the swing 弹幕 to set ai[0] (which 攻击 it is)
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, attackType);
			attackType = (attackType + 1) % 2; // Increment attackType to 确保 next swing is different
			comboExpireTimer = 0; // 每次 the 武器 is used, we 重置 this so the combo does not expire
			return false; // 返回 假 to 防止 original 弹幕 from being shot
		}

		public override void UpdateInventory(Player player) {
			if (comboExpireTimer++ >= 120) // after 120 ticks (== 2 seconds) in 库存, 重置 the 攻击 pattern
				attackType = 0;
		}

		public override bool MeleePrefix() {
			return true; // 返回 真 to 允许 武器 to have melee prefixes (e.g. Legendary)
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}