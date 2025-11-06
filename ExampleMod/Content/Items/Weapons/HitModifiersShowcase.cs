using ExampleMod.Common.Players;
using ExampleMod.Content.Buffs;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	/// <summary>
	/// This 项 can 帮助 conceptualize 各种 伤害 modification concepts. <br/>
	/// The 项.伤害 of this 武器 is 100 so the math is easy to follow. 伤害 variation is disabled for all modes except the 1st 模式 对于 same reason. <br/>
	/// When testing this weap在 first 时间, it is recommended to 禁用 other mods and to 删除 all 伤害 boosting accessories, as they will complicate the math being taught. <br/>
	/// Testing against <see cref="NPCID.BlueArmoredBonesNoPants"/> is recommended as it has high 防御 (50), good knockback resistance, and enough 生命值 for 一些 hits. Having 50 防御 makes the math for 防御 and 护甲 penetration easy to follow.
	/// <br/>
	/// The math taught in this example also assumes the 玩家 is in a normal 世界. <br/> 
	/// Use 右 点击 to switch modes.<br/>
	/// This example is purely for demonstration purposes only, it will not work in multiplayer. This should also 不 considered correct code for a working dual-use 武器. <br/>
	/// </summary>
	public class HitModifiersShowcase : ModItem
	{
		public override string Texture => "ExampleMod/Content/Items/Weapons/ExampleSword";

		private const int numberOfModes = 8;
		private int mode = 0;

		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item1;

			Item.DamageType = DamageClass.Melee;
			Item.damage = 100;
			Item.knockBack = 5;
			Item.crit = 10;
		}

		public override void NetSend(BinaryWriter writer) {
			writer.Write((byte)mode);
		}

		public override void NetReceive(BinaryReader reader) {
			mode = reader.ReadByte();
		}

		public override bool AltFunctionUse(Player player) {
			return true;
		}

		public override bool? UseItem(Player player) {
			if (player.whoAmI != Main.myPlayer) {
				return true;
			}

			if (player.altFunctionUse == 2) {
				mode++;
				if (mode >= numberOfModes) {
					mode = 0;
				}
				Main.NewText($"Switching to mode #{mode}: {GetMessageForMode()}");
				// This line will 触发器 NetSend to be called 在 结束 of this game 更新, allowing the changes to useStyle to be in 同步. 
				Item.NetStateChanged();
			}
			else {
				Main.NewText($"Mode #{mode}: {GetMessageForMode()}");
			}
			return true;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) {
			if (!Main.rand.NextBool(3))
				return;
			Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Firework_Red + mode);
		}

		private string GetMessageForMode() {
			switch (mode) {
				case 0:
					return "Normal damage behavior";
				case 1:
					return "Damage variation disabled";
				case 2:
					return "50% extra knockback";
				case 3:
					return "200% extra critical hit damage";
				case 4:
					return "10 extra armor penetration. Test against high defense enemy";
				case 5:
					// 这是 similar 到 Lightning Aura and Flymeal 武器 effects
					return "50% extra armor penetration. Ignores 50% of enemy defense";
				case 6:
					return "Will apply ExampleDefenseDebuff, reducing defense by 25%";
				case 7:
					return "On hit, gives player ExampleDodgeBuff to dodge the next hit";

			}
			return "Unknown mode";
		}

		public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers) {
			// These effects modify the hit itself, so they 需要 be in this 方法.
			if (mode != 0) {
				modifiers.DamageVariationScale *= 0f;
			}
			if (mode == 2) {
				modifiers.Knockback += .5f;
			}
			else if (mode == 3) {
				modifiers.CritDamage += 2f; // 默认 crit is 100% more than a normal hit, so with this in 效果, crits should deal 4x 伤害
			}
			else if (mode == 4) {
				modifiers.ArmorPenetration += 10f;
			}
			else if (mode == 5) {
				modifiers.ScalingArmorPenetration += 0.5f;
			}

			// Below is an example of using ModifyHitInfo to alter the final 值 of 伤害, between Modify and OnHit hooks.
			// This 'backdoor' is a replacement 对于 old style of modifiers which allowed modifying the 伤害 via `ref`
			// Please only use this if absolutely necessary, as 多个 mods freely altering the 伤害 results will create incompatible or unintuitive 玩家 experiences.
			//
			// 对于 example, the 效果 below 可能 better implemented by checking `玩家.GetWeaponDamage(项)` and adding to FinalDamage.Base, SourceDamage.Base, SourceDamage.Flat or FlatBonusDamage
			/*
			modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) => {
				if (hitInfo.Damage > 10) {
					hitInfo.Damage += 5;
				}
			};
			*/
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
			// These effects act on a hit happening, so they should go here.
			// Buffs added locally are automatically synced 到 服务器 and other players in multiplayer
			if (mode == 6) {
				target.AddBuff(ModContent.BuffType<ExampleDefenseDebuff>(), 600);
			}
			else if (mode == 7) {
				var damageModificationPlayer = player.GetModPlayer<ExampleDamageModificationPlayer>();
				if (damageModificationPlayer.exampleDodgeCooldown == 0) {
					player.AddBuff(ModContent.BuffType<ExampleDodgeBuff>(), 1800);
				}
			}
		}

		// Due 到 differences in pvp 伤害 calculations, only some 的 effects of this 武器 work in pvp.
		public override void ModifyHitPvp(Player player, Player target, ref Player.HurtModifiers modifiers) {
			// 不像 the effects in OnHitPvp, these specific effects 需要 run on all clients to keep things in 同步, so there is no check for local 玩家.
			if (mode == 2) {
				modifiers.Knockback += .5f;
			}
			else if (mode == 4) {
				modifiers.ArmorPenetration += 10f;
			}
			else if (mode == 5) {
				modifiers.ScalingArmorPenetration += 0.5f;
			}
		}

		public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) {
			// These effects of this 武器 should only run 在 玩家 damaging another, this check does that.
			if (player != Main.LocalPlayer) {
				return;
			}

			if (mode == 6) {
				// This AddBuff is not quiet because it is affecting another 玩家. This allows it to broadcast to all players th在 目标 has a 增益. (Main.pvpBuff 必须 set to 真 for other players to be 能够 give buffs to a 玩家)
				// 注意 that in PvP, it is possible to 攻击 a 玩家 and see them take 伤害, but by the 时间 the hit 消息 arrives 在 目标 客户端, they may have recharged a dodge. In this case, the 目标 will not actually take 伤害, and their 生命值 will appear to restore. Because the attacking 玩家 applies the 减益, the 目标 will receive the 减益 regardless
				target.AddBuff(ModContent.BuffType<ExampleDefenseDebuff>(), 600, quiet: false);
			}
			else if (mode == 7) {
				var damageModificationPlayer = player.GetModPlayer<ExampleDamageModificationPlayer>();
				if (damageModificationPlayer.exampleDodgeCooldown == 0) {
					player.AddBuff(ModContent.BuffType<ExampleDodgeBuff>(), 1800);
				}
			}
		}
	}
}
