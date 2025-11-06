using ExampleMod.Content.Biomes;
using ExampleMod.Content.Items.Tools;
using ExampleMod.Content.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	// 此类展示你可以用钓鱼做什么
	public class ExampleFishingPlayer : ModPlayer
	{
		public bool hasExampleCrateBuff;

		public override void ResetEffects() {
			hasExampleCrateBuff = false;
		}

		public override void ModifyFishingAttempt(ref FishingAttempt attempt) {
			// 如果玩家有示例箱子增益（由示例箱子药水给予），则捕获物是箱子的额外 10% 概率
			// 箱子的等级取决于稀有度，我们在这里不修改它，有关详细信息，请参阅 CatchFish 中的注释
			if (hasExampleCrateBuff && !attempt.crate) {
				if (Main.rand.Next(100) < 10) {
					attempt.crate = true;
				}
			}
		}

		public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition) {
			bool inWater = !attempt.inLava && !attempt.inHoney;
			bool inExampleSurfaceBiome = Player.InModBiome<ExampleSurfaceBiome>();
			if (attempt.playerFishingConditions.PoleItemType == ModContent.ItemType<ExampleFishingRod>() && inWater && inExampleSurfaceBiome) {
				// In this example, we will fish up an Example Person 从 water in Example Surface 生物群系,
				// as long as there isn't one 在 世界 yet
				// NOTE: if a fishing rod has multiple bobbers, then each one can 生成 the NPC
				int npc = ModContent.NPCType<ExamplePerson>();
				if (!NPC.AnyNPCs(npc)) {
					// 使 sure itemDrop = -1 when summoning an NPC, as otherwise terraria will only 生成 the 项
					npcSpawn = npc;
					itemDrop = -1;

					// 另外, to make it cooler, we will make a special sonar 消息 for when it shows up
					sonar.Text = "Something's wrong...";
					sonar.Color = Color.LimeGreen;
					sonar.Velocity = Vector2.Zero;
					sonar.DurationInFrames = 300;

					// And that 文本 shows up 在 玩家's head, not 在 bobber 位置.
					sonarPosition = new Vector2(Player.position.X, Player.position.Y - 64);

					return; // This is important so your code after this that rolls items will not run
				}
			}

			if (inWater && inExampleSurfaceBiome && attempt.crate) {
				// If the game rolls a crate, we want to give ours 到 玩家 if he is in Example Surface 生物群系

				// We don't want to 替换 golden/titanium crates (the highest tier crates), as they take highest priority in crate catches
				// Their 放下 conditions are "veryrare" or "legendary"
				// (After that come 生物群系 crates ("rare"), then iron/mythril ("uncommon"), then wood/pearl (none 的 previous))
				// Let's 替换 生物群系 crates 50% 的 时间 (玩家 可能 in multiple (modded) biomes, we should respect that)
				if (!attempt.veryrare && !attempt.legendary && attempt.rare && Main.rand.NextBool()) {
					itemDrop = ModContent.ItemType<Content.Items.Consumables.ExampleFishingCrate>();
					return; // This is important so your code after this that rolls items will not run
				}
			}

			// Here we will set the catch conditions for our ExampleQuestFish
			int exampleQuestFish = ModContent.ItemType<Content.Items.ExampleQuestFish>(); // We'll store the 类型 as a 变量, since we'll be referencing it several times
			// 首先 we check if today's 任务 matches our 任务 fish
			if (attempt.questFish == exampleQuestFish) {
				// Our ExampleQuestFish states that it can only be caught whilst upside-down, so we'll have to check the gravity
				// Normal gravity is positive, whilst reversed gravity is negative
				// 最后, most vanilla 任务 fish only appear on an uncommon roll, so we'll do the same
				if (Player.gravDir < 0f && attempt.uncommon) {
					itemDrop = exampleQuestFish;
					return; // While there is no more code that could roll a fish after this, we might add some 在 future so it's best to 返回 here
				}
			}
		}

		public override bool? CanConsumeBait(Item bait) {
			// 玩家.GetFishingConditions() returns you the best fishing pole 项, 类型 and power, the best bait 项, 类型 and Power, and the total fishing 级别, including modded values
			// These are the same Pole and Bait the game considers when calculating the obtained fish.
			// during CanConsumeBait, 玩家.GetFishingConditions() == attempt.playerFishingConditions from CatchFish.
			PlayerFishingConditions conditions = Player.GetFishingConditions();

			// The golden fishing rod will never consume a ladybug
			if ((bait.type == ItemID.LadyBug || bait.type == ItemID.GoldLadyBug) && conditions.Pole.type == ItemID.GoldenFishingRod) {
				return false;
			}

			return null; // Let the default logic run
		}

		// If fishing with ladybug, we will receive multiple "fish" per bobber. Does not apply to 任务 fish
		public override void ModifyCaughtFish(Item fish) {
			// In this example, we make sure that we got a Ladybug as bait, and later on use that to determine what we catch
			if (Player.GetFishingConditions().BaitItemType == ItemID.LadyBug && fish.rare != ItemRarityID.Quest) {
				fish.stack += Main.rand.Next(1, 4);
			}
		}
	}
}
