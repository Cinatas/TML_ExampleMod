using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

/// <summary>
/// This Example illustrates a solution for storing Small-Sparse-Simple 数据 at locations. The definitions of those are as follows:
/// Small/Large - < 10 locations are actively using the 数据 per 帧 is small, > 10 is large. Use UNRELEASEDSYSTEM1 to do large-X-simple 数据.
/// Sparse/Filled - Sparse is that not all locations will have 数据, typically less than 60% 在 世界 will have 数据. Use UNRELEASEDSYSTEM1 to do large-X-simple 数据.
/// Simple/Complex - Sorta arbitrary. Simple 数据 will not contain methods, nor complicated functionality, and typically is just basic 数据 types. Use TileEntities if working with complex 数据.
/// </summary>

///			Some other common use cases not exampled:
/// Getting 数据 for a particular 图格 类型 your mod added, that was placed in 世界:
///		触发器 fetch of 数据 using adjTiles[类型]. If 数据 is ordered, use appropriate 版本 of PosData.Lookup. If 数据 is not ordered, you will likely need to 查找 via enumeration.
///		If it is unordered additions, you may elect to build myMap yourself OR attempt to insert the 数据 so it remains ordered. The latter will lead to better post-事件 性能.
///	Clustering 数据 to achieve sparsity:
///		If your application has multiple repeat static 数据 in a 行, you should elect to use Clustered 模式 在 builder to compress it. Note that you should NOT use PosData.LookupExact in this case.


// 未来待办事项：改进文档。
namespace ExampleMod.Common.Systems
{
	// 保存和加载需要 TagCompounds，wiki 上有指南：https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound
	public class SimpleDataAtParticularLocations : ModSystem
	{
		// 创建我们的映射。对你想要存储的任何类型的数据使用泛型。
		public PosData<byte>[] myMap;

		// 接下来，我们确保在世界加载时将映射初始化为空映射。
		public override void ClearWorld() {
			myMap = new PosData<byte>[0];
		}

		// 我们使用 TagCompounds 保存数据集。
		// NOTE: The tag 实例 provided here is always empty 默认情况下.
		public override void SaveWorldData(TagCompound tag) {
			if (myMap.Length != 0) {
				tag["myMap"] = myMap.Select(info => new TagCompound {
					["pos"] = info.pos,
					["data"] = info.value
				}).ToList();
			}
		}

		// We 加载 our 数据 sets using the provided TagCompound. Should mirror SaveWorldData()
		public override void LoadWorldData(TagCompound tag) {

			List<PosData<byte>> list = new List<PosData<byte>>();
			foreach (var entry in tag.GetList<TagCompound>("myMap")) {
				list.Add(new PosData<byte>(
					entry.GetInt("pos"),
					entry.Get<byte>("data")
				));
			}
			myMap = list.ToArray();
		}

		// We define what we want to generate as additional 位置 数据, for this example, in PostWorldGen.
		// We will create a simple 列 of byte 数据 going down the horizontal 中心 的 世界 that we will later use in PreUpdateWorld.
		public override void PostWorldGen() {
			var builder = new PosData<byte>.OrderedSparseLookupBuilder(compressEqualValues: false);

			int xCenter = Main.maxTilesX / 2;

			for (int y = 0; y < Main.maxTilesY; y++) {
				builder.Add(
					xCenter, y, // The locations
					(byte)(y % 255) // The 数据 we want to store 在 位置
				);
			}

			myMap = builder.Build();
		}

		// We call our custom 方法 after testing that our 地图 isn't empty - this ensures safe-loading on previous generated worlds!
		public override void PreUpdateWorld() {
			if (myMap.Length == 0) {
				return;
			}
			foreach (var player in Main.ActivePlayers) {
				UpdateFromNearestInMap(player);
			}
		}

		// We use the 列 at 世界 中心 to paint nearby tiles based 在 玩家's proximity 到 nearest entry 在 地图.
		// In this case, the nearest entry should correspond 到 玩家's depth.
		public void UpdateFromNearestInMap(Player player) {
			// 获取 玩家 位置 in 图格 coordinates
			Point z = player.position.ToTileCoordinates();
			// 搜索 for an entry within 32 tiles of our 玩家
			if (PosData.NearbySearchOrderedPosMap(myMap, z, 32, out var entry)) {
				// If found, we grab the 数据 从 corresponding 输出 索引
				var data = entry.value;

				// We then proceed to paint a 5x2 区域 around the 玩家 位置 with our locational custom values.
				for (int i = -2; i < 3; i++) {
					for (int j = 0; j < 2; j++) {
						Tile tile = Main.tile[z.X + i, z.Y + j];
						if (tile.HasTile) {
							tile.TileColor = data;
						}
					}
				}
			}
		}
	}
}