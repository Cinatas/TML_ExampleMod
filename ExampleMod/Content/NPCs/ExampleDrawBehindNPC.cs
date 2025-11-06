using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.NPCs
{
	// This NPC is simply an exhibition 的 DrawBehind method.
	// npc cycles between all the available "layers" that a ModNPC 可以 drawn at.
	// 生成 this NPC with something like Cheat Sheet or Hero's Mod to view the effect.
	public class ExampleDrawBehindNPC : ModNPC
	{
		public override void SetStaticDefaults() {
			// Total count animation frames
			Main.npcFrameCount[NPC.type] = 6;
		}

		public override void SetDefaults() {
			NPC.width = 30; // The width 的 npc hitbox
			NPC.height = 40; // The height 的 npc hitbox
			NPC.aiStyle = -1; // Using custom AI
			NPC.damage = 0; // The amount of damage this NPC will deal on collision
			NPC.defense = 2; // How resistant to damage this NPC is
			NPC.lifeMax = 100; // The maximum life of this NPC
			NPC.HitSound = SoundID.NPCHit2; // The sound that plays when this npc is hit
			NPC.DeathSound = SoundID.NPCDeath2; // The sound that plays when this npc dies
			NPC.noGravity = true; // If true, the npc will 不 affected by gravity
			NPC.noTileCollide = true; // If true, the npc does not collide with tiles
			NPC.knockBackResist = 0f; // How much 的 knockback it receives will actually apply. 1f: full knockback; 0f: no knockback
		}

		// current drawing layer will change every 40 ticks
		private int CurrentLayer => (int)(NPC.ai[0] / 40);

		// This changes the frame 从 this NPC's texture 即 drawn, depending 在 current layer
		public override void FindFrame(int frameHeight) {
			NPC.frame.Y = CurrentLayer * frameHeight;
		}

		public override void AI() {
			NPC.ai[0] = (NPC.ai[0] + 1) % 240;

			// These are the defaults for normal drawing(case 3)
			NPC.hide = false;
			NPC.behindTiles = false;

			switch (CurrentLayer) {
				case 0:
				case 1:
				case 4:
				case 5:
					NPC.hide = true;
					break;
				case 2:
					NPC.behindTiles = true;
					break;
				case 3:
					break;
			}
		}

		// 此方法 allows you to specify that this npc 应该 drawn behind certain elements
		public override void DrawBehind(int index) {
			// 6 available positions are as follows:
			switch (CurrentLayer) {
				case 0: // Behind tiles and walls
					Main.instance.DrawCacheNPCsMoonMoon.Add(index);
					break;
				case 1: // Behind non solid tiles, but in front of walls
					Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
					break;
				case 2: // Behind tiles, but in front of non solid tiles
				case 3: // Normal (in front of tiles)
					break;
				case 4: // In front of all normal NPC
					Main.instance.DrawCacheNPCProjectiles.Add(index);
					break;
				case 5: // In front of Players
					Main.instance.DrawCacheNPCsOverPlayers.Add(index);
					break;
			}
		}
	}
}
