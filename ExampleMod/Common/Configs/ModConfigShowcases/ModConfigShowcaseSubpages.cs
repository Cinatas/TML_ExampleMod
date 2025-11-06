using ExampleMod.Common.Configs.CustomDataTypes;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

// 此文件包含展示定义子页面的假 ModConfig 类
// 可用于将配置部分分离为子配置以便于管理。

// 因为此配置旨在展示各种 用户界面 功能，
// 此配置对模组没有影响，纯粹提供教学示例。
namespace ExampleMod.Common.Configs.ModConfigShowcases
{
	[BackgroundColor(148, 72, 188)]
	public class ModConfigShowcaseSubpages : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Header("SeparatePageExamples")]
		// 使用 SeparatePage，对象将作为按钮呈现给用户。该按钮将引导到一个单独的页面，其中将呈现常规 用户界面。对组织很有用。
		[SeparatePage]
		public Gradient gradient = new Gradient();

		// 此示例具有多级子页面，请查看。在此示例中，SubConfigExample 类本身用 [SeparatePage] 注释
		public SubConfigExample subConfigExample = new SubConfigExample();

		[SeparatePage]
		public Dictionary<ItemDefinition, SubConfigExample> DictionaryofSubConfigExample = new Dictionary<ItemDefinition, SubConfigExample>();

		// 这 2 个示例展示了 [SeparatePage] 如何在注释类的字段和注释类的列表时工作
		[SeparatePage]
		public List<Pair> SeparateListOfPairs = new List<Pair>();

		[SeparatePage]
		public Pair pair = new Pair();

		// C# 允许内部类（此处使用），如果需要，这对组织可能很有用。
		[SeparatePage]
		public class SubConfigExample
		{
			[DefaultValue(99)]
			public int boost = 99;
			public float percent;
			public bool enabled;

			[SeparatePage]
			[BackgroundColor(50, 200, 100)]
			public SubSubConfigExample SubA = new SubSubConfigExample();

			[SeparatePage]
			public SubSubConfigExample SubB = new SubSubConfigExample();

			public override string ToString() {
				return $"{boost} {percent} {enabled} {SubA.whoa}/{SubB.whoa}";
			}

			public override bool Equals(object obj) {
				if (obj is SubConfigExample other)
					return boost == other.boost && percent == other.percent && enabled == other.enabled && SubA.Equals(other.SubA) && SubB.Equals(other.SubB);
				return base.Equals(obj);
			}

			public override int GetHashCode() {
				return new { boost, percent, enabled, SubA, SubB }.GetHashCode();
			}
		}

		public class SubSubConfigExample
		{
			public int whoa;
			public override bool Equals(object obj) {
				if (obj is SubSubConfigExample other)
					return whoa == other.whoa;
				return base.Equals(obj);
			}

			public override int GetHashCode() => whoa.GetHashCode();
		}
	}
}
