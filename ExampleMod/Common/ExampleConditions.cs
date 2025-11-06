using ExampleMod.Common.Systems;
using ExampleMod.Content.Biomes;
using Terraria;

namespace ExampleMod.Common
{
	// 此类包含将在 ExampleMod 中多个地方重用的条件。
	// 在使用的地方创建新的 条件 并没有什么问题，如 ExampleNPCShop 和 ExamplePerson 中所示，
	// 但是将多次使用的 条件 放在一个中心位置以避免拼写错误和其他错误是一个好主意。
	// 将 条件 存储在字段中还可以公开这些条件以便更容易地实现跨模组兼容性。
	// 有关使用 条件 类的更多信息，请参阅 https://github.com/tModLoader/tModLoader/wiki/Conditions
	public static class ExampleConditions
	{
		public static Condition InExampleBiome = new Condition("Mods.ExampleMod.Conditions.InExampleBiome", () => Main.LocalPlayer.InModBiome<ExampleSurfaceBiome>() || Main.LocalPlayer.InModBiome<ExampleUndergroundBiome>());
		public static Condition DownedMinionBoss = new("Mods.ExampleMod.Conditions.DownedMinionBoss", () => DownedBossSystem.downedMinionBoss);
	}
}
