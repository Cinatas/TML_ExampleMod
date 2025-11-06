using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace ExampleMod.Content.NPCs
{
	// This ModNPC serves as an example of a completely custom AI.
	public class ExampleCustomAISlimeNPC : ModNPC
	{
		// 在这里 we define an enum we will use 与 状态 槽位. Using an ai 槽位 as a means to store "状态" can simplify things greatly. Think flowchart.
		private enum ActionState
		{
			Asleep,
			Notice,
			Jump,
			Hover,
			Fall
		}

		// Our 纹理 is 36x36 with 2 pixels of 填充 vertically, so 38 is the vertical 间距.
		// These are for our benefit and the numbers could easily be used directly 在 code below, but this is how we keep code organized.
		private enum Frame
		{
			Asleep,
			Notice,
			Falling,
			Flutter1,
			Flutter2,
			Flutter3
		}

		// These are 引用 properties. One, 例如, lets us write AI_State as if it's NPC.ai[0], essentially giving the 索引 zero our own 名称.
		// 在这里 they 帮助 to keep our AI code 清除 of clutter. Without them, 每个 实例 of "AI_State" 在 AI code below 将 "npc.ai[0]", 即 quite hard to read.
		// 这是 all to just make beautiful, manageable, and clean code.
		public ref float AI_State => ref NPC.ai[0];
		public ref float AI_Timer => ref NPC.ai[1];
		public ref float AI_FlutterTime => ref NPC.ai[2];

		public override void SetStaticDefaults() {
			Main.npcFrameCount[NPC.type] = 6; // 使 sure to set this for your modnpcs.

			NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.ShimmerSlime;

			// Specify the debuffs it is immune to.
			// This NPC 将 immune 到 Poisoned 减益.
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<Buffs.ExampleGravityDebuff>()] = true;
		}

		public override void SetDefaults() {
			NPC.width = 36; // The 宽度 的 npc's hitbox (in pixels)
			NPC.height = 36; // The 高度 的 npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the 玩家, which might conflict with custom AI code.
			NPC.damage = 7; // The amount of 伤害 that this npc deals
			NPC.defense = 2; // The amount of 防御 that this npc has
			NPC.lifeMax = 25; // The amount of 生命值 that this npc has
			NPC.HitSound = SoundID.NPCHit1; // The 声音 the NPC will make when being hit.
			NPC.DeathSound = SoundID.NPCDeath1; // The 声音 the NPC will make when it dies.
			NPC.value = 25f; // How m任何 铜币 coins the NPC will 放下 when killed.
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			// we would like this npc to 生成 在 overworld.
			return SpawnCondition.OverworldDaySlime.Chance * 0.1f;
		}

		// Our AI here makes our NPC sit waiting for a 玩家 to enter 范围, jumps to 攻击, flutter mid-fall to stay afloat 一点 longer, then falls 到 ground. 注意 动画 should happen in FindFrame
		public override void AI() {
			// npc starts 在 asleep 状态, waiting for a 玩家 to enter 范围
			switch (AI_State) {
				case (float)ActionState.Asleep:
					FallAsleep();
					break;
				case (float)ActionState.Notice:
					Notice();
					break;
				case (float)ActionState.Jump:
					Jump();
					break;
				case (float)ActionState.Hover:
					Hover();
					break;
				case (float)ActionState.Fall:
					if (NPC.velocity.Y == 0) {
						NPC.velocity.X = 0;
						AI_State = (float)ActionState.Asleep;
						AI_Timer = 0;
					}

					break;
			}
		}

		// 在这里 in FindFrame, we 想要 set the 动画 帧 our npc will use 取决于 what it is doing.
		// 我们 set npc.帧.Y to x * frameHeight where x is the xth 帧 in our spritesheet, counting from 0. For convenience, we have defined a enum above.
		public override void FindFrame(int frameHeight) {
			// 这使 the 精灵 flip horizontally in conjunction 与 npc.方向.
			NPC.spriteDirection = NPC.direction;

			// 对于 the most part, our 动画 matches up with our states.
			switch (AI_State) {
				case (float)ActionState.Asleep:
					// npc.帧.Y is the goto way of changing 动画 frames. npc.帧 starts 从 顶部 左 corner in pixel coordinates, so keep that in mind.
					NPC.frame.Y = (int)Frame.Asleep * frameHeight;
					break;
				case (float)ActionState.Notice:
					// Going from Notice to Asleep makes our npc look like it's crouching to 跳跃.
					if (AI_Timer < 10) {
						NPC.frame.Y = (int)Frame.Notice * frameHeight;
					}
					else {
						NPC.frame.Y = (int)Frame.Asleep * frameHeight;
					}

					break;
				case (float)ActionState.Jump:
					NPC.frame.Y = (int)Frame.Falling * frameHeight;
					break;
				case (float)ActionState.Hover:
					// 在这里 we have 3 frames that we 想要 循环 through.
					NPC.frameCounter++;

					if (NPC.frameCounter < 10) {
						NPC.frame.Y = (int)Frame.Flutter1 * frameHeight;
					}
					else if (NPC.frameCounter < 20) {
						NPC.frame.Y = (int)Frame.Flutter2 * frameHeight;
					}
					else if (NPC.frameCounter < 30) {
						NPC.frame.Y = (int)Frame.Flutter3 * frameHeight;
					}
					else {
						NPC.frameCounter = 0;
					}

					break;
				case (float)ActionState.Fall:
					NPC.frame.Y = (int)Frame.Falling * frameHeight;
					break;
			}
		}

		// Here, because 我们使用 custom AI (aiStyle not set to a suitable vanilla 值), we should manually decide when Flutter Slime can fall through platforms
		public override bool? CanFallThroughPlatforms() {
			if (AI_State == (float)ActionState.Fall && NPC.HasValidTarget && Main.player[NPC.target].Top.Y > NPC.Bottom.Y) {
				// 如果 Flutter Slime is currently falling, we want it to keep falling through platforms 只要 it's above the 玩家
				return true;
			}

			return false;
			// 你 could also 返回 空 here to apply vanilla behavior (即 the same as 假 for custom AI)
		}

		private void FallAsleep() {
			// TargetClosest sets npc.目标 到 玩家.whoAmI 的 closest 玩家.
			// faceTarget 参数 means that npc.方向 will automatically be 1 or -1 if the targeted 玩家 is 到 右 or 左.
			// 这是 also automatically flipped if npc.confused.
			NPC.TargetClosest(true);

			// Now we check the 确保 the 目标 is still valid and within our specified notice 范围 (500)
			if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 500f) {
				// Since we have a 目标 in 范围, we change 到 Notice 状态. (and zero out the 计时器 for good measure)
				AI_State = (float)ActionState.Notice;
				AI_Timer = 0;
			}
		}

		private void Notice() {
			// 如果 the targeted 玩家 is in 攻击 范围 (250).
			if (Main.player[NPC.target].Distance(NPC.Center) < 250f) {
				// 在这里 我们使用 our 计时器 to wait .33 seconds before actually jumping. In FindFrame you'll notice AI_Timer also being 用于 animate the pre-跳跃 crouch
				AI_Timer++;

				if (AI_Timer >= 20) {
					AI_State = (float)ActionState.Jump;
					AI_Timer = 0;
				}
			}
			else {
				NPC.TargetClosest(true);

				if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 500f) {
					// Out targeted 玩家 seems to have 左 our 范围, so we'll go back to sleep.
					AI_State = (float)ActionState.Asleep;
					AI_Timer = 0;
				}
			}
		}

		private void Jump() {
			AI_Timer++;

			if (AI_Timer == 1) {
				// 我们 apply an initial 速度 the first tick we are 在 跳跃 帧. Remember that -Y is up.
				NPC.velocity = new Vector2(NPC.direction * 2, -10f);
			}
			else if (AI_Timer > 40) {
				// after .66 seconds, we go 到 悬停 状态. //TODO, gravity?
				AI_State = (float)ActionState.Hover;
				AI_Timer = 0;
			}
		}

		private void Hover() {
			AI_Timer++;

			// 在这里 we make a decision on how long this flutter will last. We check netmode != 1 to 防止 Multiplayer Clients from running this code. (similarly, spawning projectiles should also be wrapped like this)
			// netMode == 0 is SP, netMode == 1 is MP 客户端, netMode == 2 is MP 服务器.
			// Typically in MP, 客户端 and 服务器 mainta在 same 状态 by running deterministic code individually. When we 想要 do something 随机, we must do that 在 服务器 然后 inform MP Clients.
			if (AI_Timer == 1 && Main.netMode != NetmodeID.MultiplayerClient) {
				// 对于 引用: without proper syncing: https://media-1.discordapp.net/attachments/242228770855976960/1150274335269998674/FlutterSlime_Netsync_Wrong.mp4 and with proper syncing: https://media-1.discordapp.net/attachments/242228770855976960/1150274355306184804/FlutterSlime_Netsync_Correct.mp4
				AI_FlutterTime = Main.rand.NextBool() ? 100 : 50;

				// Informing MP Clients is done automatically by syncing the npc.ai 数组 over the 网络 whenever npc.netUpdate is set.
				// 不要 set netUpdate unless you do something non-deterministic ("随机")
				NPC.netUpdate = true;
			}

			// 在这里 we add a tiny bit of upward 速度 to our npc.
			NPC.velocity += new Vector2(0, -.35f);

			// ... and some additional X 速度 when traveling slow.
			if (Math.Abs(NPC.velocity.X) < 2) {
				NPC.velocity += new Vector2(NPC.direction * .05f, 0);
			}

			// after fluttering for 100 ticks (1.66 seconds), our Flutter Slime is tired, so he decides to go in到 Fall 状态.
			if (AI_Timer > AI_FlutterTime) {
				AI_State = (float)ActionState.Fall;
				AI_Timer = 0;
			}
		}

		public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox) {
			// 我们 can use ModifyCollisionData to customize collision 伤害.
			// 在这里 we double 伤害 when this npc is 在 falling 状态 and the victim is almost directly below the npc
			if (AI_State == (float)ActionState.Fall) {
				// 我们 can modify npcHitbox directly to implement a dynamic hitbox, but in this example we make a new hitbox to apply 奖励 伤害
				// This math creates a hitbox focused 在 底部 中心 的 original 36x36 hitbox:
				// --> ☐☐☐
				//     ☐☒☐
				Rectangle extraDamageHitbox = new Rectangle(npcHitbox.X + 12, npcHitbox.Y + 18, npcHitbox.Width - 24, npcHitbox.Height - 18);
				if (victimHitbox.Intersects(extraDamageHitbox)) {
					damageMultiplier *= 2f;
					Main.NewText("You got stomped");
				}
			}
			return true;
		}
	}
}
