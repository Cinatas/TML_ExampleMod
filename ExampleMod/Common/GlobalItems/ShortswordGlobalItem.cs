using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalItems
{
	// 此文件展示了一个非常简单的 GlobalItem 类示例。GlobalItem 钩子在游戏中的所有物品上调用，适用于像
	// 为游戏中的所有物品添加额外数据这样的全面更改。在这里，我们只是调整了铜短剑物品的伤害，因为它很容易理解。
	// 查看 ExampleMod 中的其他 GlobalItem 类以了解 GlobalItem 可以使用的其他方式。
	public class ShortswordGlobalItem : GlobalItem
	{
		// 在这里，我们通过检查 item.type 确保仅为铜短剑实例化此 GlobalItem
		public override bool AppliesToEntity(Item item, bool lateInstantiation) {
			return item.type == ItemID.CopperShortsword;
		}

		public override void SetDefaults(Item item) {
			item.StatsModifiedBy.Add(Mod); // 通知游戏我们对此物品进行了功能性更改。

			item.damage = 50; // 将伤害更改为 50！
		}

		public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// 无缘无故地让它发射手榴弹
			Projectile.NewProjectileDirect(source, player.Center, velocity * 5f, ProjectileID.Grenade, damage, knockback, player.whoAmI);
			// 返回 false 可防止原版的射击行为运行。
			// 在这种情况下，它会阻止短剑的刀刃刺击动画，因为刀刃本身是一个弹幕。
			return false;
		}
	}
}
