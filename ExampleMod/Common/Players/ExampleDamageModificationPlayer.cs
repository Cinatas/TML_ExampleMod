using ExampleMod.Content.Buffs;
using ExampleMod.Content.Items.Accessories;
using ExampleMod.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	internal class ExampleDamageModificationPlayer : ModPlayer
	{
		public float AdditiveCritDamageBonus;

		// 这 3 个字段与示例闪避相关。示例闪避是根据神圣套装奖励的闪避能力建模的。
		// 示例Dodge 指示玩家是否主动具有闪避下一次攻击的能力。这由 ExampleDodgeBuff 设置，在此示例中由 HitModifiersShowcase 武器应用。仅当 exampleDodgeCooldown 为 0 时才应用增益，如果闪避攻击或玩家不再持有 HitModifiersShowcase，则会自动清除。
		public bool exampleDodge; // 待办事项： Example of custom 玩家 render
		// 用于在消耗示例闪避和下次可以获得闪避增益之间添加延迟。
		public int exampleDodgeCooldown;
		// Controls the intensity 的 visual 效果 的 dodge.
		public int exampleDodgeVisualCounter;

		// If this 玩家 has an 饰品 which gives this 效果
		public bool hasAbsorbTeamDamageEffect;
		// If the 玩家 is currently in 范围 of a 玩家 with hasAbsorbTeamDamageEffect
		public bool defendedByAbsorbTeamDamageEffect;

		public bool exampleDefenseDebuff;

		public override void PreUpdate() {
			// Timers and cooldowns 应该 adjusted in PreUpdate
			if (exampleDodgeCooldown > 0) {
				exampleDodgeCooldown--;
			}
		}

		public override void ResetEffects() {
			AdditiveCritDamageBonus = 0f;

			exampleDodge = false;

			hasAbsorbTeamDamageEffect = false;
			defendedByAbsorbTeamDamageEffect = false;

			exampleDefenseDebuff = false;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
			if (AdditiveCritDamageBonus > 0) {
				modifiers.CritDamage += AdditiveCritDamageBonus;
			}
		}

		public override void PostUpdateEquips() {
			// If the conditions 对于 玩家 having the 增益 are 不再 真, 删除 the 增益.
			// This could could technically go in ExampleDodgeBuff.更新, but typically these effects are given by 护甲 or accessories, so showing this example here is more useful.
			if (exampleDodge && Player.HeldItem.type != ModContent.ItemType<HitModifiersShowcase>()) {
				Player.ClearBuff(ModContent.BuffType<ExampleDodgeBuff>());
			}

			// 示例DodgeVisualCounter 应该 updated here, not in DrawEffects, to work properly
			exampleDodgeVisualCounter = Math.Clamp(exampleDodgeVisualCounter + (exampleDodge ? 1 : -1), 0, 30);
		}

		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright) {
			// 示例DodgeVisualCounter helps fade the 颜色 效果 in and out.
			if (exampleDodgeVisualCounter > 0) {
				g = Math.Max(0, g - exampleDodgeVisualCounter * 0.03f);
			}

			if (exampleDefenseDebuff) {
				// These 颜色 adjustments 匹配 the withered 护甲 减益 visuals.
				g *= 0.5f;
				r *= 0.75f;
			}
		}

		public override bool ConsumableDodge(Player.HurtInfo info) {
			if (exampleDodge) {
				ExampleDodgeEffects();
				return true;
			}

			return false;
		}

		// 示例DodgeEffects() 将 called from ConsumableDodge and HandleExampleDodgeMessage to 同步 the 效果.
		public void ExampleDodgeEffects() {
			Player.SetImmuneTimeForAllTypes(Player.longInvince ? 120 : 80);

			// Some 声音 and visual effects
			for (int i = 0; i < 50; i++) {
				Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
				Dust d = Dust.NewDustPerfect(Player.Center + speed * 16, DustID.BlueCrystalShard, speed * 5, Scale: 1.5f);
				d.noGravity = true;
			}
			SoundEngine.PlaySound(SoundID.Shatter with { Pitch = 0.5f });

			// The visual and 声音 effects happen on all clients, but the code below only runs 对于 dodging 玩家 
			if (Player.whoAmI != Main.myPlayer) {
				return;
			}

			// 清除ing the 增益 and assigning the cooldown 时间
			Player.ClearBuff(ModContent.BuffType<ExampleDodgeBuff>());
			exampleDodgeCooldown = 180; // 3 second cooldown before the 增益 可以 given again.

			if (Main.netMode != NetmodeID.SinglePlayer) {
				SendExampleDodgeMessage(Player.whoAmI);
			}
		}

		public static void HandleExampleDodgeMessage(BinaryReader reader, int whoAmI) {
			int player = reader.ReadByte();
			if (Main.netMode == NetmodeID.Server) {
				player = whoAmI;
			}

			Main.player[player].GetModPlayer<ExampleDamageModificationPlayer>().ExampleDodgeEffects();

			if (Main.netMode == NetmodeID.Server) {
				// If the 服务器 receives this 消息, it sends it to all other clients to 同步 the effects.
				SendExampleDodgeMessage(player);
			}
		}

		public static void SendExampleDodgeMessage(int whoAmI) {
			// This code is called by both the initial 
			ModPacket packet = ModContent.GetInstance<ExampleMod>().GetPacket();
			packet.Write((byte)ExampleMod.MessageType.ExampleDodge);
			packet.Write((byte)whoAmI);
			packet.Send(ignoreClient: whoAmI);
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers) {
			if (defendedByAbsorbTeamDamageEffect && Player == Main.LocalPlayer && TeammateCanAbsorbDamage()) {
				modifiers.FinalDamage *= 1f - AbsorbTeamDamageAccessory.DamageAbsorptionMultiplier;
			}
		}

		public override void OnHurt(Player.HurtInfo info) {
			// On Hurt is used in this example to act upon another 玩家 being hurt.
			// If the 玩家 who was hurt was defended, check if the local 玩家 should take the remaining 伤害 对于m
			Player localPlayer = Main.LocalPlayer;
			if (defendedByAbsorbTeamDamageEffect && Player != localPlayer && IsClosestShieldWearerInRange(localPlayer, Player.Center, Player.team)) {
				// The intention of AbsorbTeamDamageAccessory is to transfer 30% of 伤害 taken by teammates 到 wearer.
				// In ModifiedHurt, we reduce the 伤害 by 30%. The resulting reduced 伤害 is passed to OnHurt, where the 玩家 wearing AbsorbTeamDamageAccessory hurts themselves.
				// Since OnHurt is provided 与 伤害 already reduced by 30%, we need to reverse the math to determine how much the 伤害 was originally reduced by
				// Working through the math, the amount of 伤害 that was reduced is equal to: 伤害 * (percent / (1 - percent))
				float percent = AbsorbTeamDamageAccessory.DamageAbsorptionMultiplier;
				int damage = (int)(info.Damage * (percent / (1 - percent)));

				// 不要 bother pinging the defending 玩家 and upsetting their immunity frames if the portion of 伤害 we're taking rounds down to 0
				if (damage > 0) {
					localPlayer.Hurt(PlayerDeathReason.LegacyEmpty(), damage, 0);
				}
			}
		}

		private bool TeammateCanAbsorbDamage() {
			foreach (var otherPlayer in Main.ActivePlayers) {
				if (otherPlayer.whoAmI != Main.myPlayer && IsAbleToAbsorbDamageForTeammate(otherPlayer, Player.team)) {
					return true;
				}
			}
			return false;
		}

		private static bool IsAbleToAbsorbDamageForTeammate(Player player, int team) {
			return player.active
				&& !player.dead
				&& !player.immune // This check 可以 removed, allowing players to take hits for 团队-mates in quick succession. Removing it can also 帮助 with de-syncs where the 玩家 getting hurt thinks there is no-one to tank the 伤害, but by the 时间 the hit arrives 在 玩家 与 shield, they take extra 伤害
				&& player.GetModPlayer<ExampleDamageModificationPlayer>().hasAbsorbTeamDamageEffect
				&& player.team == team
				&& player.statLife > player.statLifeMax2 * AbsorbTeamDamageAccessory.DamageAbsorptionAbilityLifeThreshold;
		}

		// This code finds the closest 玩家 wearing AbsorbTeamDamageAccessory. 
		private static bool IsClosestShieldWearerInRange(Player player, Vector2 target, int team) {
			if (!IsAbleToAbsorbDamageForTeammate(player, team)) {
				return false;
			}

			float distance = player.Distance(target);
			if (distance > AbsorbTeamDamageAccessory.DamageAbsorptionRange) {
				return false; // 玩家 we're out of 范围, so can't take the hit
			}

			foreach (var otherPlayer in Main.ActivePlayers) {
				if (otherPlayer.whoAmI != Main.myPlayer && IsAbleToAbsorbDamageForTeammate(otherPlayer, team)) {
					float otherPlayerDistance = otherPlayer.Distance(target);
					if (distance > otherPlayerDistance || (distance == otherPlayerDistance && otherPlayer.whoAmI < Main.myPlayer)) {
						return false;
					}
				}
			}

			return true;
		}
	}
}
