using ExampleMod.Common.Configs;
using ExampleMod.Common.GlobalItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

//与 GlobalItem 相关：WeaponWithGrowingDamage
namespace ExampleMod.Common.GlobalProjectiles
{
	public class ProjectileWithGrowingDamage : GlobalProjectile
	{
		private WeaponWithGrowingDamage sourceGlobalItem;

		public override bool InstancePerEntity => true;

		public override bool IsLoadingEnabled(Mod mod) {
			// 要试验此示例，你需要在配置中启用它。
			return ModContent.GetInstance<ExampleModConfig>().WeaponWithGrowingDamageToggle;
		}

		public override void OnSpawn(Projectile projectile, IEntitySource source) {
			//不要尝试存储 itemSource.Item。Terraria 可以使用 SetDefaults() 重新使用物品实例，
			//这意味着你保存的实例可能变成空气或另一个物品。存储 GlobalItem 实例要安全得多。
			if (source is IEntitySource_WithStatsFromItem itemSource) {
				itemSource.Item.TryGetGlobalItem(out sourceGlobalItem);
			}
		}

		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
			if (sourceGlobalItem == null) {
				return;
			}

			int owner = projectile.owner;
			if (owner < 0 || owner >= Main.player.Length) {
				return;
			}

			Player player = Main.player[owner];
			sourceGlobalItem.OnHitNPCGeneral(player, target, hit, projectile: projectile);
		}
	}
}
