using ExampleMod.Content.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	public class ExampleMusicBox : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.CanGetPrefixes[Type] = false; // 音乐 boxes can't get prefixes in vanilla
			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox; // recorded 音乐 boxes transform in到 basic form in shimmer

			// following code links the 音乐 box's 项 and 图格 with a 音乐 跟踪:
			//   When 音乐 与 given ID is playing, equipped 音乐 boxes have a 概率 to change their ID 到 given 项 类型.
			//   When an 项 与 given 项 类型 is equipped, it will play the 音乐 that has musicSlot as its ID.
			//   When a 图格 与 given 类型 and Y-帧 is nearby, if its X-帧 is >= 36, it will play the 音乐 that has musicSlot as its ID.
			// 当 getting the 音乐 槽位, 你应该 not add the 文件 extensions!
			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery"), ModContent.ItemType<ExampleMusicBox>(), ModContent.TileType<ExampleMusicBoxTile>());
		}

		public override void SetDefaults() {
			Item.DefaultToMusicBox(ModContent.TileType<ExampleMusicBoxTile>(), 0);
		}
	}
}
