using ExampleMod.Content.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	public class ExampleMusicBox : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.CanGetPrefixes[Type] = false; // music boxes can't get prefixes in vanilla
			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox; // recorded music boxes transform in到 basic form in shimmer

			// following code links the music box's item and tile with a music track:
			//   When music 与 given ID is playing, equipped music boxes have a chance to change their id 到 given item type.
			//   When an item 与 given item type is equipped, it will play the music that has musicSlot as its ID.
			//   When a tile 与 given type and Y-frame is nearby, if its X-frame is >= 36, it will play the music that has musicSlot as its ID.
			// 当 getting the music slot, you should not add the file extensions!
			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery"), ModContent.ItemType<ExampleMusicBox>(), ModContent.TileType<ExampleMusicBoxTile>());
		}

		public override void SetDefaults() {
			Item.DefaultToMusicBox(ModContent.TileType<ExampleMusicBoxTile>(), 0);
		}
	}
}
