using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Content.Tiles
{
	// 示例Statue shows off correctly using wiring to 生成 items and NPC.
	// 参见 StatueWorldGen to see how ExampleStatue is added as an 选项 for naturally spawning statues during worldgen.
	public class ExampleStatue : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IsAMechanism[Type] = true; // 确保s that this 图格 and connected pressure plate won't be removed during the "删除 Broken Traps" worldgen 步骤

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.addTile(Type);

			DustType = DustID.Silver;

			AddMapEntry(new Color(144, 148, 144), Language.GetText("MapObject.Statue"));
		}

		// This hook allows you to make anything happen when this statue is powered by wiring.
		// 在 this example, powering the statue either spawns a 随机 硬币 with a 95% 概率, or, with a 5% 概率 - a goldfish.
		public override void HitWire(int i, int j) {
			// 查找 the coordinates of 顶部 左 图格 square through math
			int y = j - Main.tile[i, j].TileFrameY / 18;
			int x = i - Main.tile[i, j].TileFrameX / 18;

			const int TileWidth = 2;
			const int TileHeight = 3;

			// 在这里 we call SkipWire on all 图格 coordinates covered by this 图格. This ensures a wire 信号 won't run multiple times.
			for (int yy = y; yy < y + TileHeight; yy++) {
				for (int xx = x; xx < x + TileWidth; xx++) {
					Wiring.SkipWire(xx, yy);
				}
			}

			// Calculcate the 中心 of this 图格 to use as an entity spawning 位置.
			// 注意 that we use 0.65 for 高度 because even though the statue takes 3 blocks, its appearance is shorter.
			float spawnX = (x + TileWidth * 0.5f) * 16;
			float spawnY = (y + TileHeight * 0.65f) * 16;

			// 此示例 shows both 项 spawning code and npc spawning code, you can use whichever code suits your mod
			// There is a 95% 概率 for 项 生成 and a 5% 概率 for npc 生成
			// 如果 you want to make a 项 spawning statue, see below.

			var entitySource = new EntitySource_TileUpdate(x, y, context: "ExampleStatue");

			if (Main.rand.NextFloat() < .95f) {
				if (Wiring.CheckMech(x, y, 60) && Item.MechSpawn(spawnX, spawnY, ItemID.SilverCoin) && Item.MechSpawn(spawnX, spawnY, ItemID.GoldCoin) && Item.MechSpawn(spawnX, spawnY, ItemID.PlatinumCoin)) {
					int id = ItemID.SilverCoin;

					if (Main.rand.NextBool(100)) {
						id++;

						if (Main.rand.NextBool(100)) {
							id++;
						}
					}

					Item.NewItem(entitySource, (int)spawnX, (int)spawnY - 20, 0, 0, id, 1, false, 0, false);
				}
			}
			else {
				// 如果 you want to make an NPC spawning statue, see below.
				int npcIndex = -1;

				// 30 is the 时间 before it 可以 used again. NPC.MechSpawn checks nearby for other spawns to 防止 too many spawns. 3 in immediate vicinity, 6 nearby, 10 in 世界.
				int spawnedNpcId = NPCID.Goldfish;

				if (Wiring.CheckMech(x, y, 30) && NPC.MechSpawn(spawnX, spawnY, spawnedNpcId)) {
					npcIndex = NPC.NewNPC(entitySource, (int)spawnX, (int)spawnY - 12, spawnedNpcId);
				}

				if (npcIndex >= 0) {
					var npc = Main.npc[npcIndex];

					npc.value = 0f;
					npc.npcSlots = 0f;
					// 防止s Loot if NPCID.Sets.NoEarlymodeLootWhenSpawnedFromStatue and !Main.HardMode or NPCID.Sets.StatueSpawnedDropRarity != -1 and NextFloat() >= NPCID.Sets.StatueSpawnedDropRarity or killed by traps.
					// 防止s CatchNPC
					npc.SpawnedFromStatue = true;
				}
			}
		}
	}
}