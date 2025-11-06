using ExampleMod.Common.Configs.CustomDataTypes;
using ExampleMod.Common.Configs.CustomUI;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria.ModLoader.Config;

// This 文件 contains fake ModConfig 类 that showcase various attributes
// that 可以 used to customize behavior 配置 fields.

// Because this 配置 was designed to show off various 用户界面 capabilities,
// this 配置 have no 效果 在 mod and provides purely teaching example.
namespace ExampleMod.Common.Configs.ModConfigShowcases
{
	/// <summary>
	/// This 配置 is just a showcase of various attributes and their effects 在 用户界面 窗口.
	/// </summary>
	public class ModConfigShowcaseMisc : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[CustomModConfigItem(typeof(GradientElement))]
		public Gradient gradient = new Gradient();

		/*
		// Here are some more examples, showing a complex JsonDefaultListValue and a initializer overriding the defaults 的 constructor.
		[CustomModConfigItem(typeof(GradientElement))]
		public Gradient gradient2 = new Gradient() {
			start = Color.AliceBlue,
			end = Color.DeepSkyBlue
		};

		[JsonDefaultListValue("{\"start\": \"238, 248, 255, 255\", \"end\": \"0, 191, 255, 255\"}")]
		public List<Gradient> gradients = new List<Gradient>();
		*/

		// In this case, CustomModConfigItem is annotating the Enum instead 的 字段. Either is acceptable and 可以 used for different situations.
		public Corner corner;

		// You can put multiple attributes 在 same [] if you like.
		// ColorHueSliderAttribute displays Hue Saturation Lightness. Passing in 假 means only Hue is shown.
		[DefaultValue(typeof(Color), "255, 0, 0, 255"), ColorHSLSlider(false), ColorNoAlpha]
		public Color hsl;

		// In this example we inherit from a tmodloader 配置 UIElement to slightly customize the colors.
		[CustomModConfigItem(typeof(CustomFloatElement))]
		public float tint;

		public Dictionary<string, Pair> StringPairDictionary = new Dictionary<string, Pair>();
		public Dictionary<ItemDefinition, float> JsonItemFloatDictionary = new Dictionary<ItemDefinition, float>();

		public HashSet<ItemDefinition> itemSet = new HashSet<ItemDefinition>();

		public List<Pair> ListOfPair2 = new List<Pair>();
		public Pair pairExample2 = new Pair();

		// In this example, the 列表 defaults to collapse.
		[Expand(false)]
		public List<string> collapsedList = new List<string>() { "1", "2", "3", "4", "5" };

		// This example collapses the 列表 elements 以及 as the 列表 itself.
		[Expand(false, false)]
		public List<Pair> collapsedListOfCollapsedObjects = new List<Pair>() { new Pair() { enabled = true, boost = 3 }, new Pair { enabled = true, boost = 6 } };

		[Expand(false)]
		public SimpleData simpleDataExample; // you can also initialize 在 constructor, see initialization in public ModConfigShowcaseMisc() below.

		// This annotation allows the 用户界面 to 空 out this 类. You need to make sure to initialize fields without the NullAllowed annotation in constructor or initializer or you might have issues. Of course, if you 允许 nulls, you'll need to make sure the rest of your mod will 处理 them correctly. Try to avoid 空 unless you have a good reason to use them, as 空 objects will only complicate the rest of your code.
		[NullAllowed]
		[JsonDefaultValue("{\"boost\": 777}")] // With NullAllowed, you can specify a default 值 like this.
		public SimpleData simpleDataExample2;

		public ComplexData complexData = new ComplexData();

		[JsonExtensionData]
		private IDictionary<string, JToken> _additionalData = new Dictionary<string, JToken>();

		// See _additionalData usage in OnDeserializedMethod to see how this ListOfInts 可以 populated from old versions of this mod.
		public List<int> ListOfInts = new List<int>();

		public ModConfigShowcaseMisc() {
			simpleDataExample = new SimpleData();
			simpleDataExample.boost = 32;
			simpleDataExample.percent = 0.7f;
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context) {
			// If you change ModConfig fields between versions, your users might notice their configuration is lost when they 更新 their mod.
			// We can use [JsonExtensionData] to capture un-de-serialized 数据 and manually restore them to new fields.
			// Imagine in a previous 版本 of this mod, we had a 字段 "OldListOfInts" and we want to preserve that 数据 in "ListOfInts".
			// To 测试 this, insert the following into ExampleMod_ModConfigShowcase.json: "OldListOfInts": [ 99, 999],
			if (_additionalData.TryGetValue("OldListOfInts", out var token)) {
				var OldListOfInts = token.ToObject<List<int>>();
				ListOfInts.AddRange(OldListOfInts);
			}
			_additionalData.Clear(); // 使 sure to 清除 this or it'll crash.
		}
	}
}
