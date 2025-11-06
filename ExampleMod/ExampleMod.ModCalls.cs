// 文件路径: c:\Users\Administrator\Documents\My Games\Terraria\tModLoader\ModSources\ExampleMod\ExampleMod.ModCalls.cs
using ExampleMod.Common.Players;
using ExampleMod.Common.Systems;
using System;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod
{
	// 这是一个部分类，意味着它的一些部分被拆分到其他文件中。请参阅 ExampleMod.*.cs 以了解其他部分。
	// 该类是部分类，用于将相似的代码组织在一起，以便更清楚地了解相关内容。
	// 这个类继承自 ExampleMod.cs 中的 Mod 类。如果使用此文件作为模组 Mod 类的模板，请确保继承自 Mod 类（": Mod"）。
	partial class ExampleMod
	{
		// 以下代码允许其他模组“调用”Example Mod 的数据。
		// 这使得模组开发者可以访问 Example Mod 的数据，而无需将其设置为引用。
		// 模组调用默认情况下不可用，因此您需要发布适当的调用以及它们返回的值。
		public override object Call(params object[] args) {
			// 确保调用中不包含任何可能导致异常的内容。
			if (args is null) {
				throw new ArgumentNullException(nameof(args), "参数不能为空！");
			}

			if (args.Length == 0) {
				throw new ArgumentException("参数不能为空！");
			}

			// 此检查确保参数是一个字符串，使用模式匹配。
			// 由于我们只需要一个参数，我们将只取数组中的第一个项目。
			if (args[0] is string content) {
				// 并将其视为命令类型。
				switch (content) {
					case "downedMinionBoss":
						// 如果参数调用了它，则返回 downedMinionBoss 提供的值。
						return DownedBossSystem.downedMinionBoss;
					case "showMinionCount":
						// 如果参数调用了它，则返回 showMinionCount 提供的值。
						return Main.LocalPlayer.GetModPlayer<ExampleInfoDisplayPlayer>().showMinionCount;
					case "setMinionCount":
						// 我们需要确保调用提供了一个值来设置字段。
						if (args[1] is not bool minionSet) {
							// 如果不是我们需要的类型，我们无法继续。
							// 告诉开发者我们需要的类型，以及我们实际得到的类型。
							throw new Exception($"在设置 minion count 时需要一个 bool 类型的参数，但得到的是 {args[1].GetType().Name} 类型。");
						}

						// 我们将值设置为参数提供的值。
						// 可选地，您可以返回一个值，指示赋值成功。
						// 返回 true;
						Main.LocalPlayer.GetModPlayer<ExampleInfoDisplayPlayer>().showMinionCount = minionSet;

						// 返回一个 'true' 布尔值，作为操作成功的多种方式之一。
						return true;
				}
			}

			// 我们也可以对不同的数据类型执行此操作。
			if (args[0] is int contentInt && contentInt == 4) {
				return ModContent.GetInstance<ExampleBiomeTileCount>().exampleBlockCount;
			}

			// 如果提供的参数不匹配我们想要返回值的任何内容，我们将返回一个 'false' 布尔值。
			// 此值可以是您希望提供的任何默认值。
			return false;
		}
	}
}