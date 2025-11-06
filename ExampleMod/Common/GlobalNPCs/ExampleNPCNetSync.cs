using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ExampleMod.Common.GlobalNPCs
{
	// 这是一个专门展示 Send/ReceiveExtraAI() 的类
	public class ExampleNPCNetSync : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		private bool differentBehavior;

		// 这减少了实际拥有此 GlobalNPC 的 NPC 数量
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
			return entity.type == NPCID.Sharkron2;
		}

		// 虽然这在客户端和服务器上都运行，但只有生成 NPC 的会话知道其来源
		// 因此，下面演示的检查在客户端始终为 假，代码永远不会运行！
		public override void OnSpawn(NPC npc, IEntitySource source) {

			// 在血月期间由克苏鲁龙卷风生成时
			if (source is EntitySource_Parent parent
				&& parent.Entity is Projectile projectile
				&& projectile.type == ProjectileID.Cthulunado
				&& Main.bloodMoon) {
				differentBehavior = true;
			}
		}

		// 因为此 GlobalNPC 仅适用于鲨鱼龙，所以此数据不会附加到所有 NPC 同步数据包
		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
			bitWriter.WriteBit(differentBehavior);
		}

		// 确保你始终读取与发送的数据完全相同的数据量！
		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
			differentBehavior = bitReader.ReadBit();
		}

		public override void AI(NPC npc) {
			if (differentBehavior) {
				npc.scale *= 1.0025f;
				if (npc.scale > 3f) {
					npc.scale = 3f;
				}
			}
		}
	}
}