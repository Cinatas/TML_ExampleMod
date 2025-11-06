using Microsoft.Xna.Framework;
using System.ComponentModel;

// 此文件定义表示可在 ModConfig 类中使用的渐变数据类型的自定义数据类型。
namespace ExampleMod.Common.Configs.CustomDataTypes
{
	public class Gradient
	{
		[DefaultValue(typeof(Color), "0, 0, 255, 255")]
		public Color start = Color.Blue; // For sub-objects, you'll want to make sure to set defaults in constructor or field initializer.
		[DefaultValue(typeof(Color), "255, 0, 0, 255")]
		public Color end = Color.Red;

		public override bool Equals(object obj) {
			if (obj is Gradient other)
				return start == other.start && end == other.end;
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return new { start, end }.GetHashCode();
		}
	}
}
