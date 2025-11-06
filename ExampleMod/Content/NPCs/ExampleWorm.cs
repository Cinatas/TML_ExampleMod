using ExampleMod.NPCs;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.NPCs
{
	// These three class showcase usage 的 WormHead, WormBody and WormTail classes from Worm.cs
	internal class ExampleWormHead : WormHead
	{
		public override int BodyType => ModContent.NPCType<ExampleWormBody>();

		public override int TailType => ModContent.NPCType<ExampleWormTail>();

		public override void SetStaticDefaults() {
			var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers() { // Influences how the NPC looks 在 Bestiary
				CustomTexturePath = "ExampleMod/Content/NPCs/ExampleWorm_Bestiary", // If the NPC is multiple parts like a worm, a custom texture 对于 Bestiary is encouraged.
				Position = new Vector2(40f, 24f),
				PortraitPositionXOverride = 0f,
				PortraitPositionYOverride = 12f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
		}

		public override void SetDefaults() {
			// Head is 10 defense, body 20, tail 30.
			NPC.CloneDefaults(NPCID.DiggerHead);
			NPC.aiStyle = -1;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// 我们 can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// 设置s the spawning conditions of this NPC 即 listed 在 bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// 设置s the description of this NPC 即 listed 在 bestiary.
				new FlavorTextBestiaryInfoElement("Looks like a Digger fell into some aqua-colored paint. Oh well.")
			});
		}

		public override void Init() {
			// 设置 the segment variance
			// 如果 you want the segment length to be constant, set these two properties 到 same value
			MinSegmentLength = 6;
			MaxSegmentLength = 12;

			CommonWormInit(this);
		}

		// 此方法 is invoked from ExampleWormHead, ExampleWormBody and ExampleWormTail
		internal static void CommonWormInit(Worm worm) {
			// These two properties handle the movement 的 worm
			worm.MoveSpeed = 5.5f;
			worm.Acceleration = 0.045f;
		}

		private int attackCounter;
		public override void SendExtraAI(BinaryWriter writer) {
			writer.Write(attackCounter);
		}

		public override void ReceiveExtraAI(BinaryReader reader) {
			attackCounter = reader.ReadInt32();
		}

		public override void AI() {
			if (Main.netMode != NetmodeID.MultiplayerClient) {
				if (attackCounter > 0) {
					attackCounter--; // tick down the attack counter.
				}

				Player target = Main.player[NPC.target];
				// 如果 the attack counter is 0, this NPC is less than 12.5 tiles away from its target, and has a path 到 target unobstructed by blocks, summon a projectile.
				if (attackCounter <= 0 && Vector2.Distance(NPC.Center, target.Center) < 200 && Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1)) {
					Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
					direction = direction.RotatedByRandom(MathHelper.ToRadians(10));

					int projectile = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 1, ProjectileID.ShadowBeamHostile, 5, 0, Main.myPlayer);
					Main.projectile[projectile].timeLeft = 300;
					attackCounter = 500;
					NPC.netUpdate = true;
				}
			}
		}
	}

	internal class ExampleWormBody : WormBody
	{
		public override void SetStaticDefaults() {
			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				Hide = true // 隐藏s this NPC 从 Bestiary, useful for multi-part NPCs whom you only want one entry.
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
		}

		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.DiggerBody);
			NPC.aiStyle = -1;
		}

		public override void Init() {
			ExampleWormHead.CommonWormInit(this);
		}
	}

	internal class ExampleWormTail : WormTail
	{
		public override void SetStaticDefaults() {
			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				Hide = true // 隐藏s this NPC 从 Bestiary, useful for multi-part NPCs whom you only want one entry.
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
		}

		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.DiggerTail);
			NPC.aiStyle = -1;
		}

		public override void Init() {
			ExampleWormHead.CommonWormInit(this);
		}
	}
}
