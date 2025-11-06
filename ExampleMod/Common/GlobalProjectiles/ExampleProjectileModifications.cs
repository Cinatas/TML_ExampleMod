using ExampleMod.Common.GlobalNPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalProjectiles
{
	// 这是一个专门展示弹幕修改的类
	public class ExampleProjectileModifications : GlobalProjectile
	{
		public override bool InstancePerEntity => true;
		public bool applyBuffOnHit;
		public bool sayTimesHitOnThirdHit;
		// 当用户指定他们想要轨迹时设置这些。
		private Color trailColor;
		private bool trailActive;

		// 在这里，提供了一个用于设置上述字段的方法。
		public void SetTrail(Color color) {
			trailColor = color;
			trailActive = true;
		}

		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
			if (sayTimesHitOnThirdHit) {
				ProjectileModificationGlobalNPC globalNPC = target.GetGlobalNPC<ProjectileModificationGlobalNPC>();
				if (globalNPC.timesHitByModifiedProjectiles % 3 == 0) {
					Main.NewText($"This NPC has been hit with a modified projectile {globalNPC.timesHitByModifiedProjectiles} times.");
				}
				target.GetGlobalNPC<ProjectileModificationGlobalNPC>().timesHitByModifiedProjectiles += 1;
			}

			if (applyBuffOnHit) {
				target.AddBuff(BuffID.OnFire, 50);
			}
		}

		public override void PostAI(Projectile projectile) {
			if (trailActive) {
				Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.TintableDustLighted, default, default, default, trailColor);
			}
		}
	}
}
