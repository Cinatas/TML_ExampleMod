using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.EntitySources
{
	// 以下类展示了 IEntitySource 实例的模式匹配，以使事情仅在特定上下文中发生。
	public sealed class ExampleSourceDependentProjectileTweaks : GlobalProjectile
	{
		// 尽可能重写 AppliesToEntity！
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
			return entity.type is ProjectileID.BulletDeadeye;
		}

		public override void OnSpawn(Projectile projectile, IEntitySource source) {
			// 使战术骷髅射出的子弹造成更少伤害
			if (source is EntitySource_Parent parent && parent.Entity is NPC npc && npc.type == NPCID.TacticalSkeleton) {
				projectile.damage /= 2;
			}
		}
	}

	public sealed class ExampleSourceDependentItemTweaks : GlobalItem
	{
		public override void OnSpawn(Item item, IEntitySource source) {
			// 为所有树木掉落物附带一个史莱姆。
			if (source is EntitySource_ShakeTree) {
				NPC.NewNPC(source, (int)item.position.X, (int)item.position.Y, NPCID.BlueSlime);
			}
		}
	}

	public sealed class ExampleSourceDependentItemTweaks2 : GlobalItem
	{
		// 尽可能重写 AppliesToEntity！
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
			return entity.type is ItemID.CopperCoin or ItemID.SilverCoin or ItemID.GoldCoin or ItemID.PlatinumCoin;
		}

		public override void OnSpawn(Item item, IEntitySource source) {
			// make coins spawned from the lucky coin accessory fly into the air
			if (source.Context == "LuckyCoin") {
				item.velocity.Y -= 20;
			}
		}
	}
}
