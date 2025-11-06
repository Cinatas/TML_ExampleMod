using ExampleMod.Common.Players;
using ExampleMod.Content.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	/// <summary>
	/// AbsorbTeamDamageAccessory mimics the unique 效果 的 Paladin's Shield 项.
	/// This example showcases some advanced interplay between accessories, buffs, and ModPlayer hooks.
	/// Of particular note is how this 饰品 gives other players a 增益 and how a 玩家 might act on another 玩家 being hit.
	/// </summary>
	[AutoloadEquip(EquipType.Shield)]
	public class AbsorbTeamDamageAccessory : ModItem
	{
		public static readonly int DamageAbsorptionAbilityLifeThresholdPercent = 50;
		public static float DamageAbsorptionAbilityLifeThreshold => DamageAbsorptionAbilityLifeThresholdPercent / 100f;

		public static readonly int DamageAbsorptionPercent = 30;
		public static float DamageAbsorptionMultiplier => DamageAbsorptionPercent / 100f;

		// 50 tiles is 800 世界 units. (50 * 16 == 800)
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
			// 玩家.noKnockback = 真; 可能 used here if this 饰品 prevented knockback.

			player.GetModPlayer<ExampleDamageModificationPlayer>().hasAbsorbTeamDamageEffect = true;

			// 记住 that UpdateAccessory runs for all players on all clients. Only check 每个 10 ticks
			if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0) {
				Player localPlayer = Main.player[Main.myPlayer];
				if (localPlayer.team == player.team && player.team != 0 && player.statLife > player.statLifeMax2 * DamageAbsorptionAbilityLifeThreshold && player.Distance(localPlayer.Center) <= DamageAbsorptionRange) {
					// 增益 is 用于 visually indicate 到 玩家 th在y are defended, and is also synchronized automatically to other players, letting them know that we were defended 在 时间 we took the hit
					localPlayer.AddBuff(ModContent.BuffType<AbsorbTeamDamageBuff>(), 20);
				}
			}
		}
	}
}
