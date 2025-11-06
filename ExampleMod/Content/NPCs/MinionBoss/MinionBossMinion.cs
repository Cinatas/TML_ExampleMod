using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.NPCs.MinionBoss
{
	// minions spawned when the body spawns
	// Please read MinionBossBody.cs first for important comments, they won't be explained here again
	public class MinionBossMinion : ModNPC
	{
		// 这是 a neat trick that uses the fact that NPCs have all NPC.ai[] values set to 0f on 生成 (如果不是 否则 changed).
		// 我们 set ParentIndex to a 数字 在 body after spawning it. If we set ParentIndex to 3, NPC.ai[0] 将 4. If NPC.ai[0] is 0, ParentIndex 将 -1.
		// Now 组合 两者 facts, and the conclusion is that if this NPC spawns by other means (not 从 body), ParentIndex 将 -1, allowing us to distinguish
		// between a proper 生成 and an invalid/"cheated" 生成
		public int ParentIndex {
			get => (int)NPC.ai[0] - 1;
			set => NPC.ai[0] = value + 1;
		}

		public bool HasParent => ParentIndex > -1;

		public float PositionOffset {
			get => NPC.ai[1];
			set => NPC.ai[1] = value;
		}

		public const float RotationTimerMax = 360;
		public ref float RotationTimer => ref NPC.ai[2];

		// Helper 方法 to determine the body 类型
		public static int BodyType() {
			return ModContent.NPCType<MinionBossBody>();
		}

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 1;

			// 默认情况下 enemies gain 生命值 and 攻击 if hardmode is reached. this NPC should 不 affected by that
			NPCID.Sets.DontDoHardmodeScaling[Type] = true;
			// Enemies can pick up coins, let's 防止 it for this NPC
			NPCID.Sets.CantTakeLunchMoney[Type] = true;
			// Automatically 分组 with other bosses
			NPCID.Sets.BossBestiaryPriority.Add(Type);

			// Specify the debuffs it is immune to. Most NPCs are immune to Confused.
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

			// 可选: If you don't want this NPC to show 在 bestiary (if there is no reason to show a Boss 仆从 separately)
			// 确保 to 删除 SetBestiary code 以及
			// NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers() {
			//	Hide = 真 // 隐藏s this NPC 从 bestiary
			// };
			// NPCID.Sets.NPCBestiaryDrawOffset.Add(类型, bestiaryData);
		}

		public override void SetDefaults() {
			NPC.width = 30;
			NPC.height = 30;
			NPC.damage = 7;
			NPC.defense = 0;
			NPC.lifeMax = 50;
			NPC.HitSound = SoundID.NPCHit9;
			NPC.DeathSound = SoundID.NPCDeath11;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.knockBackResist = 0.8f;
			NPC.alpha = 255; // 这使 it transparent upon spawning, we 必须 manually fade it in in AI()
			NPC.netAlways = true;

			NPC.aiStyle = -1;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// 使 it so whenever you be在 Boss associated with it, it will also get unlocked immediately
			int associatedNPCType = BodyType();
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black 背景
				new FlavorTextBestiaryInfoElement("A minion protecting his boss from taking damage by sacrificing itself. If none are alive, the boss is exposed to damage.")
			});
		}

		public override Color? GetAlpha(Color drawColor) {
			if (NPC.IsABestiaryIconDummy) {
				// 这是 required because we have NPC.alpha = 255, 在 bestiary it would look transparent
				return NPC.GetBestiaryEntryColor();
			}
			return Color.White * NPC.Opacity;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
			cooldownSlot = ImmunityCooldownID.Bosses; // use the Boss immunity cooldown 计数器, to 防止 ignoring Boss attacks by taking 伤害 from other sources
			return true;
		}

		public override void OnKill() {
			// Boss minions typically have a 概率 to 放下 an additional heart 项 另外 到 default 概率
			Player closestPlayer = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];

			if (Main.rand.NextBool(2) && closestPlayer.statLife < closestPlayer.statLifeMax2) {
				Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart);
			}
		}

		public override void HitEffect(NPC.HitInfo hit) {
			if (NPC.life <= 0) {
				// 如果 this NPC dies, 生成 some visuals

				int dustType = 59; // Some blue dust, read the dust guide 在 wiki for how to 查找 the perfect dust

				for (int i = 0; i < 20; i++) {
					Vector2 velocity = NPC.velocity + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
					Dust dust = Dust.NewDustPerfect(NPC.Center, dustType, velocity, 26, Color.White, Main.rand.NextFloat(1.5f, 2.4f));

					dust.noLight = true;
					dust.noGravity = true;
					dust.fadeIn = Main.rand.NextFloat(0.3f, 0.8f);
				}
			}
		}

		public override void AI() {
			if (Despawn()) {
				return;
			}

			FadeIn();

			MoveInFormation();
		}

		private bool Despawn() {
			if (Main.netMode != NetmodeID.MultiplayerClient &&
				(!HasParent || !Main.npc[ParentIndex].active || Main.npc[ParentIndex].type != BodyType())) {
				// * Not spawned by the Boss body (didn't assign a 位置 and parent) or
				// * Parent isn't active or
				// * Parent isn't the body
				// => invalid, kill itself without dropping 任何 items
				NPC.active = false;
				NPC.life = 0;
				NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
				return true;
			}
			return false;
		}

		private void FadeIn() {
			// Fade in (we have NPC.alpha = 255 in SetDefaults which means it spawns transparent)
			if (NPC.alpha > 0) {
				NPC.alpha -= 10;
				if (NPC.alpha < 0) {
					NPC.alpha = 0;
				}
			}
		}

		private void MoveInFormation() {
			NPC parentNPC = Main.npc[ParentIndex];

			// This basically turns the NPCs PositionIndex into a 数字 between 0f and TwoPi to determine where around
			// 主要的 body it is positioned at
			float rad = (float)PositionOffset * MathHelper.TwoPi;

			// 添加 some slight uniform 旋转 to make the eyes 移动, giving a 概率 to 触摸 the 玩家 and 因此 helping melee players
			RotationTimer += 0.5f;
			if (RotationTimer > RotationTimerMax) {
				RotationTimer = 0;
			}

			// Since RotationTimer is in degrees (0..360) 我们可以 convert it to radians (0..TwoPi) easily
			float continuousRotation = MathHelper.ToRadians(RotationTimer);
			rad += continuousRotation;
			if (rad > MathHelper.TwoPi) {
				rad -= MathHelper.TwoPi;
			}
			else if (rad < 0) {
				rad += MathHelper.TwoPi;
			}

			float distanceFromBody = parentNPC.width + NPC.width;

			// 偏移 is now a vector that will determine the 位置 的 NPC 基于 its 索引
			Vector2 offset = Vector2.One.RotatedBy(rad) * distanceFromBody;

			Vector2 destination = parentNPC.Center + offset;
			Vector2 toDestination = destination - NPC.Center;
			Vector2 toDestinationNormalized = toDestination.SafeNormalize(Vector2.Zero);

			float speed = 8f;
			float inertia = 20;

			Vector2 moveTo = toDestinationNormalized * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;
		}
	}
}
