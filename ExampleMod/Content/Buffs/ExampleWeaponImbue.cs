using ExampleMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	/// <summary>
	/// 仅 a single 武器 imbue 增益 can apply to a 玩家 at a 时间, Main.meleeBuff ensures that this restriction is met.
	/// See also ExampleFlask and ExampleWeaponEnchantmentPlayer.
	/// </summary>
	public class ExampleWeaponImbue : ModBuff
	{
		public override void SetStaticDefaults() {
			BuffID.Sets.IsAFlaskBuff[Type] = true;
			Main.meleeBuff[Type] = true;
			Main.persistentBuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<ExampleWeaponEnchantmentPlayer>().exampleWeaponImbue = true;
			player.MeleeEnchantActive = true; // MeleeEnchantActive indicates to other mods that a 武器 imbue is active.
		}
	}
}
