using Terraria.ModLoader;

namespace ExampleMod.Common.Systems
{
	// 充当此模组注册的按键绑定的容器。
	// 有关用法，请参阅 Common/Players/ExampleKeybindPlayer。
	public class KeybindSystem : ModSystem
	{
		public static ModKeybind RandomBuffKeybind { get; private set; }
		public static ModKeybind LearningExampleKeybind { get; private set; }

		public override void Load() {
			// 注册新的按键绑定
			// 我们通过向本地化文件添加 Mods.{ModName}.Keybind.{KeybindName} 条目来本地化按键绑定。向英语用户显示的实际文本在 en-US.hjson 中
			RandomBuffKeybind = KeybindLoader.RegisterKeybind(Mod, "RandomBuff", "P");
			LearningExampleKeybind = KeybindLoader.RegisterKeybind(Mod, "LearningExample", "O");
		}

		// 有关卸载过程的详细说明，请参阅 ExampleMod.cs 的 Unload() 方法。
		public override void Unload() {
			// Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can 帮助 you figure out what's keeping it loaded.
			RandomBuffKeybind = null;
			LearningExampleKeybind = null;
		}
	}
}
