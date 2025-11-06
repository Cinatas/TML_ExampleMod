using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Dusts
{
	public class ExampleBubble : ModDust
	{
		public override void OnSpawn(Dust dust) {
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 30, 30);
			// 如果 our 纹理 had 3 different dust on 顶部 of each other (a 30x90 pixel 图像), we might do this:
			// dust.帧 = new Rectangle(0, Main.rand.Next(3) * 30, 30, 30);
		}

		public override bool Update(Dust dust) {
			// 移动 the dust 基于 its 速度 and reduce its 大小 到n 删除 it, as the '返回 假;' 在 结束 will 防止 vanilla logic.
			dust.position += dust.velocity;
			dust.scale -= 0.01f;

			if (dust.scale < 0.75f)
				dust.active = false;

			return false;
		}
	}
}
