using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleInstancedProjectile : ModProjectile
	{
		private Color trailColor;

		public override void SetDefaults() {
			Projectile.width = 16; //The 宽度 of 弹幕 hitbox
			Projectile.height = 16; //The 高度 of 弹幕 hitbox
			Projectile.aiStyle = 1; //The ai style 的 弹幕, please 引用 the source code of Terraria
			Projectile.friendly = true; //Can the 弹幕 deal 伤害 to enemies?
			Projectile.hostile = false; //Can the 弹幕 deal 伤害 到 玩家?
			Projectile.DamageType = DamageClass.Ranged; //Is the 弹幕 shoot by a ranged 武器?
			Projectile.ignoreWater = true; //Does the 弹幕's 速度 be influenced by water?
			Projectile.tileCollide = true; //Can the 弹幕 collide with tiles?

			AIType = ProjectileID.Bullet; //Act exactly like default 子弹
		}

		public override bool PreDraw(ref Color lightColor) {
			Dust.NewDustPerfect(Projectile.Center, DustID.TintableDust, newColor: trailColor);

			return false;
		}

		public override void OnSpawn(IEntitySource data) {
			trailColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.7f); // Assign a 随机 颜色 on 生成
		}
	}
}