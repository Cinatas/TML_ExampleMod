using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader.Config;


// 此文件包含展示创建配置部分的假 ModConfig 类
// 通过使用具有定义范围的字段。

// Because this 配置 was designed to show off 各种 用户界面 capabilities,
// this 配置 have no 效果 在 mod and provides purely teaching example.
namespace ExampleMod.Common.Configs.ModConfigShowcases
{
	[BackgroundColor(99, 180, 209)]
	public class ModConfigShowcaseRanges : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		// 在浮点数上没有注释时，默认范围为 0 到 1，刻度为 0.01。
		public float NormalFloat;

		// 我们可以使用注释指定范围、增量，甚至是否绘制引导刻度。
		[Range(2f, 3f)]
		[Increment(.25f)]
		[DrawTicks]
		[DefaultValue(2f)]
		public float IncrementalRangedFloat;

		[Range(0f, 5f)]
		[Increment(.11f)]
		public float IncrementByPoint11;

		[Range(2f, 5f)]
		[DefaultValue(2f)]
		public float RangedFloat;

		// 在整数上没有注释时，默认范围为 0 到 100。除非存在 滑块 属性，否则整数将显示为文本输入。
		public int NormalInt;

		[Increment(5)]
		[Range(60, 250)]
		[DefaultValue(100)]
		[Slider] // The 滑块 attribute makes this 字段 be presented with a 滑块 rather than a 文本 输入. The default ticks is 1.
		public int RangedInteger;

		// 我们可以注释 列表<int>，列表的所有元素将使用范围、刻度、增量和滑块属性。
		// 我们可以使用 DefaultListValue 为添加到列表的项设置默认值。在这里使用 DefaultValue 会导致游戏崩溃。
		[Range(10, 20)]
		[Increment(2)]
		[DrawTicks]
		[DefaultListValue(16)]
		[Slider]
		public List<int> ListOfInts = new List<int>();

		[Range(-20f, 20f)]
		[Increment(5f)]
		[DrawTicks]
		public Vector2 RangedWithIncrementVector2;

		// 用 OnDeserialized 注释的方法将在反序列化后运行。你可以使用它来强制执行范围等内容，因为 范围 和 Increment 是 用户界面 建议。
		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context) {
			// RangeAttribute 只是对 用户界面 的建议。如果我们想强制执行约束，我们需要在这里验证数据。用户可以使用 RangeAttribute 之外的值手动编辑配置文件，因此如有必要，我们在这里修复。
			// 强制执行范围和不强制执行范围在模组中都有用途。如果范围外的值会弄乱你的模组，请确保修复配置值。
			RangedFloat = Utils.Clamp(RangedFloat, 2f, 5f);
		}
	}
}
