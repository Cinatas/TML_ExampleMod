using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	// 此示例 is similar 到 Wooden 箭 弹幕
	public class ExampleArrowProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// 如果 this 箭 would have strong effects (like Holy 箭 pierce), we can make it fire fewer projectiles from Daedalus Stormbow for game balance considerations like this:
			//ProjectileID.Sets.FiresFewerFromDaedalusStormbow[类型] = 真;
		}

		public override void SetDefaults() {
			Projectile.width = 10; // The 宽度 of 弹幕 hitbox
			Projectile.height = 10; // The 高度 of 弹幕 hitbox

			Projectile.arrow = true;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 1200;
		}

		public override void AI() {
			// code below was adapted 从 ProjAIStyleID.箭 behavior. Rather than 复制 an existing aiStyle using 弹幕.aiStyle and AIType,
			// like some examples do, this example has custom AI code 即 better suited for modifying directly.
			// 参见 https://github.com/tModLoader/tModLoader/wiki/Basic-弹幕#what-is-ai f或更多 information on custom 弹幕 AI.

			// 应用 gravity after a quarter of a second
			Projectile.ai[0] += 1f;
			if (Projectile.ai[0] >= 15f) {
				Projectile.ai[0] = 15f;
				Projectile.velocity.Y += 0.1f;
			}

			// 弹幕 is rotated to face the 方向 of travel
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			// Cap downward 速度
			if (Projectile.velocity.Y > 16f) {
				Projectile.velocity.Y = 16f;
			}
		}

		public override void OnKill(int timeLeft) {
			SoundEngine.PlaySound(SoundID.Dig, Projectile.position); // Plays the basic 声音 most projectiles make when hitting blocks.
			for (int i = 0; i < 5; i++) // 创建s a splash of dust around the positi在 弹幕 dies.
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Silver);
				dust.noGravity = true;
				dust.velocity *= 1.5f;
				dust.scale *= 0.9f;
			} 
		}
	}
}