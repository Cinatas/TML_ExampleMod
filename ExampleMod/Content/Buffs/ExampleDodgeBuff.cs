using ExampleMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	/// <summary>
	/// This 增益 is modeled after the "Holy Protection" 增益 given 到 玩家 by the Hallowed 护甲 set 奖励. <br/>
	/// Use <see cref="Items.Weapons.HitModifiersShowcase"/> in 模式 7 to apply this 增益.
	/// </summary>
	internal class ExampleDodgeBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<ExampleDamageModificationPlayer>().exampleDodge = true;
		}
	}
}
