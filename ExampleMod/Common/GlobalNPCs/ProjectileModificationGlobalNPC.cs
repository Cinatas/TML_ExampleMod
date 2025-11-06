using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalNPCs
{
	// 这是一个与 ExampleProjectileModifications 相关的功能类。
	public class ProjectileModificationGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public int timesHitByModifiedProjectiles;
	}
}
