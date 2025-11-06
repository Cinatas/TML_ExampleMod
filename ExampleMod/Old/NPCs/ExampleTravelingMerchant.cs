using ExampleMod.Dusts;
using ExampleMod.Items;
using ExampleMod.Items.Placeable;
using ExampleMod.Projectiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.NPCs
{
	[AutoloadHead]
	class ExampleTravelingMerchant : ModNPC
	{
		// 时间 of day for traveller to leave (6PM)
		public const double despawnTime = 48600.0;

		// the 时间 of day the traveler will 生成 (double.MaxValue for no 生成)
		// 保存d and loaded 与 世界 in ExampleWorld
		public static double spawnTime = double.MaxValue;

		// The 列表 of items 在 traveler's 商店. Saved 与 世界 and 重置 when a new traveler spawns
		public static List<Item> shopItems = new List<Item>();

		public static NPC FindNPC(int npcType) => Main.npc.FirstOrDefault(npc => npc.type == npcType && npc.active);

		public static void UpdateTravelingMerchant() {
			NPC traveler = FindNPC(NPCType<ExampleTravelingMerchant>()); // 查找 an Explorer if there's one spawned 在 世界
			if (traveler != null && (!Main.dayTime || Main.time >= despawnTime) && !IsNpcOnscreen(traveler.Center)) // If it's past the despawn 时间 and the NPC isn't onscreen
			{
				// Here we despawn the NPC and send a 消息 stating th在 NPC has despawned
				if (Main.netMode == NetmodeID.SinglePlayer) Main.NewText(traveler.FullName + " has departed!", 50, 125, 255);
				else NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(traveler.FullName + " has departed!"), new Color(50, 125, 255));
				traveler.active = false;
				traveler.netSkip = -1;
				traveler.life = 0;
				traveler = null;
			}

			// Main.时间 is set to 0 each morning, and only for one 更新. Sundialling will never 跳过 past 时间 0 so this is the place for 'on new day' code
			if (Main.dayTime && Main.time == 0) {
				// insert code here to change the 生成 概率 基于 other conditions (say, npcs which have arrived, or milestones the 玩家 has passed)
				// 你可以 also add a day 计数器 here to 防止 the 商人 from possibly spawning 多个 days in a 行.

				// NPC won't 生成 today if it stayed all night
				if (traveler == null && Main.rand.NextBool(4)) { // 4 = 25% 概率
																// Here 我们可以 make it so the NPC doesnt 生成 在 EXACT same 时间 每次 it does 生成
					spawnTime = GetRandomSpawnTime(5400, 8100); // minTime = 6:00am, maxTime = 7:30am
				}
				else {
					spawnTime = double.MaxValue; // no 生成 today
				}
			}

			// 生成 the traveler if the 生成 conditions are met (时间 of day, no events, no sundial)
			if (traveler == null && CanSpawnNow()) {
				int newTraveler = NPC.NewNPC(Main.spawnTileX * 16, Main.spawnTileY * 16, NPCType<ExampleTravelingMerchant>(), 1); // 生成ing 在 世界 生成
				traveler = Main.npc[newTraveler];
				traveler.homeless = true;
				traveler.direction = Main.spawnTileX >= WorldGen.bestX ? -1 : 1;
				traveler.netUpdate = true;
				shopItems = CreateNewShop();

				// 防止s the traveler from spawning aga在 same day
				spawnTime = double.MaxValue;

				// Annouce th在 traveler has spawned in!
				if (Main.netMode == NetmodeID.SinglePlayer) Main.NewText(Language.GetTextValue("Announcement.HasArrived", traveler.FullName), 50, 125, 255);
				else NetMessage.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasArrived", traveler.GetFullNetName()), new Color(50, 125, 255));
			}
		}

		private static bool CanSpawnNow() {
			// can't 生成 如果有的话 events are running
			if (Main.eclipse || Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0)
				return false;

			// can't 生成 if the sundial is active
			if (Main.fastForwardTime)
				return false;

			// can 生成 if daytime, and between the 生成 and despawn times
			return Main.dayTime && Main.time >= spawnTime && Main.time < despawnTime;
		}

		private static bool IsNpcOnscreen(Vector2 center) {
			int w = NPC.sWidth + NPC.safeRangeX * 2;
			int h = NPC.sHeight + NPC.safeRangeY * 2;
			Rectangle npcScreenRect = new Rectangle((int)center.X - w / 2, (int)center.Y - h / 2, w, h);
			foreach (Player player in Main.player) {
				// 如果有的话 玩家 is 关闭 enough 到 traveling 商人, it will 防止 the npc from despawning
				if (player.active && player.getRect().Intersects(npcScreenRect)) return true;
			}
			return false;
		}

		public static double GetRandomSpawnTime(double minTime, double maxTime) {
			// A simple 公式 to get a 随机 时间 between two chosen times
			return (maxTime - minTime) * Main.rand.NextDouble() + minTime;
		}

		public static List<Item> CreateNewShop() {
			// 创建 a 列表 of 项 ids
			var itemIds = new List<int>();

			// For each 槽位 we add a switch case to determine what should go in that 槽位
			switch (Main.rand.Next(2)) {
				case 0:
					itemIds.Add(ItemType<ExampleItem>());
					break;
				default:
					itemIds.Add(ItemType<EquipMaterial>());
					break;
			}

			switch (Main.rand.Next(3)) {
				case 0:
					itemIds.Add(ItemType<BossItem>());
					break;
				case 1:
					itemIds.Add(ItemType<ExampleWorkbench>());
					break;
				default:
					itemIds.Add(ItemType<ExampleChair>());
					break;
			}

			switch (Main.rand.Next(4)) {
				case 0:
					itemIds.Add(ItemType<ExampleDoor>());
					break;
				case 1:
					itemIds.Add(ItemType<ExampleBed>());
					break;
				case 2:
					itemIds.Add(ItemType<ExampleChest>());
					break;
				default:
					itemIds.Add(ItemType<ExamplePickaxe>());
					break;
			}

			// conver to a 列表 of items
			var items = new List<Item>();
			foreach (int itemId in itemIds) {
				Item item = new Item();
				item.SetDefaults(itemId);
				items.Add(item);
			}
			return items;
		}

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Example Traveler");
			Main.npcFrameCount[npc.type] = 25;
			NPCID.Sets.ExtraFramesCount[npc.type] = 9;
			NPCID.Sets.AttackFrameCount[npc.type] = 4;
			NPCID.Sets.DangerDetectRange[npc.type] = 700;
			NPCID.Sets.AttackType[npc.type] = 0;
			NPCID.Sets.AttackTime[npc.type] = 90;
			NPCID.Sets.AttackAverageChance[npc.type] = 30;
			NPCID.Sets.HatOffsetY[npc.type] = 4;
		}

		public override void SetDefaults() {
			npc.townNPC = true; // This 将 changed once the NPC is spawned
			npc.friendly = true;
			npc.width = 18;
			npc.height = 40;
			npc.aiStyle = 7;
			npc.damage = 10;
			npc.defense = 15;
			npc.lifeMax = 250;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.knockBackResist = 0.5f;
			animationType = NPCID.Guide;
		}

		public static TagCompound Save() {
			return new TagCompound {
				["spawnTime"] = spawnTime,
				["shopItems"] = shopItems
			};
		}

		public static void Load(TagCompound tag) {
			spawnTime = tag.GetDouble("spawnTime");
			shopItems = tag.Get<List<Item>>("shopItems");
		}

		public override void HitEffect(int hitDirection, double damage) {
			int num = npc.life > 0 ? 1 : 5;
			for (int k = 0; k < num; k++) {
				Dust.NewDust(npc.position, npc.width, npc.height, DustType<Sparkle>());
			}
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money) {
			return false; // 这应该 always be 假, because we 生成 在 Travleing 商人 manually
		}

		public override string TownNPCName() {
			switch (Main.rand.Next(4)) {
				case 0:
					return "Someone";
				case 1:
					return "Somebody";
				case 2:
					return "Blockster";
				default:
					return "Colorful";
			}
		}

		public override string GetChat() {
			int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
			if (partyGirl >= 0 && Main.rand.NextBool(4)) {
				return "Can you please tell " + Main.npc[partyGirl].GivenName + " to stop decorating my cousin's house with colors?";
			}
			switch (Main.rand.Next(4)) {
				case 0:
					return "Sometimes my cousin feels like they're different from everyone else here.";
				case 1:
					return "What's your favorite color? My cousin's favorite colors are white and black.";
				case 2: {
						// Main.npcChatCornerItem shows a single 项 在 corner, like the Angler 任务 chat.
						Main.npcChatCornerItem = ItemID.HiveBackpack;
						return $"Hey, if you find a [i:{ItemID.HiveBackpack}], my cousin can upgrade it for you.";
					}
				default:
					return "What? My cousin doesn't have any arms or legs? Oh, don't be ridiculous!";
			}
		}

		public override void SetChatButtons(ref string button, ref string button2) {
			button = Language.GetTextValue("LegacyInterface.28");
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop) {
			if (firstButton) {
				shop = true;
			}
		}

		public override void SetupShop(Chest shop, ref int nextSlot) {
			foreach (Item item in shopItems) {
				// We dont want "empty" items and unloaded items to appear
				if (item == null || item.type == ItemID.None)
					continue;

				shop.item[nextSlot].SetDefaults(item.type);
				nextSlot++;
			}
		}

		public override void AI() {
			npc.homeless = true; // 使 sure it stays homeless
		}

		public override void NPCLoot() {
			Item.NewItem(npc.getRect(), ItemType<Items.Armor.ExampleCostume>());
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
			damage = 20;
			knockback = 4f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
			cooldown = 30;
			randExtraCooldown = 30;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
			projType = ProjectileType<SparklingBall>();
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
			multiplier = 12f;
			randomOffset = 2f;
		}
	}
}
