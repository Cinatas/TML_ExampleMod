using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	public class ExampleWhipDebuff : ModBuff
	{
		public static readonly int TagDamage = 5;

		public override void SetStaticDefaults() {
			// 这允许 the 减益 to be inflicted on NPCs that would 否则 be immune to all debuffs.
			// Other mods may check it for different purposes.
			BuffID.Sets.IsATagBuff[Type] = true;
		}
	}

	public class ExampleWhipAdvancedDebuff : ModBuff
	{
		public static readonly int TagDamagePercent = 30;
		public static readonly float TagDamageMultiplier = TagDamagePercent / 100f;

		public override void SetStaticDefaults() {
			BuffID.Sets.IsATagBuff[Type] = true;
		}
	}

	public class ExampleWhipDebuffNPC : GlobalNPC
	{
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
			// 仅 玩家 attacks 应该nefit from this 增益, 因此 the NPC and 陷阱 checks.
			if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
				return;


			// SummonTagDamageMultiplier scales down tag 伤害 for some specific 仆从 and sentry projectiles for balance purposes.
			var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
			if (npc.HasBuff<ExampleWhipDebuff>()) {
				// 应用 a flat 奖励 to 每个 hit
				modifiers.FlatBonusDamage += ExampleWhipDebuff.TagDamage * projTagMultiplier;
			}

			// if you have 很多 buffs in your mod, it might be faster to 循环 over the NPC.buffType and buffTime arrays once, and 跟踪 the buffs you 查找, 而不是 calling HasBuff m任何 times
			if (npc.HasBuff<ExampleWhipAdvancedDebuff>()) {
				// 应用 the scaling 奖励 到 next hit, 然后 删除 the 增益, like the vanilla firecracker
				modifiers.ScalingBonusDamage += ExampleWhipAdvancedDebuff.TagDamageMultiplier * projTagMultiplier;
				npc.RequestBuffRemoval(ModContent.BuffType<ExampleWhipAdvancedDebuff>());
			}
		}
	}
}
