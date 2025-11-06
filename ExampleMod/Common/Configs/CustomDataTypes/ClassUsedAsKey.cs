using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

// 此文件定义可用作字典中的键的自定义数据类型。
namespace ExampleMod.Common.Configs.CustomDataTypes
{
	[TypeConverter(typeof(ToFromStringConverter<ClassUsedAsKey>))]
	public class ClassUsedAsKey
	{
		// 当你将数据从字典保存到文件 (json) 时，你需要将键表示为字符串
		// 但要取回对象，你需要一个 TypeConverter，此示例展示如何实现一个

		// 你从类上方的 [TypeConverter(typeof(ToFromStringConverter<NameOfClassHere>))] 属性开始
		// 为此，你需要其他示例中解释的常规 Equals 和 GetHashCode 覆盖，
		// 加上 ToString 和 FromString，用于将对象转换为字符串并返回

		public bool SomeBool { get; set; }
		public int SomeNumber { get; set; }

		public override bool Equals(object obj) {
			if (obj is ClassUsedAsKey other)
				return SomeBool == other.SomeBool && SomeNumber == other.SomeNumber;
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return new { SomeBool, SomeNumber }.GetHashCode();
		}

		// 在这里，你需要编写对象的字符串表示形式，以便易于再次重建
		// 在 json 文件中，它看起来像这样："真, 5"
		public override string ToString() {
			return $"{SomeBool}, {SomeNumber}";
		}

		// 在这里，你需要从给定字符串创建对象（基本上是还原 ToString）
		// 这必须是静态的，并且必须命名为 FromString
		public static ClassUsedAsKey FromString(string s) {
			// 以下代码取决于你的 ToString 实现，这里我们只有两个用 ',' 分隔的值
			string[] vars = s.Split(new char[] { ',' }, 2, StringSplitOptions.RemoveEmptyEntries);
			// System.Convert 类提供在数据类型之间转换的方法，这里使用字符串重载
			return new ClassUsedAsKey {
				SomeBool = Convert.ToBoolean(vars[0]),
				SomeNumber = Convert.ToInt32(vars[1])
			};
		}
	}
}
