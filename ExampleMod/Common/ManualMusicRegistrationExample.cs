using Terraria.ModLoader;

namespace ExampleMod.Common
{
	// 一个展示手动音乐加载的 ILoadable 示例。
	// 很少需要手动加载，因为默认情况下，TML 会自动加载Music文件夹（包括子目录）中的每个 .wav、.ogg 和 .mp3 声音文件作为音乐曲目。
	public sealed class ManualMusicRegistrationExample : ILoadable
	{
		public void Load(Mod mod) {
			// 手动注册音乐时，你必须提供模组实例。
			// 由于你提供的是模组实例，因此不应该在路径开头使用模组名称。
			// 接受的音乐格式为：.mp3、.ogg 和 .wav 文件。
			// 添加音乐时，请勿在代码中添加文件扩展名！

			// MusicLoader.AddMusic(Mod, "Assets/音乐/MysteriousMystery");

			// 可以在Content/Items/Placeable/ExampleMusicBox.cs中找到音乐盒注册的示例。
		}

		public void Unload() { }
	}
}
