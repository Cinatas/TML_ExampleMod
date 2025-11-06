using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	// 此类 serves as an example of a 减益 that causes constant loss of life
	// 参见 ExampleLifeRegenDebuffPlayer.UpdateBadLifeRegen 在 结束 的 文件 f或更多 information
	public class ExampleLifeRegenDebuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = true;  // Is it a 减益?
			Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
			Main.buffNoSave[Type] = true; // Causes this 增益 not to persist when exiting and rejoining the 世界
			BuffID.Sets.LongerExpertDebuff[Type] = true; // If this 增益 is a 减益, 设置 this to 真 will make this 增益 last twice as long on players in expert 模式
		}

		// 允许s you to make this 增益 give certain effects 到 given 玩家
		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<ExampleLifeRegenDebuffPlayer>().lifeRegenDebuff = true;
		}
	}

	public class ExampleLifeRegenDebuffPlayer : ModPlayer
	{
		// 标志 checking when life regen 减益 应该 activated
		public bool lifeRegenDebuff;

		public override void ResetEffects() {
			lifeRegenDebuff = false;
		}

		// 允许s you to give the 玩家 a negative life regeneration based on its 状态 (例如, the "On Fire!" 减益 makes the 玩家 take 伤害-over-时间)
		// 这是 typically done by 设置 玩家.lifeRegen to 0 if it is positive, 设置 玩家.lifeRegenTime to 0, and subtracting a 数字 from 玩家.lifeRegen
		// 玩家 will take 伤害 at a rate of half the 数字 you subtract per second
		public override void UpdateBadLifeRegen() {
			if (lifeRegenDebuff) {
				// These lines zero out any positive lifeRegen. This is expected for all bad life regeneration effects
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
				// 玩家.lifeRegenTime used to increase the 速度 at which the 玩家 reaches its 最大 natural life regeneration
				// So we set it to 0, and while this 减益 is active, it never reaches it
				Player.lifeRegenTime = 0;
				// lifeRegen is measured in 1/2 life per second. Therefore, this 效果 causes 8 life lost per second
				Player.lifeRegen -= 16;
			}
		}
	}
}
