using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Terraria.ModLoader.Config;

// 此文件定义包含各种其他数据类型的自定义数据类型，并且可以在 ModConfig 类中使用。
namespace ExampleMod.Common.Configs.CustomDataTypes
{
	public class ComplexData
	{
		public List<int> ListOfInts = new List<int>();

		public SimpleData nestedSimple = new SimpleData();

		[Range(2f, 3f)]
		[Increment(.25f)]
		[DrawTicks]
		[DefaultValue(2f)]
		public float IncrementalFloat = 2f;
		public override bool Equals(object obj) {
			if (obj is ComplexData other)
				return ListOfInts.SequenceEqual(other.ListOfInts) && IncrementalFloat == other.IncrementalFloat && nestedSimple.Equals(other.nestedSimple);
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return new { ListOfInts, nestedSimple, IncrementalFloat }.GetHashCode();
		}
	}
}
