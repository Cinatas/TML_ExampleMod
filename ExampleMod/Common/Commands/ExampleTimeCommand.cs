using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Commands
{
	public class ExampleTimeCommand : ModCommand
	{
		// CommandType.世界 表示命令可以在单人和多人模式的聊天中使用，但在多人模式中在服务器上执行
		public override CommandType Type
			=> CommandType.World;

		// 触发此命令所需的文本
		public override string Command
			=> "addTime";

		// 此命令的简短使用说明
		public override string Usage
			=> "/addTime numTicks" +
			"\n numTicks — positive or negative number in ticks (1 second = 60 ticks).";

		// 此命令的简短描述
		public override string Description
			=> "Adds numTicks to fast forward or rewind world time";

		public override void Action(CommandCaller caller, string input, string[] args) {
			// 完整昼夜循环的时间（以刻度为单位）（86600）
			const double cycleLength = Main.dayLength + Main.nightLength;
			// 检查输入参数
			if (args.Length == 0) {
				throw new UsageException("至少 one argument was expected.");
			}
			if (!int.TryParse(args[0], out int extraTime)) {
				throw new UsageException(args[0] + " is not a correct integer value.");
			}

			// 将当前时间（白天为 0-54000，夜晚为 0-32400）转换为循环时间（0-86600）
			double fullTime = Main.time;
			if (!Main.dayTime) {
				fullTime += Main.dayLength;
			}

			// 从参数添加时间
			fullTime += extraTime;
			// 当超出循环时间范围时限制时间（fullTime < 0 || fullTime > 86600）
			fullTime %= cycleLength;
			if (fullTime < 0) {
				fullTime += cycleLength;
			}

			// 如果 fullTime（0-86600）< dayLength（54000），则为白天，否则为夜晚
			Main.dayTime = fullTime < Main.dayLength;
			// 将循环时间转换为默认的昼/夜时间
			if (!Main.dayTime) {
				fullTime -= Main.dayLength;
			}
			Main.time = fullTime;

			// 在多人模式中在服务器上同步世界数据
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
		}
	}
}