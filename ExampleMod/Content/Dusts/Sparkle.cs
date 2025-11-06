using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Dusts
{
	public class Sparkle : ModDust
	{
		public override void OnSpawn(Dust dust) {
			dust.velocity *= 0.4f; // Multiply the dust's 开始 速度 by 0.4, slowing it down
			dust.noGravity = true; // 使 the dust have no gravity.
			dust.noLight = true; // 使 the dust emit no light.
			dust.scale *= 1.5f; // Multiplies the dust's initial 缩放 by 1.5.
		}

		public override bool Update(Dust dust) { // 调用s every 帧 the dust is active
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X * 0.15f;
			dust.scale *= 0.99f;

			float light = 0.35f * dust.scale;

			Lighting.AddLight(dust.position, light, light, light);

			if (dust.scale < 0.5f) {
				dust.active = false;
			}

			return false; // 返回 假 to 防止 vanilla behavior.
		}
	}
}
