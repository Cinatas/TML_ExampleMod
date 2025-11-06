using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Commands
{
	public class ExampleSummonCommand : ModCommand
	{
		// CommandType.World 表示命令可以在单人和多人模式的聊天中使用，但在多人模式中在服务器上执行
		public override CommandType Type
			=> CommandType.World;

		// 触发此命令所需的文本
		public override string Command
			=> "summon";

		// 此命令的简短使用说明
		public override string Usage
			=> "/summon type [[~]x] [[~]y] [number]" +
			"\n type - NPCID of NPC." +
			"\n x and y - position of spawn." +
			"\n ~ - to use position relative to player." +
			"\n number - number of NPC's to spawn.";

		// 此命令的简短描述
		public override string Description
			=> "Spawn a NPC by NPCID";

		public override void Action(CommandCaller caller, string input, string[] args) {
			// 检查输入参数
			if (args.Length == 0) {
				throw new UsageException("至少 one argument was expected.");
			}
			if (!int.TryParse(args[0], out int type)) {
				throw new UsageException(args[0] + " is not a correct integer value.");
			}

			// 生成的默认值
			// 位置 - Player.Bottom，NPC 数量 - 1 
			int xSpawnPosition = (int)caller.Player.Bottom.X;
			int ySpawnPosition = (int)caller.Player.Bottom.Y;
			int numToSpawn = 1;
			bool relativeX = false;
			bool relativeY = false;

			// 如果命令有 X 位置参数
			if (args.Length > 1) {
				// X 相对检查
				if (args[1][0] == '~') {
					relativeX = true;
					args[1] = args[1].Substring(1);
				}
				// 解析 X 位置
				if (!int.TryParse(args[1], out xSpawnPosition)) {
					throw new UsageException(args[1] + " is not a correct X position (必须 valid integer value).");
				}
			}

			// 如果命令有 Y 位置参数
			if (args.Length > 2) {
				// Y 相对检查
				if (args[2][0] == '~') {
					relativeY = true;
					args[2] = args[2].Substring(1);
				}
				// 解析 Y 位置
				if (!int.TryParse(args[2], out ySpawnPosition)) {
					throw new UsageException(args[2] + " is not a correct Y position (必须 valid integer value).");
				}
			}

			// 如果位置是相对的，则调整位置
			if (relativeX) {
				xSpawnPosition += (int)caller.Player.Bottom.X;
			}
			if (relativeY) {
				ySpawnPosition += (int)caller.Player.Bottom.Y;
			}

			// 如果命令有数量参数
			if (args.Length > 3) {
				if (!int.TryParse(args[3], out numToSpawn)) {
					throw new UsageException(args[3] + " is not a correct number (必须 valid integer value).");
				}
			}

			// 使用给定的位置和类型生成 numToSpawn 个 NPC
			for (int k = 0; k < numToSpawn; k++) {
				// 如果没有足够的 NPC 槽位来生成，NPC.NewNPC 返回 200 (Main.maxNPCs)
				int slot = NPC.NewNPC(new EntitySource_DebugCommand($"{nameof(ExampleMod)}_{nameof(ExampleSummonCommand)}"), xSpawnPosition, ySpawnPosition, type);

				// 在多人模式中在服务器上同步 NPC
				if (Main.netMode == NetmodeID.Server && slot < Main.maxNPCs) {
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, slot);
				}
			}
		}
	}
}
