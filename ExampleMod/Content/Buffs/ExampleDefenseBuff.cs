using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	public class ExampleDefenseBuff : ModBuff
	{
		public static readonly int DefenseBonus = 10;

		public override LocalizedText Description => base.Description.WithFormatArgs(DefenseBonus);

		public override void Update(Player player, ref int buffIndex) {
			player.statDefense += DefenseBonus; // Grant a +10 防御 boost 到 玩家 while the 增益 is active.
		}
	}
}
