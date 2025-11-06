using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	/// <summary>
	/// Showcases ActiveSounds. The various styles when studied in 顺序 serve to teach important concepts, please experiment 与m in-game.
	/// Do note th在 earlier examples aren't useful to 复制, if you are just looking for an example to 复制, consult SoundUpdateCallbackApproach and LoopedSound, as they are the most suitable examples.
	/// This example serves as a companion 到 Active Sounds section 的 Sounds wiki 页面, please study them both together: https://github.com/tModLoader/tModLoader/wiki/Basic-Sounds#active-sounds
	/// </summary>
	public class ActiveSoundShowcaseProjectile : ModProjectile
	{
		internal enum ActiveSoundShowcaseStyle
		{
			// 此示例 plays a long 声音 (12 seconds) and never attempts to change it. Notice how the 声音 plays without any 位置, the 玩家 and 弹幕 can 移动 左 or 右 and the 声音 panning and 音量 do not change. Also note th在 声音 keeps playing after the 弹幕 dies.
			FireAndForget,
			// 此示例 improves on FireAndForget. The 弹幕 位置 is passed into PlaySound. The 声音 still does not 更新 位置, but the 玩家 can 移动 around the initial 生成 位置 and the 声音 pans and 音量 adjusts accordingly.
			FireAndForgetPlusInitialPosition,
			// Further improving on FireAndForgetPlusInitialPosition, this example updates the 声音 位置 in AI and stops the 声音 when the 弹幕 is killed in Kill.
			SyncSoundToProjectilePosition,
			// Further improving on SyncSoundToProjectilePosition, this example uses the SoundUpdateCallback 参数 to keep all 声音 logic organized in a single place instead of spread between different methods.
			SoundUpdateCallbackApproach,
			// LoopedSound shows using SoundUpdateCallback once again to adjust 声音 位置. The SoundStyle used is looped, so SoundUpdateCallback is necessary in case 弹幕.Kill doesn't get called for some exceptional reason.
			LoopedSound,
			// LoopedSoundAdvanced adjusts 音高 and 音量 dynamically 在 SoundUpdateCallback, in addition 到 usual 声音 位置.
			LoopedSoundAdvanced,
		}

		private ActiveSoundShowcaseStyle Style {
			get => (ActiveSoundShowcaseStyle)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}

		SlotId soundSlot;
		bool played = false;

		SoundStyle soundStyleTwister = new SoundStyle("Terraria/Sounds/Custom/dd2_book_staff_twister_loop");

		SoundStyle soundStyleIgniteLoop = new SoundStyle("Terraria/Sounds/Custom/dd2_kobold_ignite_loop") {
			IsLooped = true,
			SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
			// 注意 that MaxInstances defaults to 1.
		};

		public override void SetStaticDefaults() {
			Main.projFrames[Projectile.type] = 6;
		}

		public override void SetDefaults() {
			Projectile.width = 22;
			Projectile.height = 24;
			Projectile.penetrate = 4; // Can bounce 3 times and dies on 4th 图格 collide
			Projectile.timeLeft = 300; // Despawns after 5 seconds
		}

		public override void OnSpawn(IEntitySource source) {
			Main.NewText($"{Style}");
		}

		public override void AI() {
			Projectile.frame = (int)Style;

			// Sounds are paused when the game loses focus (玩家 switches to another program). In some situations the modder might want to restart a 声音 when the game is focused again, in other situations that might 不 desired. Some 的se examples use a bool, "played", to 跟踪 if the 声音 has been played since the 弹幕 spawned, while others do not and will attempt to restart the 声音 if it is not currently playing.

			// 另外 note that in this example the SoundStyle all have "MaxInstances = 1" and "SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest" 默认情况下, so if 2 projectiles attempt to play the same 声音, they'll constantly interrupt each other every AI 更新, making a horrible 声音.
			// 在 a real mod, the modder should design the SoundStyle properties and PlaySound logic to meet their needs. 例如, the modder might decide that 3 overlapping sounds is too chaotic and adjust MaxInstances accordingly. The modder might also decide th在 声音 should not restart when the game is re-focused and use logic to only attempt to play the 声音 once.
			switch (Style) {
				case ActiveSoundShowcaseStyle.FireAndForget:
					if (!played) {
						played = true;
						SoundEngine.PlaySound(soundStyleTwister);
					}
					break;
				case ActiveSoundShowcaseStyle.FireAndForgetPlusInitialPosition:
					if (!played) {
						played = true;
						SoundEngine.PlaySound(soundStyleTwister, Projectile.position);
					}
					break;
				case ActiveSoundShowcaseStyle.SyncSoundToProjectilePosition:
					if (!SoundEngine.TryGetActiveSound(soundSlot, out var activeSoundTwister)) {
						soundSlot = SoundEngine.PlaySound(soundStyleTwister, Projectile.position);
					}
					else {
						// 如果 the 声音 is playing, 更新 the 声音's 位置 to 匹配 the current 位置 的 弹幕.
						activeSoundTwister.Position = Projectile.position;
					}
					break;
				case ActiveSoundShowcaseStyle.SoundUpdateCallbackApproach:
					if (!SoundEngine.TryGetActiveSound(soundSlot, out var _)) {
						var tracker = new ProjectileAudioTracker(Projectile);
						soundSlot = SoundEngine.PlaySound(soundStyleTwister, Projectile.position, soundInstance => BasicSoundUpdateCallback(tracker, soundInstance));

						// 如果 only the 声音 stopping when the 弹幕 is killed is required, this simpler code 可以 used:
						//soundSlot = SoundEngine.PlaySound(soundStyleTwister, 弹幕.位置, soundInstance => tracker.IsActiveAndInGame());

						// Do NOT make this mistake, the ProjectileAudioTracker 对象 必须 initialized outside the 回调:
						// soundSlot = SoundEngine.PlaySound(soundStyleTwister, 弹幕.位置, soundInstance => new ProjectileAudioTracker(弹幕).IsActiveAndInGame()); // WRONG
					}
					break;
				case ActiveSoundShowcaseStyle.LoopedSound:
					if (!SoundEngine.TryGetActiveSound(soundSlot, out var _)) {
						var tracker = new ProjectileAudioTracker(Projectile);
						soundSlot = SoundEngine.PlaySound(soundStyleIgniteLoop, Projectile.position, soundInstance => {
							// SoundUpdateCallback 可以 inlined if desired, 例如 in this example. Otherwise, LoopedSoundAdvanced shows the other approach
							soundInstance.Position = Projectile.position;
							return tracker.IsActiveAndInGame();
						});
					}

					// SlotId 可以 stored as a float, 例如 in 弹幕.localAI entries. This 可以 an alternative to making a SlotId 字段 在 类.
					// 不要 use ai slots for SlotId, since those will 同步 and sounds and 声音 slots are completely local and are not synced
					// SlotId soundSlot = SlotId.FromFloat(弹幕.localAI[0]);
					// 弹幕.localAI[0] = soundSlot.ToFloat();

					// As an alternate approach to TryGetActiveSound, we could use FindActiveSound. The difference is that FindActiveSound will 查找 any ActiveSound matching the given SoundStyle, so if 2 弹幕 instances 生成 the same SoundStyle, the ActiveSound retrieved isn't necessarily the 声音 spawned by this 实例. This 可以 useful, but in this situation we want the ActiveSound spawned by this 弹幕.
					/* 
					var activeSoundB = SoundEngine.FindActiveSound(soundStyleIgniteLoop);
					if (activeSoundB == null) {
						SoundEngine.PlaySound(soundStyleIgniteLoop, Projectile.position, updateIgniteLoop);
					}
					*/
					break;
				case ActiveSoundShowcaseStyle.LoopedSoundAdvanced:
					if (!SoundEngine.TryGetActiveSound(soundSlot, out var _)) {
						var tracker = new ProjectileAudioTracker(Projectile);
						soundSlot = SoundEngine.PlaySound(soundStyleIgniteLoop, Projectile.position, soundInstance => AdvancedSoundUpdateCallback(tracker, soundInstance));
					}
					break;
			}
		}

		private bool BasicSoundUpdateCallback(ProjectileAudioTracker tracker, ActiveSound soundInstance) {
			// 更新 声音 位置 according to 弹幕 位置
			soundInstance.Position = Projectile.position;
			// ProjectileAudioTracker is necessary to avoid rare situations where sounds can 循环 indefinitely. IsActiveAndInGame returns a 值 indicating if the 声音 should still be active.
			return tracker.IsActiveAndInGame();
		}

		private bool AdvancedSoundUpdateCallback(ProjectileAudioTracker tracker, ActiveSound soundInstance) {
			soundInstance.Position = Projectile.position;

			// Dynamic 音高 example: 音高 rises 每次 the 弹幕 bounces
			soundInstance.Pitch = (Projectile.maxPenetrate - Projectile.penetrate) * 0.15f;

			// Muffle the 声音 if the 弹幕 is wet
			if (Projectile.wet) {
				soundInstance.Pitch -= 0.4f;
				soundInstance.Volume = MathHelper.Clamp(soundInstance.Style.Volume - 0.4f, 0f, 1f);
			}

			return tracker.IsActiveAndInGame();
		}

		public override void OnKill(int timeLeft) {
			// 对于 long sounds, the 声音 可以 stopped when the 弹幕 is killed.
			// This approach is not foolproof, so it should 不 used, especially for looped sounds.
			// 参见 SoundUpdateCallbackApproach 对于 better approach. This example, however, does show how an ActiveSound 可以 modified from another hook other than where the 声音 was played.
			if (Style == ActiveSoundShowcaseStyle.SyncSoundToProjectilePosition) {
				if (SoundEngine.TryGetActiveSound(soundSlot, out var activeSound)) {
					activeSound.Stop();
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity) {
			// 如果 collide with 图格, reduce the penetrate.
			// So the 弹幕 can reflect 至多 3 times
			Projectile.penetrate--;
			if (Projectile.penetrate <= 0) {
				Projectile.Kill();
			}
			else {
				Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
				SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

				// 如果 the 弹幕 hits the 左 or 右 side 的 图格, reverse the X 速度
				if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
					Projectile.velocity.X = -oldVelocity.X;
				}

				// 如果 the 弹幕 hits the 顶部 or 底部 side 的 图格, reverse the Y 速度
				if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
					Projectile.velocity.Y = -oldVelocity.Y;
				}
			}

			return false;
		}
	}
}
