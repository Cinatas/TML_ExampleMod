using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Dusts
{
	// This Dust will show off Dust.customData, using vanilla dust 纹理, and some neat movement.
	internal class ExampleAdvancedDust : ModDust
	{
		/*
			Spawning this dust is a little more involved because we need to assign a rotation, customData, and fix the position.
			Position 必须 fixed here because otherwise the first time the dust is drawn it'll draw 在 incorrect place.
			This dust is not used in ExampleMod yet, so you'll have to add some code somewhere. Try ExamplePlayer.DrawEffects.

			Dust dust = Dust.NewDustDirect(Player.Center, 0, 0, ModContent.DustType<Content.Dusts.AdvancedDust>(), Scale: 2);
			dust.rotation = Main.rand.NextFloat(6.28f);
			dust.customData = Player;
			dust.position = Player.Center + Vector2.UnitX.RotatedBy(dust.rotation, Vector2.Zero) * dust.scale * 50;
		*/
		public override string Texture => null; // If we 想要 use vanilla 纹理

		public override void OnSpawn(Dust dust) {
			dust.noGravity = true;

			// Since the vanilla dust 纹理 has all the dust in 1 文件, we'll 需要 do some math.
			// 如果 you 想要 use a vanilla dust 纹理, you can 复制 and 粘贴 it, changing the desiredVanillaDustTexture
			int desiredVanillaDustTexture = 139;
			int frameX = desiredVanillaDustTexture * 10 % 1000;
			int frameY = desiredVanillaDustTexture * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
			dust.frame = new Rectangle(frameX, frameY, 8, 8);

			dust.velocity = Vector2.Zero;
		}

		// This 更新 方法 shows off some interesting movement. Using customData assigned to a 玩家, we spiral around the 玩家 while slowly getting closer. In practice, it looks like a vortex.
		public override bool Update(Dust dust) {
			// 在这里 we 旋转 and 缩放 down the dust. The dustIndex % 2 == 0 part lets half the dust 旋转 clockwise and the other half 计数器 clockwise
			dust.rotation += 0.1f * (dust.dustIndex % 2 == 0 ? -1 : 1);
			dust.scale -= 0.05f;

			// 在这里 我们使用 the customData 字段. If customData is the 类型 we expect, 玩家, we do some special movement.
			if (dust.customData != null && dust.customData is Player player) {
				// 在这里 we assign 位置 to some 偏移 从 玩家 that was assigned. This 偏移 scales with dust.缩放. The 缩放 and 旋转 cause the spiral movement we desired.
				dust.position = player.Center + Vector2.UnitX.RotatedBy(dust.rotation, Vector2.Zero) * dust.scale * 50;
			}

			// 在这里 we 确保 to kill 任何 dust that get really small.
			if (dust.scale < 0.25f)
				dust.active = false;

			return false;
		}
	}
}
