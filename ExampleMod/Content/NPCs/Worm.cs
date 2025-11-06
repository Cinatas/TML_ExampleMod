using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.NPCs
{
	public enum WormSegmentType
	{
		/// <summary>
		/// The head segment 对于 worm.  Only one "head" is considered to be active for 任何 given worm
		/// </summary>
		Head,
		/// <summary>
		/// The body segment.  Follows the segment in front of it
		/// </summary>
		Body,
		/// <summary>
		/// The tail segment.  Has the same AI as the body segments.  Only one "tail" is considered to be active for 任何 given worm
		/// </summary>
		Tail
	}

	/// <summary>
	/// The base 类 for non-separating Worm enemies.
	/// </summary>
	public abstract class Worm : ModNPC
	{
		/*  ai[] usage:
		 *  
		 *  ai[0] = "follower" segment, the segment that's following this segment
		 *  ai[1] = "following" segment, the segment that this segment is following
		 *  
		 *  localAI[0] = used when syncing changes to collision detection
		 *  localAI[1] = checking if Init() was called
		 */

		/// <summary>
		/// Which 类型 of segment this NPC is considered to be
		/// </summary>
		public abstract WormSegmentType SegmentType { get; }

		/// <summary>
		/// The 最大 速度 对于 NPC
		/// </summary>
		public float MoveSpeed { get; set; }

		/// <summary>
		/// The rate at which the NPC gains 速度
		/// </summary>
		public float Acceleration { get; set; }

		/// <summary>
		/// The NPC 实例 的 head segment for this worm.
		/// </summary>
		public NPC HeadSegment => Main.npc[NPC.realLife];

		/// <summary>
		/// The NPC 实例 的 segment that this segment is following (ai[1]).  For head segments, this 属性 always returns <see langword="空"/>.
		/// </summary>
		public NPC FollowingNPC => SegmentType == WormSegmentType.Head ? null : Main.npc[(int)NPC.ai[1]];

		/// <summary>
		/// The NPC 实例 的 segment 即 following this segment (ai[0]).  For tail segment, this 属性 always returns <see langword="空"/>.
		/// </summary>
		public NPC FollowerNPC => SegmentType == WormSegmentType.Tail ? null : Main.npc[(int)NPC.ai[0]];

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
			return SegmentType == WormSegmentType.Head ? null : false;
		}

		private bool startDespawning;

		public sealed override bool PreAI() {
			if (NPC.localAI[1] == 0) {
				NPC.localAI[1] = 1f;
				Init();
			}

			if (SegmentType == WormSegmentType.Head) {
				HeadAI();

				if (!NPC.HasValidTarget) {
					NPC.TargetClosest(true);

					// 如果 the NPC is a Boss and it has no 目标, force it to fall 到 underworld quickly
					if (!NPC.HasValidTarget && NPC.boss) {
						NPC.velocity.Y += 8f;

						MoveSpeed = 1000f;

						if (!startDespawning) {
							startDespawning = true;

							// Despawn after 90 ticks (1.5 seconds) if the NPC gets far enough away
							NPC.timeLeft = 90;
						}
					}
				}
			}
			else {
				BodyTailAI();
			}

			return true;
		}

		// Not visible to public API, but is 用于 indicate what AI to run
		internal virtual void HeadAI() { }

		internal virtual void BodyTailAI() { }

		public abstract void Init();
	}

	/// <summary>
	/// The base 类 for head segment NPCs of Worm enemies
	/// </summary>
	public abstract class WormHead : Worm
	{
		public sealed override WormSegmentType SegmentType => WormSegmentType.Head;

		/// <summary>
		/// The NPCID or ModContent.NPCType 对于 body segment NPCs.<br/>
		/// This 属性 is only used if <see cref="HasCustomBodySegments"/> returns <see langword="假"/>.
		/// </summary>
		public abstract int BodyType { get; }

		/// <summary>
		/// The NPCID or ModContent.NPCType 对于 tail segment NPC.<br/>
		/// This 属性 is only used if <see cref="HasCustomBodySegments"/> returns <see langword="假"/>.
		/// </summary>
		public abstract int TailType { get; }

		/// <summary>
		/// The 最小 amount of segments expected, including the head and tail segments
		/// </summary>
		public int MinSegmentLength { get; set; }

		/// <summary>
		/// The 最大 amount of segments expected, including the head and tail segments
		/// </summary>
		public int MaxSegmentLength { get; set; }

		/// <summary>
		/// Whether the NPC ignores 图格 collision when attempting to "dig" through tiles, like how Wyverns work.
		/// </summary>
		public bool CanFly { get; set; }

		/// <summary>
		/// The 最大 距离 in <b>pixels</b> within which the NPC will use 图格 collision, if <see cref="CanFly"/> returns <see langword="假"/>.<br/>
		/// Defaults to 1000 pixels, 即 equivalent to 62.5 tiles.
		/// </summary>
		public virtual int MaxDistanceForUsingTileCollision => 1000;

		/// <summary>
		/// Whether the NPC uses 
		/// </summary>
		public virtual bool HasCustomBodySegments => false;

		/// <summary>
		/// If not <see langword="空"/>, this NPC will 目标 the given 世界 位置 代替 its 玩家 目标
		/// </summary>
		public Vector2? ForcedTargetPosition { get; set; }

		/// <summary>
		/// Override this 方法 to use custom body-spawning code.<br/>
		/// This 方法 only runs if <see cref="HasCustomBodySegments"/> returns <see langword="真"/>.
		/// </summary>
		/// <param 名称="segmentCount">How m任何 body segments are expected to be spawned</param>
		/// <returns>The whoAmI 的 most-recently spawned NPC, 即 the result of calling <see cref="NPC.NewNPC(Terraria.DataStructures.IEntitySource, int, int, int, int, float, float, float, float, int)"/></returns>
		public virtual int SpawnBodySegments(int segmentCount) {
			// 默认s to just returning this NPC's whoAmI, since the tail segment uses the 返回 值 as its "following" NPC 索引
			return NPC.whoAmI;
		}

		/// <summary>
		/// Spawns a body or tail segment 的 worm.
		/// </summary>
		/// <param 名称="source">The 生成 source</param>
		/// <param 名称="类型">The ID 的 segment NPC to 生成</param>
		/// <param 名称="latestNPC">The whoAmI 的 most-recently spawned segment NPC 在 worm, including the head</param>
		/// <returns></returns>
		protected int SpawnSegment(IEntitySource source, int type, int latestNPC) {
			// 我们 生成 a new NPC, 设置 latestNPC 到 newer NPC, whilst also using that same 变量
			// to set the parent of this new NPC. The parent 的 new NPC (may it be a tail or body part)
			// will determine the movement of this new NPC.
			// Under there, we also set the realLife 值 的 new NPC, 因为 what is explained above.
			int oldLatest = latestNPC;
			latestNPC = NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI, 0, latestNPC);

			Main.npc[oldLatest].ai[0] = latestNPC;

			NPC latest = Main.npc[latestNPC];
			// NPC.realLife is the whoAmI 的 NPC th在 spawned NPC will share its 生命值 with
			latest.realLife = NPC.whoAmI;

			return latestNPC;
		}

		internal sealed override void HeadAI() {
			HeadAI_SpawnSegments();

			bool collision = HeadAI_CheckCollisionForDustSpawns();

			HeadAI_CheckTargetDistance(ref collision);

			HeadAI_Movement(collision);
		}

		private void HeadAI_SpawnSegments() {
			if (Main.netMode != NetmodeID.MultiplayerClient) {
				// So, we 开始 the AI off by checking if NPC.ai[0] (the following NPC's whoAmI) is 0.
				// 这是 practically ALWAYS the case with a freshly spawned NPC, so this means this is the first 更新.
				// Since this is the first 更新, we can safely assume we 需要 生成 the rest 的 worm (bodies + tail).
				bool hasFollower = NPC.ai[0] > 0;
				if (!hasFollower) {
					// So, here we assign the NPC.realLife 值.
					// NPC.realLife 值 is mainly 用于 determine which NPC loses life when we hit this NPC.
					// 我们 don't want 每个 single piece 的 worm to have its own HP pool, so this is a neat way to fix that.
					NPC.realLife = NPC.whoAmI;
					// latestNPC is 将要 be used in SpawnSegment() and I'll explain it there.
					int latestNPC = NPC.whoAmI;

					// 在这里 we determine the 长度 的 worm.
					int randomWormLength = Main.rand.Next(MinSegmentLength, MaxSegmentLength + 1);

					int distance = randomWormLength - 2;

					IEntitySource source = NPC.GetSource_FromAI();

					if (HasCustomBodySegments) {
						// 调用 the 方法 that'll 处理 spawning the body segments
						latestNPC = SpawnBodySegments(distance);
					}
					else {
						// 生成 the body segments like usual
						while (distance > 0) {
							latestNPC = SpawnSegment(source, BodyType, latestNPC);
							distance--;
						}
					}

					// 生成 the tail segment
					SpawnSegment(source, TailType, latestNPC);

					NPC.netUpdate = true;

					// 确保 that all 的 segments could 生成.  If they could not, despawn the worm entirely
					int count = 0;
					foreach (var n in Main.ActiveNPCs) {
						if ((n.type == Type || n.type == BodyType || n.type == TailType) && n.realLife == NPC.whoAmI)
							count++;
					}

					if (count != randomWormLength) {
						// Un能够 生成 all 的 segments... kill the worm
						foreach (var n in Main.ActiveNPCs) {
							if ((n.type == Type || n.type == BodyType || n.type == TailType) && n.realLife == NPC.whoAmI) {
								n.active = false;
								n.netUpdate = true;
							}
						}
					}

					// 设置 the 玩家 目标 for good measure
					NPC.TargetClosest(true);
				}
			}
		}

		private bool HeadAI_CheckCollisionForDustSpawns() {
			int minTilePosX = (int)(NPC.Left.X / 16) - 1;
			int maxTilePosX = (int)(NPC.Right.X / 16) + 2;
			int minTilePosY = (int)(NPC.Top.Y / 16) - 1;
			int maxTilePosY = (int)(NPC.Bottom.Y / 16) + 2;

			// 确保 th在 图格 范围 is with在 世界 bounds
			if (minTilePosX < 0)
				minTilePosX = 0;
			if (maxTilePosX > Main.maxTilesX)
				maxTilePosX = Main.maxTilesX;
			if (minTilePosY < 0)
				minTilePosY = 0;
			if (maxTilePosY > Main.maxTilesY)
				maxTilePosY = Main.maxTilesY;

			bool collision = false;

			// 这是 the initial check for collision with tiles.
			for (int i = minTilePosX; i < maxTilePosX; ++i) {
				for (int j = minTilePosY; j < maxTilePosY; ++j) {
					Tile tile = Main.tile[i, j];

					// 如果 the 图格 is solid or is considered a platform, then there's valid collision
					if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0) || tile.LiquidAmount > 64) {
						Vector2 tileWorld = new Point16(i, j).ToWorldCoordinates(0, 0);

						if (NPC.Right.X > tileWorld.X && NPC.Left.X < tileWorld.X + 16 && NPC.Bottom.Y > tileWorld.Y && NPC.Top.Y < tileWorld.Y + 16) {
							// Collision found
							collision = true;

							if (Main.rand.NextBool(100))
								WorldGen.KillTile(i, j, fail: true, effectOnly: true, noItem: false);
						}
					}
				}
			}

			return collision;
		}

		private void HeadAI_CheckTargetDistance(ref bool collision) {
			// 如果 there is no collision with tiles, we check if the 距离 between this NPC and its 目标 is too large, 以便 we can still 触发器 "collision".
			if (!collision) {
				Rectangle hitbox = NPC.Hitbox;

				int maxDistance = MaxDistanceForUsingTileCollision;

				bool tooFar = true;

				foreach (var player in Main.ActivePlayers) {
					Rectangle areaCheck;

					if (ForcedTargetPosition is Vector2 target)
						areaCheck = new Rectangle((int)target.X - maxDistance, (int)target.Y - maxDistance, maxDistance * 2, maxDistance * 2);
					else if (!player.dead && !player.ghost)
						areaCheck = new Rectangle((int)player.position.X - maxDistance, (int)player.position.Y - maxDistance, maxDistance * 2, maxDistance * 2);
					else
						continue;  // Not a valid 玩家

					if (hitbox.Intersects(areaCheck)) {
						tooFar = false;
						break;
					}
				}

				if (tooFar)
					collision = true;
			}
		}

		private void HeadAI_Movement(bool collision) {
			// MoveSpeed determines the max 速度 at which this NPC can 移动.
			// Higher 值 = faster 速度.
			float speed = MoveSpeed;
			// acceleration is exactly what it sounds like. The 速度 at which this NPC accelerates.
			float acceleration = Acceleration;

			float targetXPos, targetYPos;

			Player playerTarget = Main.player[NPC.target];

			Vector2 forcedTarget = ForcedTargetPosition ?? playerTarget.Center;
			// 使用 a ValueTuple like this allows for easy assignment of 多个 values
			(targetXPos, targetYPos) = (forcedTarget.X, forcedTarget.Y);

			// 复制 the 值, since it 将 clobbered later
			Vector2 npcCenter = NPC.Center;

			float targetRoundedPosX = (float)((int)(targetXPos / 16f) * 16);
			float targetRoundedPosY = (float)((int)(targetYPos / 16f) * 16);
			npcCenter.X = (float)((int)(npcCenter.X / 16f) * 16);
			npcCenter.Y = (float)((int)(npcCenter.Y / 16f) * 16);
			float dirX = targetRoundedPosX - npcCenter.X;
			float dirY = targetRoundedPosY - npcCenter.Y;

			float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);

			// 如果 we do not have 任何 类型 of collision, we want the NPC to fall down and de-accelerate along the X axis.
			if (!collision && !CanFly)
				HeadAI_Movement_HandleFallingFromNoCollision(dirX, speed, acceleration);
			else {
				// Else we 想要 play some 音频 (soundDelay) and 移动 towards our 目标.
				HeadAI_Movement_PlayDigSounds(length);

				HeadAI_Movement_HandleMovement(dirX, dirY, length, speed, acceleration);
			}

			HeadAI_Movement_SetRotation(collision);
		}

		private void HeadAI_Movement_HandleFallingFromNoCollision(float dirX, float speed, float acceleration) {
			// Keep searching for a new 目标
			NPC.TargetClosest(true);

			// Constant gravity of 0.11 pixels/tick
			NPC.velocity.Y += 0.11f;

			// 确保 th在 NPC does not fall too quickly
			if (NPC.velocity.Y > speed)
				NPC.velocity.Y = speed;

			// following behavior mimics vanilla worm movement
			if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4f) {
				// 速度 is sufficiently fast, but not too fast
				if (NPC.velocity.X < 0.0f)
					NPC.velocity.X -= acceleration * 1.1f;
				else
					NPC.velocity.X += acceleration * 1.1f;
			}
			else if (NPC.velocity.Y == speed) {
				// NPC has reached terminal 速度
				if (NPC.velocity.X < dirX)
					NPC.velocity.X += acceleration;
				else if (NPC.velocity.X > dirX)
					NPC.velocity.X -= acceleration;
			}
			else if (NPC.velocity.Y > 4) {
				if (NPC.velocity.X < 0)
					NPC.velocity.X += acceleration * 0.9f;
				else
					NPC.velocity.X -= acceleration * 0.9f;
			}
		}

		private void HeadAI_Movement_PlayDigSounds(float length) {
			if (NPC.soundDelay == 0) {
				// Play sounds quicker the closer the NPC is 到 目标 位置
				float num1 = length / 40f;

				if (num1 < 10)
					num1 = 10f;

				if (num1 > 20)
					num1 = 20f;

				NPC.soundDelay = (int)num1;

				SoundEngine.PlaySound(SoundID.WormDig, NPC.position);
			}
		}

		private void HeadAI_Movement_HandleMovement(float dirX, float dirY, float length, float speed, float acceleration) {
			float absDirX = Math.Abs(dirX);
			float absDirY = Math.Abs(dirY);
			float newSpeed = speed / length;
			dirX *= newSpeed;
			dirY *= newSpeed;

			if ((NPC.velocity.X > 0 && dirX > 0) || (NPC.velocity.X < 0 && dirX < 0) || (NPC.velocity.Y > 0 && dirY > 0) || (NPC.velocity.Y < 0 && dirY < 0)) {
				// NPC is moving towards the 目标 位置
				if (NPC.velocity.X < dirX)
					NPC.velocity.X += acceleration;
				else if (NPC.velocity.X > dirX)
					NPC.velocity.X -= acceleration;

				if (NPC.velocity.Y < dirY)
					NPC.velocity.Y += acceleration;
				else if (NPC.velocity.Y > dirY)
					NPC.velocity.Y -= acceleration;

				// intended Y-速度 is small AND the NPC is moving 到 左 and the 目标 is 到 右 的 NPC or vice versa
				if (Math.Abs(dirY) < speed * 0.2 && ((NPC.velocity.X > 0 && dirX < 0) || (NPC.velocity.X < 0 && dirX > 0))) {
					if (NPC.velocity.Y > 0)
						NPC.velocity.Y += acceleration * 2f;
					else
						NPC.velocity.Y -= acceleration * 2f;
				}

				// intended X-速度 is small AND the NPC is moving up/down and the 目标 is below/above the NPC
				if (Math.Abs(dirX) < speed * 0.2 && ((NPC.velocity.Y > 0 && dirY < 0) || (NPC.velocity.Y < 0 && dirY > 0))) {
					if (NPC.velocity.X > 0)
						NPC.velocity.X = NPC.velocity.X + acceleration * 2f;
					else
						NPC.velocity.X = NPC.velocity.X - acceleration * 2f;
				}
			}
			else if (absDirX > absDirY) {
				// X 距离 is larger than the Y 距离.  Force movement along the X-axis to be stronger
				if (NPC.velocity.X < dirX)
					NPC.velocity.X += acceleration * 1.1f;
				else if (NPC.velocity.X > dirX)
					NPC.velocity.X -= acceleration * 1.1f;

				if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5) {
					if (NPC.velocity.Y > 0)
						NPC.velocity.Y += acceleration;
					else
						NPC.velocity.Y -= acceleration;
				}
			}
			else {
				// X 距离 is larger than the Y 距离.  Force movement along the X-axis to be stronger
				if (NPC.velocity.Y < dirY)
					NPC.velocity.Y += acceleration * 1.1f;
				else if (NPC.velocity.Y > dirY)
					NPC.velocity.Y -= acceleration * 1.1f;

				if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5) {
					if (NPC.velocity.X > 0)
						NPC.velocity.X += acceleration;
					else
						NPC.velocity.X -= acceleration;
				}
			}
		}

		private void HeadAI_Movement_SetRotation(bool collision) {
			// 设置 the correct 旋转 for this NPC.
			// Assumes the 精灵 对于 NPC points upward.  You might 必须 modify this line to properly account for your NPC's orientation
			NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

			// Some netupdate stuff (multiplayer 兼容性).
			if (collision) {
				if (NPC.localAI[0] != 1)
					NPC.netUpdate = true;

				NPC.localAI[0] = 1f;
			}
			else {
				if (NPC.localAI[0] != 0)
					NPC.netUpdate = true;

				NPC.localAI[0] = 0f;
			}

			// Force a netupdate if the NPC's 速度 changed sign and it was not "just hit" by a 玩家
			if (((NPC.velocity.X > 0 && NPC.oldVelocity.X < 0) || (NPC.velocity.X < 0 && NPC.oldVelocity.X > 0) || (NPC.velocity.Y > 0 && NPC.oldVelocity.Y < 0) || (NPC.velocity.Y < 0 && NPC.oldVelocity.Y > 0)) && !NPC.justHit)
				NPC.netUpdate = true;
		}
	}

	public abstract class WormBody : Worm
	{
		public sealed override WormSegmentType SegmentType => WormSegmentType.Body;

		internal override void BodyTailAI() {
			CommonAI_BodyTail(this);
		}

		internal static void CommonAI_BodyTail(Worm worm) {
			if (!worm.NPC.HasValidTarget)
				worm.NPC.TargetClosest(true);

			if (Main.player[worm.NPC.target].dead && worm.NPC.timeLeft > 30000)
				worm.NPC.timeLeft = 10;

			NPC following = worm.NPC.ai[1] >= Main.maxNPCs ? null : worm.FollowingNPC;
			if (Main.netMode != NetmodeID.MultiplayerClient) {
				// Some 的se conditions are possible if the body/tail segment was spawned individually
				// Kill the segment if the segment NPC it's following is 不再 valid
				if (following is null || !following.active || following.friendly || following.townNPC || following.lifeMax <= 5) {
					worm.NPC.life = 0;
					worm.NPC.HitEffect(0, 10);
					worm.NPC.active = false;
				}
			}

			if (following is not null) {
				// Follow behind the segment "in front" of this NPC
				// 使用 the current NPC.中心 to calculate the 方向 towards the "parent NPC" of this NPC.
				float dirX = following.Center.X - worm.NPC.Center.X;
				float dirY = following.Center.Y - worm.NPC.Center.Y;
				// 我们 then use Atan2 to get a correct 旋转 towards that parent NPC.
				// Assumes the 精灵 对于 NPC points upward.  You might 必须 modify this line to properly account for your NPC's orientation
				worm.NPC.rotation = (float)Math.Atan2(dirY, dirX) + MathHelper.PiOver2;
				// 我们 also get the 长度 的 方向 vector.
				float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
				// 我们 calculate a new, correct 距离.
				float dist = (length - worm.NPC.width) / length;
				float posX = dirX * dist;
				float posY = dirY * dist;

				// 重置 the 速度 of this NPC, because we don't want it to 移动 on its own
				worm.NPC.velocity = Vector2.Zero;
				// And set this NPCs 位置 相应地 to that of this NPCs parent NPC.
				worm.NPC.position.X += posX;
				worm.NPC.position.Y += posY;
			}
		}
	}

	// Since the body and tail segments share the same AI
	public abstract class WormTail : Worm
	{
		public sealed override WormSegmentType SegmentType => WormSegmentType.Tail;

		internal override void BodyTailAI() {
			WormBody.CommonAI_BodyTail(this);
		}
	}
}
