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
	// This mount is a car with wheels which behaves similarly 到 unicorn mount. The car has 3 balloons attached 到 back.
	public class ExampleMount : ModMount
	{
		private Asset<Texture2D> balloonTexture;

		// Since only a single instance of ModMountData ever exists, we can use player.mount._mountSpecificData to store additional data related to a specific mount.
		// 使用 something like this for gameplay effects would require ModPlayer syncing, but this example is purely visual.
		protected class CarSpecificData
		{
			internal static float[] offsets = new float[] { 0, 14, -14 };

			internal int count; // 跟踪 how many balloons are still left.
			internal float[] rotations;

			public CarSpecificData() {
				count = 3;
				rotations = new float[count];
			}
		}

		public override void SetStaticDefaults() {
			// Movement
			MountData.jumpHeight = 5; // How high the mount can jump.
			MountData.acceleration = 0.19f; // The rate at which the mount speeds up.
			MountData.jumpSpeed = 4f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is pressed.
			MountData.blockExtraJumps = false; // 确定s whether or not you can use a double jump (like cloud in a bottle) while 在 mount.
			MountData.constantJump = true; // 允许s you to hold the jump button down.
			MountData.heightBoost = 20; // Height between the mount and the ground
			MountData.fallDamage = 0.5f; // Fall damage multiplier.
			MountData.runSpeed = 11f; // The speed 的 mount
			MountData.dashSpeed = 8f; // The speed the mount moves when 在 state of dashing.
			MountData.flightTimeMax = 0; // The amount of time in frames a mount 可以 在 state of flying.

			// Misc
			MountData.fatigueMax = 0;
			MountData.buff = ModContent.BuffType<Buffs.ExampleMountBuff>(); // The ID number 的 buff assigned 到 mount.

			// Effects
			MountData.spawnDust = ModContent.DustType<Dusts.Sparkle>(); // The ID 的 dust spawned when mounted or dismounted.

			// Frame data and player offsets
			MountData.totalFrames = 4; // Amount of animation frames 对于 mount
			MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
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
				ref float rotation = ref balloons.rotations[i]; // This is a reference variable. It's set to point directly 到 'i' index 在 rotations array, so it works like an alias here.

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
			// 当 this mount is mounted, we initialize _mountSpecificData with a new CarSpecificData object which will track some extra visuals 对于 mount.
			player.mount._mountSpecificData = new CarSpecificData();

			// This code bypasses the normal mount spawning dust and replaces it with our own visual.
			if (!Main.dedServ) {
				for (int i = 0; i < 16; i++) {
					Dust.NewDustPerfect(player.Center + new Vector2(80, 0).RotatedBy(i * Math.PI * 2 / 16f), MountData.spawnDust);
				}

				skipDust = true;
			}
		}

		public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow) {
			// 绘制 is called for each mount texture we provide, so we check drawType to avoid duplicate draws.
			if (drawType == 0) {
				// 我们 draw some extra balloons before _Back texture
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

			// by returning true, the regular drawing will still happen.
			return true;
		}
	}
}