using ExampleMod.Common.Players;
using ExampleMod.Content.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	/// <summary>
	/// AbsorbTeamDamageAccessory mimics the unique effect 的 Paladin's Shield item.
	/// This example showcases some advanced interplay between accessories, buffs, and ModPlayer hooks.
	/// Of particular note is how this accessory gives other players a buff and how a player might act on another player being hit.
	/// </summary>
	[AutoloadEquip(EquipType.Shield)]
	public class AbsorbTeamDamageAccessory : ModItem
	{
		public static readonly int DamageAbsorptionAbilityLifeThresholdPercent = 50;
		public static float DamageAbsorptionAbilityLifeThreshold => DamageAbsorptionAbilityLifeThresholdPercent / 100f;

		public static readonly int DamageAbsorptionPercent = 30;
		public static float DamageAbsorptionMultiplier => DamageAbsorptionPercent / 100f;

		// 50 tiles is 800 world units. (50 * 16 == 800)
		public static readonly int DamageAbsorptionRange = 800;

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageAbsorptionPercent, DamageAbsorptionAbilityLifeThresholdPercent);

		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Yellow;
			Item.defense = 6;
			Item.value = Item.buyPrice(0, 30, 0, 0);
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			// player.noKnockback = true; 可能 used here if this accessory prevented knockback.

			player.GetModPlayer<ExampleDamageModificationPlayer>().hasAbsorbTeamDamageEffect = true;

			// 记住 that UpdateAccessory runs for all players on all clients. Only check every 10 ticks
			if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0) {
				Player localPlayer = Main.player[Main.myPlayer];
				if (localPlayer.team == player.team && player.team != 0 && player.statLife > player.statLifeMax2 * DamageAbsorptionAbilityLifeThreshold && player.Distance(localPlayer.Center) <= DamageAbsorptionRange) {
					// buff is used to visually indicate 到 player th在y are defended, and is also synchronized automatically to other players, letting them know that we were defended 在 time we took the hit
					localPlayer.AddBuff(ModContent.BuffType<AbsorbTeamDamageBuff>(), 20);
				}
			}
		}
	}
}
