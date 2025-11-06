using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Pets.MinionBossPet
{
	// 你 can 查找 a simple 宠物 example in ExampleMod\Content\Pets\ExamplePet
	public class MinionBossPetBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) { // This 方法 gets called every 帧 your 增益 is active on your 玩家.
			bool unused = false;
			player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<MinionBossPetProjectile>());
		}
	}
}
