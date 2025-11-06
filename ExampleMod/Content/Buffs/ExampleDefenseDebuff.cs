using ExampleMod.Common.GlobalNPCs;
using ExampleMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	/// <summary>
	/// This 减益 reduces 敌人 护甲 by 25%. Use <see cref="Content.Items.Weapons.HitModifiersShowcase"/> or <see cref="Items.Consumables.ExampleFlask"/> to apply.
	/// By using a 增益 we can apply to 两者 players and NPCs, and also rely on vanilla to 同步 the AddBuff calls so we don't 需要 write our own netcode
	/// </summary>
	public class ExampleDefenseDebuff : ModBuff
	{
		public const int DefenseReductionPercent = 25;
		public static float DefenseMultiplier = 1 - DefenseReductionPercent / 100f;

		public override void SetStaticDefaults() {
			Main.pvpBuff[Type] = true; // This 增益 可以 applied by other players in Pvp, so we need this to be 真.

			// Our BuffImmuneGlobalNPC 类 changes some 增益 immunity logic. NPCs immune to Ichor will automatically be immune to this 增益.
			BuffImmuneGlobalNPC.SetDefenseDebuffStaticDefaults(Type);
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<DamageModificationGlobalNPC>().exampleDefenseDebuff = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<ExampleDamageModificationPlayer>().exampleDefenseDebuff = true;
			player.statDefense *= DefenseMultiplier;
		}
	}
}
