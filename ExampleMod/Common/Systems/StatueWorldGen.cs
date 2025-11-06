using ExampleMod.Content.Tiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace ExampleMod.Common.Systems
{
	// 这是向世界生成添加自定义雕像的简单示例。
	// 在此示例中，我们将雕像图格添加到现有数据结构中，每当放置随机雕像时都会查询该数据结构。
	public class StatueWorldGen : ModSystem
	{
		public override void Load() {
			// 使用 MonoMod detour，我们可以影响原本没有 tModLoader 钩子的 Terraria 方法。
			On_WorldGen.SetupStatueList += On_WorldGen_SetupStatueList;
		}

		private void On_WorldGen_SetupStatueList(On_WorldGen.orig_SetupStatueList orig) {
			// 调用原始 SetupStatueList 方法，这会用数据初始化 GenVars.statueList
			orig();

			// 原版游戏有一个雕像类型数组，我们将把我们的添加到其中。
			int startIndex = GenVars.statueList.Length; // 保存 the original 长度 的 vanilla 列表 to use later.

			// This is an 数组 of statues we want to add to worldgen.
			// 设置 shouldBeWired to 真 to make the statue 生成 with a pressure plate wired to it (like traps are).
			(int type, bool shouldBeWired, ushort placeStyle)[] statueTypesToAdd = {
				(ModContent.TileType<ExampleStatue>(), false, 0),
				// If the mod adds more statues, they 可以 added here.
			};

			// 使 space 在 statueList 数组.
			Array.Resize(ref GenVars.statueList, GenVars.statueList.Length + statueTypesToAdd.Length);

			// 然后 add Point16s of (TileID, PlaceStyle) to it.
			for (int i = 0; i < statueTypesToAdd.Length; i++) {
				int arrayIndex = startIndex + i;
				(int statueType, bool shouldBeWired, ushort placeStyle) = statueTypesToAdd[i];

				GenVars.statueList[arrayIndex] = new Point16(statueType, placeStyle);

				if (shouldBeWired) {
					GenVars.StatuesWithTraps.Add(arrayIndex);
				}
			}
		}
	}
}