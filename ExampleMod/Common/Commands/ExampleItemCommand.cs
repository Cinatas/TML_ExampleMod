using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ExampleMod.Common.Commands
{
	public class ExampleItemCommand : ModCommand
	{
		// CommandType.Chat 表示命令可以在单人和多人模式的聊天中使用
		public override CommandType Type
			=> CommandType.Chat;

		// 触发此命令所需的文本
		public override string Command
			=> "item";

		// 此命令的简短使用说明
		public override string Usage
			=> "/item <type|name> [stack]" +
			"\n type — ItemID of item." +
			"\n name — name of Item in current localization." +
			"\n Replace spaces in item name with underscores.";

		// 此命令的简短描述
		public override string Description
			=> "Spawn an item by name or by typeId";

		public override void Action(CommandCaller caller, string input, string[] args) {
			// 检查输入参数
			if (args.Length == 0)
				throw new UsageException("至少 one argument was expected.");

			// 如果无法解析整数，意味着我们有一个名称（或错误使用了命令）
			// 在这种情况下，type 等于 0
			if (!int.TryParse(args[0], out int type)) {
				// 将元素名称中的下划线替换为空格
				string name = args[0].Replace("_", " ");

				// 遍历所有物品以查找所需的 typeId
				// 仅当物品名称与当前本地化中所需的名称匹配时（不区分大小写） 
				for (int k = 1; k < ItemLoader.ItemCount; k++) {
					if (name.ToLower() == Lang.GetItemNameValue(k).ToLower()) {
						type = k;
						break;
					}
				}
			}

			if (type <= 0 || type >= ItemLoader.ItemCount)
				throw new UsageException(string.Format("Unknown item — 必须 valid name or 0 < type < {0}", ItemLoader.ItemCount));

			// 如果命令至少有两个参数，我们尝试获取堆叠值
			// 默认堆叠值为 1
			int stack = 1;
			if (args.Length >= 2) {
				if (!int.TryParse(args[1], out stack))
					throw new UsageException("Stack value 必须 integer, but met: " + args[1]);
			}

			// 在调用玩家所在的位置生成物品
			caller.Player.QuickSpawnItem(new EntitySource_DebugCommand($"{nameof(ExampleMod)}_{nameof(ExampleItemCommand)}"), type, stack);
		}
	}
}