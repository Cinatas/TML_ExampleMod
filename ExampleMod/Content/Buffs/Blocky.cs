using ExampleMod.Common.Players;
using ExampleMod.Content.Items.Placeable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	public class Blocky : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			ExampleCostumePlayer p = player.GetModPlayer<ExampleCostumePlayer>();

			// 我们 use blockyAccessoryPrevious here 代替 blockyAccessory because UpdateBuffs happens before UpdateEquips but after ResetEffects.
			if (player.townNPCs >= 1 && p.BlockyAccessoryPrevious) {
				p.BlockyPower = true;

				if (Main.myPlayer == player.whoAmI && Main.time % 1000 == 0) {
					player.QuickSpawnItem(player.GetSource_Buff(buffIndex), ModContent.ItemType<ExampleBlock>());
				}

				player.jumpSpeedBoost += 4.8f;
				player.extraFall += 45;

				// Some other effects:
				// 玩家.lifeRegen++;
				// 玩家.GetCritChance(DamageClass.Melee) += 2;
				// 玩家.GetDamage(DamageClass.Melee) += 0.051f;
				// 玩家.GetAttackSpeed(DamageClass.Melee) += 0.051f;
				// 玩家.statDefense += 3;
				// 玩家.moveSpeed += 0.05f;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}
