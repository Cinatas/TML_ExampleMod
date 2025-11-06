using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace ExampleMod.Content.Tiles
{
	public class ExampleOre : ModTile
	{
		public override void SetStaticDefaults() {
			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true; // The 图格 将 affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 410; // Metal Detector 值, see https://terraria.wiki.gg/wiki/Metal_Detector
			Main.tileShine2[Type] = true; // 修改 the draw 颜色 slightly.
			Main.tileShine[Type] = 975; // How often tiny dust appear off this 图格. Larger is less frequently
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(152, 171, 198), name);

			DustType = 84;
			HitSound = SoundID.Tink;
			// MineResist = 4f;
			// MinPick = 200;
		}

		// 示例 of how to 启用 the 生物群系 Sight 增益 to highlight this 图格. 生物群系 Sight is technically intended to show "infected" tiles, so this example is purely for demonstration purposes.
		public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor) {
			sightColor = Color.Blue;
			return true;
		}
	}

	// 示例OreSystem contains code related to spawning ExampleOre. It contains both spawning ore during 世界 生成, seen in ModifyWorldGenTasks, and spawning ore after defeating a Boss, seen in BlessWorldWithExampleOre and MinionBossBody.OnKill.
	public class ExampleOreSystem : ModSystem
	{
		public static LocalizedText ExampleOrePassMessage { get; private set; }
		public static LocalizedText BlessedWithExampleOreMessage { get; private set; }

		public override void SetStaticDefaults() {
			ExampleOrePassMessage = Mod.GetLocalization($"WorldGen.{nameof(ExampleOrePassMessage)}");
			BlessedWithExampleOreMessage = Mod.GetLocalization($"WorldGen.{nameof(BlessedWithExampleOreMessage)}");
		}

		// 此方法 is called from MinionBossBody.OnKill the first 时间 the Boss is killed.
		// logic is located here for organizational purposes.
		public void BlessWorldWithExampleOre() {
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				return; // This should not happen, but just in case.
			}

			// Since this happens during gameplay, we need to run this code on another thread. If we do not, the game will 经验 lag for a brief moment. This is especially necessary for 世界 生成 tasks that would take even longer to execute.
			// 参见 https://github.com/tModLoader/tModLoader/wiki/世界-生成/#long-running-tasks f或更多 information.
			ThreadPool.QueueUserWorkItem(_ => {
				// Broadcast a 消息 to notify the 用户.
				if (Main.netMode == NetmodeID.SinglePlayer) {
					Main.NewText(BlessedWithExampleOreMessage.Value, 50, 255, 130);
				}
				else if (Main.netMode == NetmodeID.Server) {
					ChatHelper.BroadcastChatMessage(BlessedWithExampleOreMessage.ToNetworkText(), new Color(50, 255, 130));
				}

				// 100 controls how many splotches of ore are spawned in到 世界, scaled by 世界 大小. For comparison, the first 3 times altars are smashed about 275, 190, or 120 splotches 的 respective hardmode ores are spawned. 
				int splotches = (int)(100 * (Main.maxTilesX / 4200f));
				int highestY = (int)Utils.Lerp(Main.rockLayer, Main.UnderworldLayer, 0.5);
				for (int iteration = 0; iteration < splotches; iteration++) {
					// 查找 a 点 在 lower half 的 rock 层 but above the underworld depth.
					int i = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
					int j = WorldGen.genRand.Next(highestY, Main.UnderworldLayer);

					// OreRunner will 生成 ExampleOre in splotches. OnKill only runs 在 服务器 or single 玩家, so it is safe to run 世界 生成 code.
					WorldGen.OreRunner(i, j, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), (ushort)ModContent.TileType<ExampleOre>());
				}
			});
		}

		// 世界 生成 is explained more in https://github.com/tModLoader/tModLoader/wiki/世界-生成
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
			// Because 世界 生成 is like layering several images on 顶部 of each other, we need to do some steps between the original 世界 生成 steps.

			// Most vanilla ores are generated in a 步骤 called "Shinies", so for 最大 兼容性, we will also do this.
			// 首先, we 查找 out which 步骤 "Shinies" is.
			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

			if (ShiniesIndex != -1) {
				// Next, we insert our pass directly after the original "Shinies" pass.
				// 示例OrePass is a 类 seen bellow
				tasks.Insert(ShiniesIndex + 1, new ExampleOrePass("Example Mod Ores", 237.4298f));
			}
		}
	}

	public class ExampleOrePass : GenPass
	{
		public ExampleOrePass(string name, float loadWeight) : base(name, loadWeight) {
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration) {
			// progress.消息 is the 消息 shown 到 用户 while the following code is running.
			// Try to make your 消息 清除. You 可以 a little bit clever, but make sure it is descriptive enough for troubleshooting purposes.
			progress.Message = ExampleOreSystem.ExampleOrePassMessage.Value;

			// Ores are quite simple, we simply use a for 循环 and the WorldGen.TileRunner to place splotches 的 specified 图格 在 世界.
			// "6E-05" is "scientific notation". It simply means 0.00006 but in some ways is easier to read.
			for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 6E-05); k++) {
				// inside of this for 循环 corresponds to one single splotch of our Ore.
				// 首先, we randomly choose any 坐标 在 世界 by choosing a 随机 x and y 值.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);

				// WorldGen.worldSurfaceLow is actually the highest surface 图格. In practice you might want to use WorldGen.rockLayer or other WorldGen values.
				int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);

				// Then, we call WorldGen.TileRunner with 随机 "strength" and 随机 "steps", 以及 as the 图格 we wish to place.
				// Feel free to experiment with strength and 步骤 to see the shape they generate.
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), ModContent.TileType<ExampleOre>());

				// Alternately, we could check the 图格 already present 在 坐标 we are interested.
				// Wrapping WorldGen.TileRunner 在 following 条件 would make the ore only generate in Snow.
				// 图格 图格 = Framing.GetTileSafely(x, y);
				// if (图格.HasTile && 图格.TileType == TileID.SnowBlock) {
				// 	WorldGen.TileRunner(.....);
				// }
			}
		}
	}
}
