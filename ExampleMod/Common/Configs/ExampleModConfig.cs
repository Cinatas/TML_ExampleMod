using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ExampleMod.Common.Configs
{
	public class ExampleModConfig : ModConfig
	{
		// ConfigScope.ClientSide 应该 used for 客户端 side, usually visual or 音频 tweaks.
		// ConfigScope.ServerSide 应该 used for basically 每个thing else, including disabling items or changing NPC behaviors
		public override ConfigScope Mode => ConfigScope.ServerSide;

		// The things in brackets are known as "Attributes".

		[Header("Items")] // Headers are like titles in a 配置. You only 需要 declare a 标题 在 项 it should appear over, not 每个 项 在 category. 
		// [标签("$Some.键")] // A 标签 is the 文本 displayed next 到 选项. This should usually be a short 描述 of what it does. 默认情况下 all ModConfig fields and properties have an automatic 标签 翻译 键, but modders can specify a specific 翻译 键.
		// [工具提示("$Some.键")] // A 工具提示 is a 描述 showed when you 悬停 your 鼠标 over the 选项. It 可以 used as a more in-depth explanation 的 选项. Like with 标签, a specific 键 可以 provided.
		[DefaultValue(true)] // This sets the configs default 值.
		[ReloadRequired] // Marking it with [ReloadRequired] makes tModLoader force a mod 重新加载 if the 选项 is changed. It 应该 used for things like 项 toggles, which only take 效果 during mod loading
		public bool ExampleWingsToggle; // To see the implementation of this 选项, see ExampleWings.cs

		[ReloadRequired]
		public bool WeaponWithGrowingDamageToggle;
	}
}
