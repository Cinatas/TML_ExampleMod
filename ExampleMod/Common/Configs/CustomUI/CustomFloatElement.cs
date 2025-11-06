using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.Config.UI;

// 注意：此点以下是自定义配置 UI 元素。
// 请注意，使用自定义配置元素的模组将在接下来的几个 tModLoader 更新中中断，直到它们的设计最终确定。
// 如果你使用这些，你需要非常积极地更新你的模组，因为它们可能在任何更新中中断。

// 此文件定义基于浮点数据类型的自定义 ConfigElement
// 实现了可在 ModConfig 类中使用的自定义滑块着色方法。
namespace ExampleMod.Common.Configs.CustomUI
{

	class CustomFloatElement : FloatElement
	{
		public CustomFloatElement() {
			ColorMethod = new Utils.ColorLerpMethod((percent) => Color.Lerp(Color.BlueViolet, Color.Aquamarine, percent));
		}
	}
}
