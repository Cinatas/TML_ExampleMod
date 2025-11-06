// This file defines an enum data type that 可以 used in ModConfig classes.
namespace ExampleMod.Common.Configs.CustomDataTypes
{
	public enum SampleEnum
	{
		Weird,
		Odd,
		// 枚举成员也可以单独标记
		// [LabelKey("$Mods.ExampleMod.Configs.SampleEnum.Strange.Label")]
		Strange,
		Peculiar
	}
}
