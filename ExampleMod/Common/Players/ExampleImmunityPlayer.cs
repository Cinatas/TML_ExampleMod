using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	public class ExampleImmunityPlayer : ModPlayer
	{
		public bool HasExampleImmunityAcc;

		// 始终在此处将饰品字段重置为其默认值。
		public override void ResetEffects() {
			HasExampleImmunityAcc = false;
		}

		// 原版在此方法之前以及 PreHurt 和 Hurt 之后应用免疫时间
		// 因此，我们应该在这里应用我们的免疫时间增量
		public override void PostHurt(Player.HurtInfo info) {
			// Here we increase the 玩家's immunity 时间 by 1 second when Example Immunity 饰品 is equipped
			if (!HasExampleImmunityAcc) {
				return;
			}

			// Different cooldownCounter values mean different 伤害 types taken and different cooldown slots
			// See ImmunityCooldownID for a 列表.
			// 不要 apply extra immunity 时间 to pvp 伤害 (like vanilla)
			if (!info.PvP) {
				Player.AddImmuneTime(info.CooldownCounter, 60);
			}
		}
	}
}
