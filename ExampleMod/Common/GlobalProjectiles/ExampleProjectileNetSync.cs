using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ExampleMod.Common.GlobalProjectiles
{
	// 这是一个专门展示 Send/ReceiveExtraAI() 的类
	public class ExampleProjectileNetSync : GlobalProjectile
	{
		public override bool InstancePerEntity => true;
		private bool differentBehavior;
		private float distance;

		// 这减少了实际拥有此 GlobalProjectile 的弹幕数量
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
			return entity.type == ProjectileID.SharknadoBolt;
		}

		// 虽然这在客户端和服务器上都运行，但只有生成弹幕的会话知道其来源
		// 因此，下面演示的检查在客户端始终为 假，代码永远不会运行！
		public override void OnSpawn(Projectile projectile, IEntitySource source) {

			// 在血月期间由猪鲨公爵生成时
			if (source is EntitySource_Parent parent
				&& parent.Entity is NPC npc
				&& npc.type == NPCID.DukeFishron
				&& Main.bloodMoon) {

				differentBehavior = true;
				distance = projectile.Distance(Main.player[npc.target].Center);
			}
		}

		// 因为此 GlobalProjectile 仅适用于台风，所以此数据不会附加到所有弹幕同步数据包
		public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) {
			bitWriter.WriteBit(differentBehavior);

			// 此检查进一步避免在不必要时发送距离
			if (differentBehavior) {
				binaryWriter.Write(distance);
			}
		}

		// 确保你始终读取与发送的数据完全相同的数据量！
		public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) {
			differentBehavior = bitReader.ReadBit();

			if (differentBehavior) {
				distance = binaryReader.ReadSingle();
			}
		}

		public override void AI(Projectile projectile) {
			if (differentBehavior) {
				int p = Player.FindClosest(projectile.position, projectile.width, projectile.height);
				float currentDistance = p == -1 ? 0 : projectile.Distance(Main.player[p].Center);
				int dustType = DustID.GemSapphire;

				// 在非常近的范围内时结束行为
				if (currentDistance < distance / 4) {
					differentBehavior = false;
					projectile.netUpdate = true;
				}
				// 以正常速度移动，但可以加速
				else if (currentDistance < distance / 2) {
					projectile.extraUpdates = 0;
				}
				// 超出范围时变得更快
				else {
					projectile.extraUpdates = 1;
					dustType = DustID.GemRuby;
				}

				// 视觉上表明此台风具有特殊行为以及它处于哪种模式
				int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, Scale: 5f);
				Main.dust[d].noGravity = true;
			}
		}
	}
}
