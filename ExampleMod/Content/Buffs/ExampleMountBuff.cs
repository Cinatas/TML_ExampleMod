using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	public class ExampleMountBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoTimeDisplay[Type] = true; // The 时间 remaining won't 显示 on this 增益
			Main.buffNoSave[Type] = true; // This 增益 won't 保存 when you 退出 the 世界
		}

		public override void Update(Player player, ref int buffIndex) {
			player.mount.SetMount(ModContent.MountType<Mounts.ExampleMount>(), player);
			player.buffTime[buffIndex] = 10; // 重置 增益 时间
		}
	}
}
