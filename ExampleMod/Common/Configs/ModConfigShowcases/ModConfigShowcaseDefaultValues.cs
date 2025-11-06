using ExampleMod.Common.Configs.CustomDataTypes;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

// This 文件 contains fake ModConfig 类 that showcase defining default values for 配置 fields.

// Because this 配置 was designed to show off 各种 用户界面 capabilities,
// this 配置 have no 效果 在 mod and provides purely teaching example.
namespace ExampleMod.Common.Configs.ModConfigShowcases
{
	[BackgroundColor(164, 153, 190)]
	public class ModConfigShowcaseDefaultValues : ModConfig
	{
		// 有 2 approaches to default values. One is applicable only to 值 types (int, bool, float, 字符串, structs, etc) and the other to 引用 types (classes).
		// For 值 types, annotate the 字段 与 DefaultValue attribute. Some structs, like 颜色 and Vector2, 接受 a 字符串 that 将 converted to a default 值.
		// For 引用 types (classes), simply assign the 值 在 字段 initializer or constructor as you would typically do.

		public override ConfigScope Mode => ConfigScope.ClientSide;

		// Using DefaultValue, 我们可以 specify a default 值.
		[DefaultValue(99)]
		public int SimpleDefaultInt;

		[DefaultValue(typeof(Color), "73, 94, 171, 255")] // needs 4 comma separated bytes. The 颜色 struct has [TypeConverter(typeof(ColorConverter))] annotating it supplying a way to convert a 文本 constant to a runtime default 值.
		public Color SomeColor;

		[DefaultValue(typeof(Vector2), "0.23, 0.77")]
		public Vector2 SomeVector2;

		[DefaultValue(SampleEnum.Strange)]
		[DrawTicks]
		public SampleEnum EnumExample2;

		// Using StringEnumConverter, Enums are read and written as strings 而不是 the numerical 值 的 Enum. This makes the 配置 文件 more readable, but prone to errors if a 玩家 manually modifies the 配置 文件.
		[JsonConverter(typeof(StringEnumConverter))]
		public SampleEnum EnumExample1 { get; set; }

		// OptionStrings makes a 字符串 appear as a choice 而不是 an 输入 字段. Remember that users can manually edit json files, so be aware that a 值 other than the Options in OptionStrings might populate the 字段.
		// 待办事项： Not working. Won't restore defaults
		[OptionStrings(new string[] { "Win", "Lose", "Give Up" })]
		[DefaultValue(new string[] { "Give Up", "Give Up" })]
		public string[] ArrayOfString;

		[DrawTicks]
		[OptionStrings(new string[] { "Pikachu", "Charmander", "Bulbasaur", "Squirtle" })]
		[DefaultValue("Bulbasaur")]
		public string FavoritePokemon;

		// 默认ListValue provides the default 值 to be added when the 用户 clicks add 在 用户界面.
		[DefaultListValue(123)]
		public List<int> ListOfInts = new List<int>();

		[DefaultListValue(typeof(Vector2), "0.1, 0.2")]
		public List<Vector2> ListOfVector2 = new List<Vector2>();

		// JsonDefaultListValue provides the default 值 for 引用 types/classes, expressed as JSON. If you are unsure 的 JSON, you can 复制 from a saved 配置 文件 itself.
		[JsonDefaultListValue("{\"name\": \"GoldBar\"}")]
		public List<ItemDefinition> ListOfItemDefinition = new List<ItemDefinition>();

		// For Dictionaries, additional attributes (DefaultDictionaryKeyValue or JsonDefaultDictionaryKeyValue) are 用于 specify a default 值 对于 键 的 字典 entry. The 值 uses the DefaultListValue or JsonDefaultListValue as 列表 and HashSet do.
		[DefaultDictionaryKeyValue(0.3f)]
		[DefaultListValue(10)]
		public Dictionary<float, int> DictionaryDefaults = new Dictionary<float, int>();

		[JsonDefaultDictionaryKeyValue("{\"name\": \"GoldBar\"}")]
		[JsonDefaultListValue("{\"name\": \"SilverBar\"}")]
		public Dictionary<ItemDefinition, ItemDefinition> DictionaryDefaults2 = new Dictionary<ItemDefinition, ItemDefinition>();
	}
}
