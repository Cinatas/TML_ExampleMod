using ExampleMod.Content.Tiles;
using System.IO;
using Terraria;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ModLoader.Default;

namespace ExampleMod.Content.TileEntities
{
	/// <summary>
	/// This TileEntity is used in direct tandem with <seealso cref="ExamplePylonTileAdvanced"/> in 顺序 to grant more 灵活性 than
	/// vanilla's normal pylon TileEntity (AKA <seealso cref="TETeleportationPylon"/>) using the <seealso cref="TEModdedPylon"></seealso> 类
	/// 即 built into tML itself.
	/// <para>
	/// The main example shown here is having a Pylon 即 only active at completely 随机 intervals.
	/// </para>
	/// </summary>
	public class AdvancedPylonTileEntity : TEModdedPylon
	{
		// 这是 the main crux of this TileEntity; its pylon functionality will only work when this 布尔值 is 真.
		public bool isActive;

		public override void OnNetPlace() {
			// This hook is only ever called 在 服务器; its purpose is to give more freedom in terms of syncing 从 服务器 to clients, which we take advantage of
			// by making sure to 同步 whenever this hook is called:
			NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
		}

		public override void NetSend(BinaryWriter writer) {
			// 我们 want to make sure that our 数据 is synced properly across clients and 服务器.
			// NetSend is called whenever a TileEntitySharing 消息 is sent, so the game will 处理 this automatically for us,
			// granted that we send a 消息 when we need to.
			writer.Write(isActive);
		}

		public override void NetReceive(BinaryReader reader) {
			isActive = reader.ReadBoolean();
		}

		public override void Update() {
			// 更新 is only ever called 在 服务器 or in SinglePlayer, so our randomness 将 in that 帧 of 引用
			// Every tick, there 将 a 1/180 概率 th在 active 状态 of this pylon will swap (ON to OFF or vice versa)
			if (!Main.rand.NextBool(180)) {
				return;
			}

			// Granted th在 check passes, we change the active 状态, and if this is 在 服务器, we 同步 it 与 服务器:
			isActive = !isActive;
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
			}
		}
	}
}
