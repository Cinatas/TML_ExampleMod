using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.NPCs
{
	// This NPC inherits 从 悬停 abstract 类 included in ExampleMod, 即 a more customizable 复制 的 vanilla Hovering AI.
	// It implements the `CustomBehavior` and `ShouldMove` virtual methods being overridden here, 以及 as the `acceleration` and `accelerationY` 字段 being set 在 类 constructor.
	public class FallenSoul : Hover
	{
		public FallenSoul() {
			acceleration = 0.06f;
			accelerationY = 0.025f;
		}

		public override void SetStaticDefaults() {
			Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.Wraith];
		}

		public override void SetDefaults() {
			npc.CloneDefaults(NPCID.Wraith);
			npc.width = 28;
			npc.height = 36;
			npc.aiStyle = -1;
			npc.damage = 0;
			npc.friendly = true;
			npc.dontTakeDamageFromHostiles = true;
			animationType = NPCID.Wraith;
		}

		// 允许s hitting the NPC with melee 类型 weapons, even if it's friendly.
		public override bool? CanBeHitByItem(Player player, Item item) {
			return true;
		}

		// Same as the above but with projectiles.
		public override bool? CanBeHitByProjectile(Projectile projectile) {
			return projectile.friendly && projectile.owner < 255;
		}

		public override void HitEffect(int hitDirection, double damage) {
			if (npc.life > 0) {
				for (int i = 0; i < damage / npc.lifeMax * 100; i++) {
					Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 192, hitDirection, -1f, 100, new Color(100, 100, 100, 100), 1f);
					dust.noGravity = true;
				}
				return;
			}
			for (int i = 0; i < 50; i++) {
				Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 192, 2 * hitDirection, -2f, 100, new Color(100, 100, 100, 100), 1f);
				dust.noGravity = true;
			}
		}

		// 允许s the NPC to talk 与 玩家, even if it isn't a town NPC.
		public override bool CanChat() {
			return true;
		}

		public override string GetChat() {
			// npc.SpawnedFromStatue 值 is kept when the NPC is transformed.
			switch (Main.rand.Next(npc.SpawnedFromStatue ? 5 : 3)) {
				case 0:
					return "Thank you, now i don't have to haunt random people anymore, only you.";
				case 1:
					return "Keep breaking those evil altars, me and many others were cursed to haunt anyone who did so.";
				case 2:
					return "Can you help me get into heaven?";
				default:
					return "Please stop messing with that haunted statue. Don't you know what \"RIP\" means?";
			}
		}

		public override void SetChatButtons(ref string button, ref string button2) {
			button = "Send to heaven";
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop) {
			if (firstButton) {
				// Hit the NPC for about 500 伤害
				Main.LocalPlayer.ApplyDamageToNPC(npc, Main.DamageVar(500), 5f, Main.LocalPlayer.direction, true);
			}
		}

		// 仅 show 生命值 条 的 NPC when 关闭 到 玩家
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
			float distance = npc.Distance(Main.player[npc.target].Center);
			if (distance <= 200) {
				if (distance > 100) {
					// 使 the 生命值 条 become smaller the farther away the NPC is.
					scale *= (100 - (distance - 100)) / 100;
				}
				return null;
			}
			return false;
		}

		// 使 the NPC invisible when far away 从 玩家.
		public override void CustomBehavior(ref float ai) {
			float distance = npc.Distance(Main.player[npc.target].Center);
			if (distance <= 250) {
				npc.alpha = 100;
				if (distance > 100) {
					// 使 the NPC fade out the farther away the NPC is.
					npc.alpha += (int)(155 * ((distance - 100) / 150));
				}
				return;
			}
			npc.alpha = 255;
		}

		// 使 the NPC 停止 moving if it is 关闭 到 玩家.
		public override bool ShouldMove(float ai) {
			npc.ai[2] = 0; // 防止s the NPC from stopping following their 目标.
			if (npc.Distance(Main.player[npc.target].Center) < 150f) {
				npc.velocity *= 0.95f;
				if (Math.Abs(npc.velocity.X) < 0.1f) {
					npc.spriteDirection = Main.player[npc.target].Center.X > npc.Center.X ? 1 : -1;
					npc.velocity.X = 0;
				}
				return false;
			}
			return true;
		}
	}

	public class PurificationPowder : GlobalProjectile
	{
		// 使 purification powder transform wraiths into purified ghosts.
		public override void PostAI(Projectile projectile) {
			if (projectile.type != ProjectileID.PurificationPowder || Main.netMode == NetmodeID.MultiplayerClient) {
				return;
			}

			for (int i = 0; i < Main.maxNPCs; i++) {
				if (Main.npc[i].active && Main.npc[i].type == NPCID.Wraith && projectile.Hitbox.Intersects(Main.npc[i].Hitbox)) {
					Main.npc[i].Transform(NPCType<FallenSoul>());
				}
			}
		}
	}
}
