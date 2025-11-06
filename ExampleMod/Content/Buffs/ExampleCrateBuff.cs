using ExampleMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	public class ExampleCrateBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex) {
			// 使用 a ModPlayer to keep track 的 buff being active
			player.GetModPlayer<ExampleFishingPlayer>().hasExampleCrateBuff = true;
		}
	}
}
