using ExampleMod.Common.Configs.CustomDataTypes;
using Newtonsoft.Json;
using Terraria;
using Terraria.ModLoader.Config;

// This 文件 contains fake ModConfig 类 that showcase using
// access modifiers (to 控制 which fields 应该 visible and have their 值 saved to 文件)
// and properties (to implement simple "presets" system).

// Because this 配置 was designed to show off 各种 用户界面 capabilities,
// this 配置 have no 效果 在 mod and provides purely teaching example.
namespace ExampleMod.Common.Configs.ModConfigShowcases
{
	[BackgroundColor(164, 153, 190)]
	public class ModConfigShowcaseAccessibility : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		// Private and Internal fields and properties will 不 shown.
		// Note that private and internal values will 不 replaced by the 反序列化, so initializer and ctor work.
		// 你应该 avoid private and internal values in
#pragma warning disable CS0414
		private float Private = 144;
#pragma warning restore CS0414
		internal float Internal;

		// Public fields are most common. Use public for most items.
		public float Public;

		// Will not show. Avoid static. Due to how ModConfig works, static fields will not work correctly. Use a static 字段 named 实例 在 manner used in ExampleConfigServer for accessing ModConfig fields 在 rest of your mod.
		public static float Static;

		// 获取 only properties will show up, but 将 grayed out to show th在y can't be changed.
		public float Getter => Main.rand?.NextFloat(1f) ?? 0; // This is just an example, please don't do this.

		// AutoProperties work the same as fields.
		public float AutoProperty { get; set; }

		// Properties work 以及. The backing 字段 将 ignored when writing the json out.
		private float propertyBackingField;
		public float Property {
			get { return propertyBackingField; }
			set { propertyBackingField = value + 0.2f; } // + 0.2f is just to mess 与 用户.
		}

		// Using JsonIgnore on a public 字段 means the 字段 won't show up 在 json or 用户界面. Not really useful.
		[JsonIgnore]
		public float Ignore;

		// Using ShowDespiteJsonIgnore overrides JsonIgnore 对于 用户界面. Use this to 显示 info 到 用户 如果需要. The 值 won't be saved since it is derived from other fields.
		// 使用ful for things like displaying sums or calculated relationships.
		[JsonIgnore]
		[ShowDespiteJsonIgnore]
		public float IgnoreWithLabelGetter => AutoProperty + Public;

		// 引用 类型 getters kind of work 与 用户界面. You can experiment with this if you want.
		[JsonIgnore]
		public Pair pair2 => pair;
		public Pair pair;

		// 设置 only properties will crash tModLoader.
		// public float Setter { set { Public = 值; } }

		// 以下 shows how you can use properties to implement a preset system
		public bool PresetA {
			get => Data1 == 23 && Data2 == 63;
			set {
				if (value) {
					Data1 = 23;
					Data2 = 63;
				}
			}
		}

		public bool PresetB {
			get => Data1 == 93 && Data2 == 13;
			set {
				if (value) {
					Data1 = 93;
					Data2 = 13;
				}
			}
		}

		[Slider]
		public int Data1 { get; set; }
		[Slider]
		public int Data2 { get; set; }

		public ModConfigShowcaseAccessibility() {
			Internal = 0.2f;
		}

		// ShouldSerialize{FieldNameHere}. ShouldSerialize 可以 useful, but this example is simply replicating the behavior of JSONIgnore and is just an example 例如s sake. https://www.newtonsoft.com/json/帮助/html/ConditionalProperties.htm
		public bool ShouldSerializeGetter() {
			// 我们可以 have some logic in here to determine if the 值 is worth saving, but this is just a trivial example
			return false;
		}
	}
}
