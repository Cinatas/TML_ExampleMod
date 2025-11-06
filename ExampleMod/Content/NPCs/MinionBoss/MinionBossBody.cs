using ExampleMod.Common.Systems;
using ExampleMod.Content.BossBars;
using ExampleMod.Content.Items;
using ExampleMod.Content.Items.Armor.Vanity;
using ExampleMod.Content.Items.Consumables;
using ExampleMod.Content.Pets.MinionBossPet;
using ExampleMod.Content.Projectiles;
using ExampleMod.Content.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.NPCs.MinionBoss
{
	// main part 的 Boss, usually referred to as "body"
	[AutoloadBossHead] // This attribute looks for a 纹理 called "ClassName_Head_Boss" and automatically registers it as the NPC Boss head 图标
	public class MinionBossBody : ModNPC
	{
		// This Boss has a second 阶段 and we want to give it a second Boss head 图标, this 变量 keeps 跟踪 的 registered 纹理 from 加载().
		// It is applied 在 BossHeadSlot hook when the Boss is in its second 阶段
		public static int secondStageHeadSlot = -1;

		// This code here is called a 属性: It acts like a 变量, but can modify other things. In this case it uses the NPC.ai[] 数组 that has four entries.
		// 我们 use properties because it makes code more readable ("if (SecondStage)" vs "if (NPC.ai[0] == 1f)").
		// 我们 use NPC.ai[] because in combination with NPC.netUpdate we can make it multiplayer compatible. Otherwise (making our own fields) we would have to write extra code to make it work (not covered here)
		public bool SecondStage {
			get => NPC.ai[0] == 1f;
			set => NPC.ai[0] = value ? 1f : 0f;
		}
		// 如果 your Boss has more than two stages, and since this is a 布尔值 and can only be two things (真, 假), consider using an integer or enum

		// More advanced usage of a 属性, used to wrap around to floats to act as a Vector2
		public Vector2 FirstStageDestination {
			get => new Vector2(NPC.ai[1], NPC.ai[2]);
			set {
				NPC.ai[1] = value.X;
				NPC.ai[2] = value.Y;
			}
		}

		public int MinionMaxHealthTotal {
			get => (int)NPC.ai[3];
			set => NPC.ai[3] = value;
		}

		public int MinionHealthTotal { get; set; }

		// Auto-implemented 属性, acts exactly like a 变量 by using a hidden backing 字段
		public Vector2 LastFirstStageDestination { get; set; } = Vector2.Zero;

		// This 属性 uses NPC.localAI[] instead which doesn't get synced, but because SpawnedMinions is only used on 生成 as a 标志, this will get set by all parties to 真.
		// Knowing what side (客户端, 服务器, all) is in charge of a 变量 is important as NPC.ai[] only has four entries, so choose wisely which things you need synced and not synced
		public bool SpawnedMinions {
			get => NPC.localAI[0] == 1f;
			set => NPC.localAI[0] = value ? 1f : 0f;
		}

		private const int FirstStageTimerMax = 90;
		// 这是 a 引用 属性. It lets us write FirstStageTimer as if it's NPC.localAI[1], essentially giving it our own 名称
		public ref float FirstStageTimer => ref NPC.localAI[1];

		// 我们 could also repurpose FirstStageTimer since it's unused 在 second 阶段, or write "=> ref FirstStageTimer", but then we have to 重置 the 计时器 when the 状态 switch happens
		public ref float SecondStageTimer_SpawnEyes => ref NPC.localAI[3];

		// Do NOT try to use NPC.ai[4]/NPC.localAI[4] or higher indexes, it only accepts 0, 1, 2 and 3!
		// 如果 you choose to go the route of "wrapping properties" for NPC.ai[], make sure they don't overlap (two properties using the same 变量 in different ways), and that you don't accidently use NPC.ai[] directly

		// Helper 方法 to determine the 仆从 类型
		public static int MinionType() {
			return ModContent.NPCType<MinionBossMinion>();
		}

		// Helper 方法 to determine the amount of minions summoned
		public static int MinionCount() {
			int count = 15;

			if (Main.expertMode) {
				count += 5; // Increase by 5 if expert or master 模式
			}

			if (Main.getGoodWorld) {
				count += 5; // Increase by 5 if using the "对于 Worthy" 种子
			}

			return count;
		}

		public override void Load() {
			// 我们 want to give it a second Boss head 图标, so we register one
			string texture = BossHeadTexture + "_SecondStage"; // Our 纹理 is called "ClassName_Head_Boss_SecondStage"
			secondStageHeadSlot = Mod.AddBossHeadTexture(texture, -1); // -1 because we already have one registered via the [AutoloadBossHead] attribute, it would overwrite it otherwise
		}

		public override void BossHeadSlot(ref int index) {
			int slot = secondStageHeadSlot;
			if (SecondStage && slot != -1) {
				// 如果 the Boss is in its second 阶段, 显示 the other head 图标 instead
				index = slot;
			}
		}

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 6;

			// 添加 this in for bosses that have a summon 项, requires corresponding code 在 项 (See MinionBossSummonItem.cs)
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			// Automatically 分组 with other bosses
			NPCID.Sets.BossBestiaryPriority.Add(Type);

			// Specify the debuffs it is immune to. Most NPCs are immune to Confused.
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			// This Boss also becomes immune to OnFire and all buffs that inherit OnFire immunity during the second half 的 fight. See the ApplySecondStageBuffImmunities 方法.

			// Influences how the NPC looks 在 Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				CustomTexturePath = "ExampleMod/Assets/Textures/Bestiary/MinionBoss_Preview",
				PortraitScale = 0.6f, // Portrait refers 到 full picture when clicking 在 图标 在 bestiary
				PortraitPositionYOverride = 0f,
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults() {
			NPC.width = 110;
			NPC.height = 110;
			NPC.damage = 12;
			NPC.defense = 10;
			NPC.lifeMax = 2000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 5);
			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.npcSlots = 10f; // Take up 打开 生成 slots, preventing 随机 NPCs from spawning during the fight

			// 默认 增益 immunities 应该 set in SetStaticDefaults through the NPCID.Sets.ImmuneTo{X} arrays.
			// 要 dynamically adjust immunities of an active NPC, NPC.buffImmune[] 可以 changed in AI: NPC.buffImmune[BuffID.OnFire] = 真;
			// This approach, however, will not preserve 增益 immunities. To preserve 增益 immunities, use the NPC.BecomeImmuneTo and NPC.ClearImmuneToBuffs methods instead, 如所示 在 ApplySecondStageBuffImmunities 方法 below.

			// 自定义 AI, 0 is "bound town NPC" AI which slows the NPC down and changes 精灵 orientation towards the 目标
			NPC.aiStyle = -1;

			// 自定义 Boss 条
			NPC.BossBar = ModContent.GetInstance<MinionBossBossBar>();

			// following code assigns a 音乐 跟踪 到 Boss in a simple way.
			if (!Main.dedServ) {
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Ropocalypse2");
			}
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// 设置s the 描述 of this NPC 即 listed 在 bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black 背景
				new FlavorTextBestiaryInfoElement("Example Minion Boss that spawns minions on spawn, summoned with a spawn item. Showcases boss minion handling, multiplayer considerations, and custom boss bar.")
			});
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			// Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

			// 顺序 in which you add loot will appear as such 在 Bestiary. To mirror vanilla Boss 顺序:
			// 1. 奖杯
			// 2. Classic 模式 ("not expert")
			// 3. Expert 模式 (usually just the treasure bag)
			// 4. Master 模式 (relic first, 宠物 last, everything else inbetween)

			// Trophies are spawned with 1/10 概率
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Furniture.MinionBossTrophy>(), 10));

			// All the Classic 模式 drops here are based on "not expert", meaning we use .OnSuccess() to add them in到 规则, which then gets added
			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

			// 注意 we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal 模式
			// Boss masks are spawned with 1/7 概率
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MinionBossMask>(), 7));

			// This part is not required for a Boss and is just showcasing some advanced stuff you can do with 放下 rules to 控制 how items 生成
			// 我们 make 12-15 ExampleItems 生成 randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne 规则,
			// which requires these parameters to be defined
			int itemType = ModContent.ItemType<ExampleItem>();
			var parameters = new DropOneByOne.Parameters() {
				ChanceNumerator = 1,
				ChanceDenominator = 1,
				MinimumStackPerChunkBase = 1,
				MaximumStackPerChunkBase = 1,
				MinimumItemDropsCount = 12,
				MaximumItemDropsCount = 15,
			};

			notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));

			// 最后 add the leading 规则
			npcLoot.Add(notExpertRule);

			// 添加 the treasure bag using ItemDropRule.BossBag (automatically checks for expert 模式)
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<MinionBossBag>()));

			// ItemDropRule.MasterModeCommonDrop 对于 relic
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.MinionBossRelic>()));

			// ItemDropRule.MasterModeDropOnAllPlayers 对于 宠物
			npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));
		}

		public override void OnKill() {
			// first 时间 this Boss is killed, 生成 ExampleOre in到 世界. This code is above SetEventFlagCleared because that will set downedMinionBoss to 真.
			if (!DownedBossSystem.downedMinionBoss) {
				ModContent.GetInstance<ExampleOreSystem>().BlessWorldWithExampleOre();
			}

			// This sets downedMinionBoss to 真, and if it was 假 before, it initiates a lantern night
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);

			// Since this hook is only ran in singleplayer and serverside, we would have to 同步 it manually.
			// Thankfully, vanilla sends the MessageID.WorldData 数据包 if a Boss was killed automatically, shortly after this hook is ran

			// 如果 your NPC is not a Boss and you need to 同步 the 世界 (which includes ModSystem, check DownedBossSystem), use this code:
			/*
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
			*/
		}

		public override void BossLoot(ref string name, ref int potionType) {
			// 在这里 you'd want to change the 药水 类型 that drops when the Boss is defeated. Because this Boss is early pre-hardmode, we keep it unchanged
			// (Lesser Healing 药水). If you wanted to change it, simply write "potionType = ItemID.HealingPotion;" or any other 药水 类型
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
			cooldownSlot = ImmunityCooldownID.Bosses; // use the Boss immunity cooldown 计数器, to 防止 ignoring Boss attacks by taking 伤害 from other sources
			return true;
		}

		public override void FindFrame(int frameHeight) {
			// This NPC animates with a simple "go from 开始 帧 to final 帧, and 循环 back to 开始 帧" 规则
			// 在 this case: First 阶段: 0-1-2-0-1-2, Second 阶段: 3-4-5-3-4-5, 5 being "total 帧 计数 - 1"
			int startFrame = 0;
			int finalFrame = 2;

			if (SecondStage) {
				startFrame = 3;
				finalFrame = Main.npcFrameCount[NPC.type] - 1;

				if (NPC.frame.Y < startFrame * frameHeight) {
					// 如果 we were animating the first 阶段 frames 然后 switch to second 阶段, immediately change 到 开始 帧 的 second 阶段
					NPC.frame.Y = startFrame * frameHeight;
				}
			}

			int frameSpeed = 5;
			NPC.frameCounter += 0.5f;
			NPC.frameCounter += NPC.velocity.Length() / 10f; // 使 the 计数器 go faster with more movement 速度
			if (NPC.frameCounter > frameSpeed) {
				NPC.frameCounter = 0;
				NPC.frame.Y += frameHeight;

				if (NPC.frame.Y > finalFrame * frameHeight) {
					NPC.frame.Y = startFrame * frameHeight;
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit) {
			// 如果 the NPC dies, 生成 gore and play a 声音
			if (Main.netMode == NetmodeID.Server) {
				// 我们 don't want Mod.查找<ModGore> to run on servers as it will crash because gores are not loaded on servers
				return;
			}

			if (NPC.life <= 0) {
				// These gores work by simply existing as a 纹理 inside any 文件夹 which 路径 contains "Gores/"
				int backGoreType = Mod.Find<ModGore>("MinionBossBody_Back").Type;
				int frontGoreType = Mod.Find<ModGore>("MinionBossBody_Front").Type;

				var entitySource = NPC.GetSource_Death();

				for (int i = 0; i < 2; i++) {
					Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), backGoreType);
					Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), frontGoreType);
				}

				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

				// This adds a 屏幕 shake (screenshake) similar to Deerclops
				PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, FullName);
				Main.instance.CameraModifiers.Add(modifier);
			}
		}

		public override void AI() {
			// This should almost always be the first code in AI() as it is responsible for finding the proper 玩家 目标
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) {
				NPC.TargetClosest();
			}

			Player player = Main.player[NPC.target];

			if (player.dead) {
				// 如果 the targeted 玩家 is dead, flee
				NPC.velocity.Y -= 0.04f;
				// 此方法 makes it so when the Boss is in "despawn 范围" (outside 的 屏幕), it despawns in 10 ticks
				NPC.EncourageDespawn(10);
				return;
			}

			SpawnMinions();

			CheckSecondStage();

			// Be invulnerable during the first 阶段
			NPC.dontTakeDamage = !SecondStage;

			if (SecondStage) {
				DoSecondStage(player);
			}
			else {
				DoFirstStage(player);
			}
		}

		private void SpawnMinions() {
			if (SpawnedMinions) {
				// No 点 executing the code in this 方法 again
				return;
			}

			SpawnedMinions = true;

			if (Main.netMode == NetmodeID.MultiplayerClient) {
				// Because we want to 生成 minions, and minions are NPCs, we have to do this 在 服务器 (or singleplayer, "!= NetmodeID.MultiplayerClient" covers both)
				// This means we also have to 同步 it after we spawned and set up the 仆从
				return;
			}

			int count = MinionCount();
			var entitySource = NPC.GetSource_FromAI();

			MinionMaxHealthTotal = 0;
			for (int i = 0; i < count; i++) {
				NPC minionNPC = NPC.NewNPCDirect(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MinionBossMinion>(), NPC.whoAmI);
				if (minionNPC.whoAmI == Main.maxNPCs)
					continue; // 生成 failed due to 生成 cap

				// Now th在 仆从 is spawned, we need to prepare it with 数据 即 necessary for it to work
				// 这是 not required usually if you simply 生成 NPCs, but because the 仆从 is tied 到 body, we need to pass this information to it
				MinionBossMinion minion = (MinionBossMinion)minionNPC.ModNPC;
				minion.ParentIndex = NPC.whoAmI; // Let the 仆从 know who the "parent" is
				minion.PositionOffset = i / (float)count; // Give it a 分离 位置 偏移

				MinionMaxHealthTotal += minionNPC.lifeMax; // 添加 the total 仆从 life for Boss 条 shield 文本

				// 最后, syncing, only 同步 on 服务器 and if the NPC actually exists (Main.maxNPCs is the 索引 of a dummy NPC, there is no 点 syncing it)
				if (Main.netMode == NetmodeID.Server) {
					NetMessage.SendData(MessageID.SyncNPC, number: minionNPC.whoAmI);
				}
			}

			// 同步 MinionMaxHealthTotal
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
			}
		}

		private void CheckSecondStage() {
			MinionHealthTotal = 0;
			if (SecondStage) {
				// No 点 checking if the NPC is already in its second 阶段
				return;
			}

			foreach (var otherNPC in Main.ActiveNPCs) {
				if (otherNPC.type == MinionType() && otherNPC.ModNPC is MinionBossMinion minion) {
					if (minion.ParentIndex == NPC.whoAmI) {
						MinionHealthTotal += otherNPC.life;
					}
				}
			}

			if (MinionHealthTotal <= 0 && Main.netMode != NetmodeID.MultiplayerClient) {
				// 如果 we have no shields (aka "no minions alive"), we initiate the second 阶段, and notify other players that this NPC has reached its second 阶段
				// by 设置 NPC.netUpdate to 真 in this tick. It will send important 数据 like 位置, 速度 and the NPC.ai[] 数组 to all connected clients

				// Because SecondStage is a 属性 using NPC.ai[], it will get synced this way
				SecondStage = true;
				NPC.netUpdate = true;
			}
		}

		private void DoFirstStage(Player player) {
			// 每次 the 计时器 is 0, pick a 随机 位置 a fixed 距离 away 从 玩家 but towards the opposite side
			// NPC moves directly towards it with fixed 速度, while displaying its trajectory as a telegraph

			FirstStageTimer++;
			if (FirstStageTimer > FirstStageTimerMax) {
				FirstStageTimer = 0;
			}

			float distance = 200; // 距离 in pixels behind the 玩家

			if (FirstStageTimer == 0) {
				Vector2 fromPlayer = NPC.Center - player.Center;

				if (Main.netMode != NetmodeID.MultiplayerClient) {
					// 重要 multiplayer consideration: drastic change in behavior (即 also decided by randomness) like this requires
					// to be executed 在 服务器 (or singleplayer) to keep the Boss in 同步

					float angle = fromPlayer.ToRotation();
					float twelfth = MathHelper.Pi / 6;

					angle += MathHelper.Pi + Main.rand.NextFloat(-twelfth, twelfth);
					if (angle > MathHelper.TwoPi) {
						angle -= MathHelper.TwoPi;
					}
					else if (angle < 0) {
						angle += MathHelper.TwoPi;
					}

					Vector2 relativeDestination = angle.ToRotationVector2() * distance;

					FirstStageDestination = player.Center + relativeDestination;
					NPC.netUpdate = true;
				}
			}

			// 移动 along the vector
			Vector2 toDestination = FirstStageDestination - NPC.Center;
			Vector2 toDestinationNormalized = toDestination.SafeNormalize(Vector2.UnitY);
			float speed = Math.Min(distance, toDestination.Length());
			NPC.velocity = toDestinationNormalized * speed / 30;

			if (FirstStageDestination != LastFirstStageDestination) {
				// 如果 destination changed
				NPC.TargetClosest(); // Pick the closest 玩家 目标 again

				// "Why is this not 在 same code that sets FirstStageDestination?" Because in multiplayer it's ran by the 服务器.
				// 客户端 has to know when the destination changes a different way. Keeping 跟踪 的 previous ticks' destination is one way
				if (Main.netMode != NetmodeID.Server) {
					// 对于 visuals regarding NPC 位置, netOffset has to be concidered to make visuals align properly
					NPC.position += NPC.netOffset;

					// 绘制 a line between the NPC and its destination, represented as dusts every 20 pixels
					Dust.QuickDustLine(NPC.Center + toDestinationNormalized * NPC.width, FirstStageDestination, toDestination.Length() / 20f, Color.Yellow);

					NPC.position -= NPC.netOffset;
				}
			}
			LastFirstStageDestination = FirstStageDestination;

			// No 伤害 during first 阶段
			NPC.damage = 0;

			// Fade in based on remaining total 仆从 life
			float remainingShields = MinionHealthTotal / (float)MinionMaxHealthTotal;
			NPC.alpha = (int)(remainingShields * 255);

			NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
		}

		private void DoSecondStage(Player player) {
			if (NPC.life < NPC.lifeMax * 0.5f) {
				ApplySecondStageBuffImmunities();
			}

			Vector2 toPlayer = player.Center - NPC.Center;

			float offsetX = 200f;

			Vector2 abovePlayer = player.Top + new Vector2(NPC.direction * offsetX, -NPC.height);

			Vector2 toAbovePlayer = abovePlayer - NPC.Center;
			Vector2 toAbovePlayerNormalized = toAbovePlayer.SafeNormalize(Vector2.UnitY);

			// NPC tries to go towards the offsetX 位置, but most likely it will never get there exactly, or 关闭 to if the 玩家 is moving
			// This checks if the npc is "70% there", 然后 changes 方向
			float changeDirOffset = offsetX * 0.7f;

			if (NPC.direction == -1 && NPC.Center.X - changeDirOffset < abovePlayer.X ||
				NPC.direction == 1 && NPC.Center.X + changeDirOffset > abovePlayer.X) {
				NPC.direction *= -1;
			}

			float speed = 8f;
			float inertia = 40f;

			// 如果 the Boss is somehow below the 玩家, 移动 faster to catch up
			if (NPC.Top.Y > player.Bottom.Y) {
				speed = 12f;
			}

			Vector2 moveTo = toAbovePlayerNormalized * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;

			DoSecondStage_SpawnEyes(player);

			NPC.damage = NPC.defDamage;

			NPC.alpha = 0;

			NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
		}

		private void DoSecondStage_SpawnEyes(Player player) {
			// At 100% 生命值, 生成 every 90 ticks
			// Drops down until 33% 生命值 to 生成 every 30 ticks
			float timerMax = Utils.Clamp((float)NPC.life / NPC.lifeMax, 0.33f, 1f) * 90;

			SecondStageTimer_SpawnEyes++;
			if (SecondStageTimer_SpawnEyes > timerMax) {
				SecondStageTimer_SpawnEyes = 0;
			}

			if (NPC.HasValidTarget && SecondStageTimer_SpawnEyes == 0 && Main.netMode != NetmodeID.MultiplayerClient) {
				// 生成 弹幕 randomly below 玩家, based on horizontal 速度 to make kiting harder, starting 速度 1f upwards
				// (The projectiles accelerate 从ir initial 速度)

				float kitingOffsetX = Utils.Clamp(player.velocity.X * 16, -100, 100);
				Vector2 position = player.Bottom + new Vector2(kitingOffsetX + Main.rand.Next(-100, 100), Main.rand.Next(50, 100));

				int type = ModContent.ProjectileType<MinionBossEye>();
				int damage = NPC.damage / 2;
				var entitySource = NPC.GetSource_FromAI();

				Projectile.NewProjectile(entitySource, position, -Vector2.UnitY, type, damage, 0f, Main.myPlayer);
			}
		}

		private void ApplySecondStageBuffImmunities() {
			if (NPC.buffImmune[BuffID.OnFire]) {
				return;
			}
			// Halfway through 阶段 2, this Boss becomes immune 到 OnFire 增益.
			// This code will only run once because 的 !NPC.buffImmune[BuffID.OnFire] check.
			// 如果 you make a similar check for just a life 百分比 in a Boss, you will need to use a bool to 跟踪 if the corresponding code has run yet or not.
			NPC.BecomeImmuneTo(BuffID.OnFire);

			// 最后, this Boss will 清除 all the buffs it currently has that it is now immune to. ClearImmuneToBuffs should 不 run on multiplayer clients, the 服务器 has authority over buffs.
			if (Main.netMode != NetmodeID.MultiplayerClient) {
				NPC.ClearImmuneToBuffs(out bool anyBuffsCleared);

				if (anyBuffsCleared) {
					// Since we cleared some fire related buffs, 生成 some smoke to communicate th在 fire buffs have been extinguished.
					// 此示例 is commented out because it would require a ModPacket to manually 同步 in 顺序 to work in multiplayer.
					/* for (int g = 0; g < 8; g++) {
						Gore gore = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.Center, default, Main.rand.Next(61, 64), 1f);
						gore.scale = 1.5f;
						gore.velocity += new Vector2(1.5f, 0).RotatedBy(g * MathHelper.PiOver2);
					}*/
				}
			}

			// 生成 a ring of dust to communicate the change.
			for (int loops = 0; loops < 2; loops++) {
				for (int i = 0; i < 50; i++) {
					Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
					Dust d = Dust.NewDustPerfect(NPC.Center, DustID.BlueCrystalShard, speed * 10 * (loops + 1), Scale: 1.5f);
					d.noGravity = true;
				}
			}
		}
	}
}
