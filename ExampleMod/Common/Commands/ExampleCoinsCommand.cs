using ExampleMod.Common.UI.ExampleCoinsUI;
using Terraria.ModLoader;

namespace ExampleMod.Common.Commands
{
	public class ExampleCoinsCommand : ModCommand
	{
		// CommandType.Chat 表示命令可以在单人和多人模式的聊天中使用
		public override CommandType Type
			=> CommandType.Chat;

		// 触发此命令所需的文本
		public override string Command
			=> "coins";

		// 此命令的简短描述
		public override string Description
			=> "Show the coin rate UI";

		public override void Action(CommandCaller caller, string input, string[] args) {
			ModContent.GetInstance<ExampleCoinsUISystem>().ShowMyUI();
		}
	}
}