// This 文件 defines an enum 数据 类型 that 可以 used in ModConfig classes.
namespace ExampleMod.Common.Configs.CustomDataTypes
{
	public enum SampleEnum
	{
		Weird,
		Odd,
		// 枚举成员也可以单独标记
		// [LabelKey("$Mods.ExampleMod.Configs.SampleEnum.Strange.标签")]
		Strange,
		Peculiar
	}
}
