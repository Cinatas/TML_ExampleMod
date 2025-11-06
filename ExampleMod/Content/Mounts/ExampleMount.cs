using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ExampleMod.Content.Mounts
{
	// This 坐骑 is a car with wheels which behaves similarly 到 unicorn 坐骑. The car has 3 balloons attached 到 back.
	public class ExampleMount : ModMount
	{
		private Asset<Texture2D> balloonTexture;

		// Since only a single 实例 of ModMountData ever exists, we can use 玩家.坐骑._mountSpecificData to store additional 数据 related to a specific 坐骑.
		// 使用 something like this for gameplay effects would require ModPlayer syncing, but this example is purely visual.
		protected class CarSpecificData
		{
			internal static float[] offsets = new float[] { 0, 14, -14 };

			internal int count; // 跟踪 how many balloons are still 左.
			internal float[] rotations;

			public CarSpecificData() {
				count = 3;
				rotations = new float[count];
			}
		}

		public override void SetStaticDefaults() {
			// Movement
			MountData.jumpHeight = 5; // How high the 坐骑 can 跳跃.
			MountData.acceleration = 0.19f; // The rate at which the 坐骑 speeds up.
			MountData.jumpSpeed = 4f; // The rate at which the 玩家 and 坐骑 ascend towards (negative y 速度) the 跳跃 高度 when the 跳跃 按钮 is pressed.
			MountData.blockExtraJumps = false; // 确定s whether or not you can use a double 跳跃 (like cloud in a bottle) while 在 坐骑.
			MountData.constantJump = true; // 允许s you to hold the 跳跃 按钮 down.
			MountData.heightBoost = 20; // 高度 between the 坐骑 and the ground
			MountData.fallDamage = 0.5f; // Fall 伤害 乘数.
			MountData.runSpeed = 11f; // The 速度 的 坐骑
			MountData.dashSpeed = 8f; // The 速度 the 坐骑 moves when 在 状态 of dashing.
			MountData.flightTimeMax = 0; // The amount of 时间 in frames a 坐骑 可以 在 状态 of flying.

			// Misc
			MountData.fatigueMax = 0;
			MountData.buff = ModContent.BuffType<Buffs.ExampleMountBuff>(); // The ID 数字 的 增益 assigned 到 坐骑.

			// Effects
			MountData.spawnDust = ModContent.DustType<Dusts.Sparkle>(); // The ID 的 dust spawned when mounted or dismounted.

			// 帧 数据 and 玩家 offsets
			MountData.totalFrames = 4; // Amount of 动画 frames 对于 坐骑
			MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray(); // Fills an 数组 with values for less repeating code
			MountData.xOffset = 13;
			MountData.yOffset = -12;
			MountData.playerHeadOffset = 22;
			MountData.bodyFrame = 3;
			// Standing
			MountData.standingFrameCount = 4;
			MountData.standingFrameDelay = 12;
			MountData.standingFrameStart = 0;
			// 运行ning
			MountData.runningFrameCount = 4;
			MountData.runningFrameDelay = 12;
			MountData.runningFrameStart = 0;
			// Flying
			MountData.flyingFrameCount = 0;
			MountData.flyingFrameDelay = 0;
			MountData.flyingFrameStart = 0;
			// In-air
			MountData.inAirFrameCount = 1;
			MountData.inAirFrameDelay = 12;
			MountData.inAirFrameStart = 0;
			// Idle
			MountData.idleFrameCount = 4;
			MountData.idleFrameDelay = 12;
			MountData.idleFrameStart = 0;
			MountData.idleFrameLoop = true;
			// Swim
			MountData.swimFrameCount = MountData.inAirFrameCount;
			MountData.swimFrameDelay = MountData.inAirFrameDelay;
			MountData.swimFrameStart = MountData.inAirFrameStart;

			if (!Main.dedServ) {
				MountData.textureWidth = MountData.backTexture.Width() + 20;
				MountData.textureHeight = MountData.backTexture.Height();
			}

			balloonTexture = Mod.Assets.Request<Texture2D>("Content/Items/Armor/SimpleAccessory_Balloon");
		}

		public override void UpdateEffects(Player player) {
			// This code simulates some wind resistance 对于 balloons.
			var balloons = (CarSpecificData)player.mount._mountSpecificData;
			float balloonMovementScale = 0.05f;

			for (int i = 0; i < balloons.count; i++) {
				ref float rotation = ref balloons.rotations[i]; // This is a 引用 变量. It's set to 点 directly 到 'i' 索引 在 rotations 数组, so it works like an alias here.

				if (Math.Abs(rotation) > MathHelper.PiOver2)
					balloonMovementScale *= -1;

				rotation += -player.velocity.X * balloonMovementScale * Main.rand.NextFloat();
				rotation = rotation.AngleLerp(0, 0.05f);
			}

			// This code spawns some dust if we are moving fast enough.
			if (Math.Abs(player.velocity.X) > 4f) {
				Rectangle rect = player.getRect();

				Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, ModContent.DustType<Dusts.Sparkle>());
			}
		}

		public override void SetMount(Player player, ref bool skipDust) {
			// 当 this 坐骑 is mounted, we initialize _mountSpecificData with a new CarSpecificData 对象 which will 跟踪 some extra visuals 对于 坐骑.
			player.mount._mountSpecificData = new CarSpecificData();

			// This code bypasses the normal 坐骑 spawning dust and replaces it with our own visual.
			if (!Main.dedServ) {
				for (int i = 0; i < 16; i++) {
					Dust.NewDustPerfect(player.Center + new Vector2(80, 0).RotatedBy(i * Math.PI * 2 / 16f), MountData.spawnDust);
				}

				skipDust = true;
			}
		}

		public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow) {
			// 绘制 is called for each 坐骑 纹理 we provide, so we check drawType to avoid duplicate draws.
			if (drawType == 0) {
				// 我们 draw some extra balloons before _Back 纹理
				var balloons = (CarSpecificData)drawPlayer.mount._mountSpecificData;
				int timer = DateTime.Now.Millisecond % 800 / 200;
				Texture2D balloon = balloonTexture.Value;

				for (int i = 0; i < balloons.count; i++) {
					var position = drawPosition + new Vector2((-36 + CarSpecificData.offsets[i]) * drawPlayer.direction, 14);
					var srcRect = new Rectangle(28, balloon.Height / 4 * ((timer + i) % 4), 28, 42);
					float drawRotation = rotation + balloons.rotations[i];
					var origin = new Vector2(14 + drawPlayer.direction * 7, 42);

					playerDrawData.Add(new DrawData(balloon, position, srcRect, drawColor, drawRotation, origin, drawScale, spriteEffects ^ SpriteEffects.FlipHorizontally, 0));
				}
			}

			// by returning 真, the regular drawing will still happen.
			return true;
		}
	}
}