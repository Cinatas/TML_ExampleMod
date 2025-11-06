using ExampleMod.Common.Configs.CustomDataTypes;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader.Config;

// This 文件 contains fake ModConfig 类 that showcase making 配置 fields more readable
// with use of labels, headers and tooltips.

// Because this 配置 was designed to show off various 用户界面 capabilities,
// this 配置 have no 效果 在 mod and provides purely teaching example.
namespace ExampleMod.Common.Configs.ModConfigShowcases
{
	[BackgroundColor(154, 152, 181)]
	public class ModConfigShowcaseLabels : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		// 默认情况下, all ModConfig fields and properties will have an automatically assigned 标签 and 工具提示 翻译 键. You'll 查找 these 翻译 keys in your 翻译 files. All 的 English translations 对于 configs in ExampleMod are found in ExampleMod/Localization/en-US_Mods.ExampleMod.Configs.hjson

		// 使用 工具提示 to convey additional information about the 配置 项.
		// This example shows additional 文本 when hovered.
		[SliderColor(255, 0, 127)]
		public float SomeFloat;

		// Modders can pass in custom localization keys. This 可以 useful for reusing translations.
		[LabelKey("$Mods.ExampleMod.Configs.Common.LocalizedLabel")]
		[TooltipKey("$Mods.ExampleMod.Configs.Common.LocalizedTooltip")]
		public int LocalizedLabel;

		// These 3 examples showcase the power of interpolating values in到 translations.
		// Note how all 3 are using the same 标签 键, but are interpolating different values in到 标签 翻译, resulting in different 文本. The same is done for tooltips.
		// 使用 this approach to reduce unnecessary duplication of 文本.
		// Note: using nameof can 帮助 avoid typos and errors. That would look like: $"$Mods.ExampleMod.Items.{nameof(ExampleYoyo)}.DisplayName"
		// Note: These examples use 颜色 and 项 chat tags. See here for 帮助 on using Tags: https://terraria.wiki.gg/wiki/Chat#Tags
		const string InterpolatedLabel = "$Mods.ExampleMod.Configs.Common.InterpolatedLabel";
		const string InterpolatedTooltip = "$Mods.ExampleMod.Configs.Common.InterpolatedTooltip";

		[LabelKey(InterpolatedLabel), TooltipKey(InterpolatedTooltip)] // Attributes can also be combined into a single line
		[LabelArgs("ExampleMod/ExampleYoyo", 1, "=>", "$Mods.ExampleMod.Items.ExampleYoyo.DisplayName")]
		[TooltipArgs("$Mods.ExampleMod.Items.ExampleYoyo.DisplayName", "FF55AA", "22a2dd")]
		public bool InterpolatedTextA;

		[LabelKey(InterpolatedLabel), TooltipKey(InterpolatedTooltip)]
		[LabelArgs("ExampleMod/ExampleSword", 2, "=>", "$Items.ExampleSword.DisplayName")] // due to scope simplification, "Mods.ExampleMod." 可以 omitted. (https://github.com/tModLoader/tModLoader/wiki/Localization#scope-simplification)
		[TooltipArgs("$Mods.ExampleMod.Items.ExampleSword.DisplayName", "77bd8e", "88AADD")]
		public bool InterpolatedTextB;

		[LabelKey(InterpolatedLabel), TooltipKey(InterpolatedTooltip)]
		[LabelArgs(ItemID.Meowmere, 3, "=>", $"$ItemName.{nameof(ItemID.Meowmere)}")]
		[TooltipArgs($"$ItemName.{nameof(ItemID.Meowmere)}", "c441c6", "deeb55")]
		public bool InterpolatedTextC;

		// This example shows advanced capabilities of 字符串 formatting. Values 可以 formatted to appear as percentages, with language appropriate thousandths separators, and with specific 填充 or 精度.
		// The c# documentation has more information: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-格式-strings
		[LabelArgs(.15753f, 1234567890, 12, 1.77777f)]
		public bool StringFormatting;

		// The 颜色 的 配置 entry 可以 customized. R, G, B
		[BackgroundColor(255, 0, 255)]
		// The corresponding 工具提示 翻译 for this entry is empty, so the 工具提示 shown 将 从 Pair 类.
		// If the Pair 类 工具提示 had entries for arguments, we could use [TooltipArgs] here to customize it.
		public Pair pairExample = new Pair();

		// 列表 elements also inherit BackgroundColor
		[BackgroundColor(255, 0, 0)]
		public List<Pair> ListOfPair = new List<Pair>();

		// We can also add section headers, separating fields for organization
		// Using [标题("HeaderIdentifier")], Mods.ExampleMod.Configs.ModConfigShowcaseLabels.Headers.HeaderIdentifier will automatically appear in localization files. We have populated the English entry 与 值 "Headers Section".
		[Header("HeaderIdentifier")]
		public int TypicalHeader;

		// We can also specify a specific 翻译 键, if desired.
		// The "$" character before a 名称 means it should interpret the 值 as a 翻译 键 and use the loaded 翻译 与 same 键.
		[Header("$Mods.ExampleMod.Configs.Common.LocalizedHeader")]
		public int LocalizedHeader;

		// Chat tags 例如 colored 文本 or 项 icons can 帮助 users 查找 配置 sections quickly
		[Header("ChatTagExample")]
		public int CoolHeader;

		// The 类 declaration of SimpleData specifies [BackgroundColor(255, 7, 7)]. 字段 and 数据 structure 字段 annotations override 类 annotations.
		[BackgroundColor(85, 107, 47)]
		public SimpleData simpleDataExample2 = new SimpleData();
	}
}
