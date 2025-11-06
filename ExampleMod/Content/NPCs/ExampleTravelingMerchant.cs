using ExampleMod.Content.Dusts;
using ExampleMod.Content.EmoteBubbles;
using ExampleMod.Content.Items;
using ExampleMod.Content.Items.Armor;
using ExampleMod.Content.Items.Placeable;
using ExampleMod.Content.Items.Placeable.Furniture;
using ExampleMod.Content.Items.Tools;
using ExampleMod.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ExampleMod.Content.NPCs
{
	[AutoloadHead]
	class ExampleTravelingMerchant : ModNPC
	{
		// 时间 of day for traveler to leave (6PM)
		public const double despawnTime = 48600.0;

		// the 时间 of day the traveler will 生成 (double.MaxValue for no 生成). Saved and loaded 与 世界 in TravelingMerchantSystem
		public static double spawnTime = double.MaxValue;

		// 列表 of items 在 traveler's 商店. Saved 与 世界 and set when the traveler spawns. Synced by the 服务器 to clients in multi 玩家
		public readonly static List<Item> shopItems = new();

		// 一个 static 实例 的 declarative 商店, defining all the items which 可以 brought. Used to create a new 库存 when the NPC spawns
		public static ExampleTravelingMerchantShop Shop;

		private static int ShimmerHeadIndex;
		private static Profiles.StackedNPCProfile NPCProfile;

		public override bool PreAI() {
			if ((!Main.dayTime || Main.time >= despawnTime) && !IsNpcOnscreen(NPC.Center)) // If it's past the despawn 时间 and the NPC isn't onscreen
			{
				// 在这里 we despawn the NPC and send a 消息 stating th在 NPC has despawned
				// LegacyMisc.35 is {0) has departed!
				if (Main.netMode == NetmodeID.SinglePlayer) Main.NewText(Language.GetTextValue("LegacyMisc.35", NPC.FullName), 50, 125, 255);
				else ChatHelper.BroadcastChatMessage(NetworkText.FromKey("LegacyMisc.35", NPC.GetFullNetName()), new Color(50, 125, 255));
				NPC.active = false;
				NPC.netSkip = -1;
				NPC.life = 0;
				return false;
			}

			return true;
		}

		public override void AddShops() {
			Shop = new ExampleTravelingMerchantShop(NPC.type);

			// 始终 bring an ExampleItem
			Shop.Add<ExampleItem>();

			// Bring 2 Tools
			Shop.AddPool("Tools", slots: 2)
				.Add<ExampleDrill>()
				.Add<ExampleHamaxe>()
				.Add<ExampleFishingRod>()
				.Add<ExampleHookItem>()
				.Add<ExampleBugNet>()
				.Add<ExamplePickaxe>();

			// Bring 4 Weapons
			Shop.AddPool("Weapons", slots: 4)
				.Add<ExampleSword>()
				.Add<ExampleShortsword>()
				.Add<ExampleShootingSword>()
				.Add<ExampleJavelin>()
				.Add<ExampleSpear>()
				.Add<ExampleMagicWeapon>()
				.Add<ExampleGun>()
				.Add<ExampleShotgun>()
				.Add<ExampleMinigun>()
				.Add<ExampleFlail>()
				.Add<ExampleAdvancedFlail>(Condition.Hardmode) // 仅 bring advanced examples in hardmode!
				.Add<ExampleWhip>()
				.Add<ExampleWhipAdvanced>(Condition.Hardmode)
				.Add<ExampleYoyo>();

			// Bring 3 Furniture
			Shop.AddPool("Furniture", slots: 3)
				.Add<ExampleLamp>()
				.Add<ExampleBed>()
				.Add<ExampleChair>()
				.Add<ExampleChest>()
				.Add<ExampleClock>()
				.Add<ExampleDoor>()
				.Add<ExampleSink>()
				.Add<ExampleTable>()
				.Add<ExampleToilet>()
				.Add<ExampleWorkbench>();

			Shop.Register();
		}

		public static void UpdateTravelingMerchant() {
			bool travelerIsThere = (NPC.FindFirstNPC(ModContent.NPCType<ExampleTravelingMerchant>()) != -1); // 查找 a 商人 if there's one spawned 在 世界

			// Main.时间 is set to 0 each morning, and only for one 更新. Sundialling will never 跳过 past 时间 0 so this is the place for 'on new day' code
			if (Main.dayTime && Main.time == 0) {
				// insert code here to change the 生成 概率 基于 other conditions (say, NPCs which have arrived, or milestones the 玩家 has passed)
				// 你 can also add a day 计数器 here to 防止 the 商人 from possibly spawning 多个 days in a 行.

				// NPC won't 生成 today if it stayed all night
				if (!travelerIsThere && Main.rand.NextBool(4)) { // 4 = 25% 概率
					// 在这里 we can make it so the NPC doesn't 生成 在 EXACT same 时间 每次 it does 生成
					spawnTime = GetRandomSpawnTime(5400, 8100); // minTime = 6:00am, maxTime = 7:30am
				}
				else {
					spawnTime = double.MaxValue; // no 生成 today
				}
			}

			// 生成 the traveler if the 生成 conditions are met (时间 of day, no events, no sundial)
			if (!travelerIsThere && CanSpawnNow()) {
				int newTraveler = NPC.NewNPC(Terraria.Entity.GetSource_TownSpawn(), Main.spawnTileX * 16, Main.spawnTileY * 16, ModContent.NPCType<ExampleTravelingMerchant>(), 1); // 生成ing 在 世界 生成
				NPC traveler = Main.npc[newTraveler];
				traveler.homeless = true;
				traveler.direction = Main.spawnTileX >= WorldGen.bestX ? -1 : 1;
				traveler.netUpdate = true;

				// 防止s the traveler from spawning aga在 same day
				spawnTime = double.MaxValue;

				// Announce th在 traveler has spawned in!
				if (Main.netMode == NetmodeID.SinglePlayer) Main.NewText(Language.GetTextValue("Announcement.HasArrived", traveler.FullName), 50, 125, 255);
				else ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasArrived", traveler.GetFullNetName()), new Color(50, 125, 255));
			}
		}

		private static bool CanSpawnNow() {
			// can't 生成 如果有的话 events are running
			if (Main.eclipse || Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0)
				return false;

			// can't 生成 if the sundial is active
			if (Main.IsFastForwardingTime())
				return false;

			// can 生成 if daytime, and between the 生成 and despawn times
			return Main.dayTime && Main.time >= spawnTime && Main.time < despawnTime;
		}

		private static bool IsNpcOnscreen(Vector2 center) {
			int w = NPC.sWidth + NPC.safeRangeX * 2;
			int h = NPC.sHeight + NPC.safeRangeY * 2;
			Rectangle npcScreenRect = new Rectangle((int)center.X - w / 2, (int)center.Y - h / 2, w, h);
			foreach (Player player in Main.ActivePlayers) {
				// 如果 任何 玩家 is 关闭 enough 到 traveling 商人, it will 防止 the npc from despawning
				if (player.getRect().Intersects(npcScreenRect)) {
					return true;
				}
			}
			return false;
		}

		public static double GetRandomSpawnTime(double minTime, double maxTime) {
			// 一个 simple 公式 to get a 随机 时间 between two chosen times
			return (maxTime - minTime) * Main.rand.NextDouble() + minTime;
		}

		public override void Load() {
			// 添加s our Shimmer Head 到 NPCHeadLoader.
			ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
		}

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 25;
			NPCID.Sets.ExtraFramesCount[Type] = 9;
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 60;
			NPCID.Sets.AttackType[Type] = 3; // Swings a 武器. This NPC attacks in roughly the same manner as Stylist
			NPCID.Sets.AttackTime[Type] = 12;
			NPCID.Sets.AttackAverageChance[Type] = 1;
			NPCID.Sets.HatOffsetY[Type] = 4;
			NPCID.Sets.ShimmerTownTransform[Type] = true;
			NPCID.Sets.NoTownNPCHappiness[Type] = true; // 防止s the happiness 按钮
			NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<ExampleTravellingMerchantEmote>();

			// Influences how the NPC looks 在 Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				Velocity = 2f, // 绘制s the NPC 在 bestiary as if its walking +2 tiles 在 x 方向
				Direction = -1 // -1 is 左 and 1 is 右.
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			NPCProfile = new Profiles.StackedNPCProfile(
				new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture + "_Party"),
				new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex)
			);
		}

		public override void SetDefaults() {
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = 7;
			NPC.damage = 10;
			NPC.defense = 15;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			AnimationType = NPCID.Stylist;
			TownNPCStayingHomeless = true;
		}

		public override void OnSpawn(IEntitySource source) {
			shopItems.Clear();
   			shopItems.AddRange(Shop.GenerateNewInventoryList());

			// 在 multi 玩家, ensure the 商店 items are synced with clients (see TravelingMerchantSystem.cs)
			if (Main.netMode == NetmodeID.Server) {
				// 我们 recommend modders avoid sending WorldData too often, or filling it with too much 数据, lest too much bandwidth be consumed sending redundant 数据 repeatedly
				// Consider sending a custom 数据包 代替 WorldData if you have a significant amount of 数据 to synchronise
				NetMessage.SendData(MessageID.WorldData);
   			}
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface
			});
		}

		public override void HitEffect(NPC.HitInfo hit) {
			int num = NPC.life > 0 ? 1 : 5;
			for (int k = 0; k < num; k++) {
				Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
			}

			// 创建 gore when the NPC is killed.
			if (Main.netMode != NetmodeID.Server && NPC.life <= 0) {
				// 检索 the gore types. This NPC has shimmer variants for head, arm, and leg gore. It also has a custom hat gore. (7 gores)
				// This NPC will 生成 任一 the assigned party hat or a custom hat gore when not shimmered. When shimmered the 顶部 hat is part 的 head and no hat gore is spawned.
				int hatGore = NPC.GetPartyHatGore();
				// 如果 not wearing a party hat, and not shimmered, retrieve the custom hat gore 
				if (hatGore == 0 && !NPC.IsShimmerVariant) {
					hatGore = Mod.Find<ModGore>($"{Name}_Gore_Hat").Type;
				}
				string variant = "";
				if (NPC.IsShimmerVariant) variant += "_Shimmer";
				int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
				int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
				int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

				// 生成 the gores. The positions 的 arms and legs are lowered for a more natural look.
				if (hatGore > 0) {
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, hatGore);
				}
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
			}
		}

		public override bool UsesPartyHat() {
			// 示例TravelingMerchant likes to keep his hat on while shimmered.
			if (NPC.IsShimmerVariant) {
				return false;
			}
			return true;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs) {
			return false; // 这应该 always be 假, because we 生成 在 Traveling 商人 manually
		}

		public override ITownNPCProfile TownNPCProfile() {
			return NPCProfile;
		}

		public override List<string> SetNPCNameList() {
			return new List<string>() {
				"Someone Else",
				"Somebody Else",
				"Blockster",
				"Colorful"
			};
		}

		public override string GetChat() {
			WeightedRandom<string> chat = new WeightedRandom<string>();

			int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
			if (partyGirl >= 0) {
				chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExampleTravelingMerchant.PartyGirlDialogue", Main.npc[partyGirl].GivenName));
			}

			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExampleTravelingMerchant.StandardDialogue1"));
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExampleTravelingMerchant.StandardDialogue2"));
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExampleTravelingMerchant.StandardDialogue3"));

			string hivePackDialogue = Language.GetTextValue("Mods.ExampleMod.Dialogue.ExampleTravelingMerchant.HiveBackpackDialogue");
			chat.Add(hivePackDialogue);

			string dialogueLine = chat; // chat is implicitly cast to a 字符串.
			if (hivePackDialogue.Equals(dialogueLine)) {
				// Main.npcChatCornerItem shows a single 项 在 corner, like the Angler 任务 chat.
				Main.npcChatCornerItem = ItemID.HiveBackpack;
			}

			return dialogueLine;
		}

		public override void SetChatButtons(ref string button, ref string button2) {
			button = Language.GetTextValue("LegacyInterface.28");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop) {
			if (firstButton) {
				shop = Shop.Name; // Opens the 商店
			}
		}

		public override void AI() { 
			NPC.homeless = true; // 使 sure it stays homeless
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExampleCostume>()));
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
			damage = 20;
			knockback = 4f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
			cooldown = 15;
			randExtraCooldown = 8;
		}

		public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight) {
			itemWidth = itemHeight = 40;
		}

		public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset) {
			Main.GetItemDrawFrame(ModContent.ItemType<ExampleSword>(), out item, out itemFrame);
			itemSize = 40;
			// This adjustment draws the swing the way town npcs usually do.
			if (NPC.ai[1] > NPCID.Sets.AttackTime[NPC.type] * 0.66f) {
				offset.Y = 12f;
			}
		}
	}

	// 你 have the freedom to implement custom shops however you want
	// 此示例 uses a 'pool' concept where items 将 randomly selected from a pool with equal weight
	// 我们 复制 a bunch of code from NPCShop and NPCShop.Entry, allowing this 商店 to be easily adjusted by other mods.
	// 
	// This uses some fairly advanced C# to avoid being excessively long, so make sure you learn the language before trying to adapt it significantly
	public class ExampleTravelingMerchantShop : AbstractNPCShop
	{
		public new record Entry(Item Item, List<Condition> Conditions) : AbstractNPCShop.Entry
		{
			IEnumerable<Condition> AbstractNPCShop.Entry.Conditions => Conditions;

			public bool Disabled { get; private set; }

			public Entry Disable() {
				Disabled = true;
				return this;
			}

			public bool ConditionsMet() => Conditions.All(c => c.IsMet());
		}

		public record Pool(string Name, int Slots, List<Entry> Entries)
		{
			public Pool Add(Item item, params Condition[] conditions) {
				Entries.Add(new Entry(item, conditions.ToList()));
				return this;
			}

			public Pool Add<T>(params Condition[] conditions) where T : ModItem => Add(ModContent.ItemType<T>(), conditions);
			public Pool Add(int item, params Condition[] conditions) => Add(ContentSamples.ItemsByType[item], conditions);

			// Picks a 数字 of items (up to Slots) 从 entries 列表, provided conditions are met.
			public IEnumerable<Item> PickItems() {
				// 这是 not a fast way to pick items without replacement, but it's certainly easy. Be careful not to do this m任何 m任何 times per 帧, or on huge lists of items.
				var list = Entries.Where(e => !e.Disabled && e.ConditionsMet()).ToList();
				for (int i = 0; i < Slots; i++) {
					if (list.Count == 0)
						break;

					int k = Main.rand.Next(list.Count);
					yield return list[k].Item;

					// 删除 the entry 从 列表 so it can't be selected again this pick
					list.RemoveAt(k);
				}
			}
		}

		public List<Pool> Pools { get; } = new();

		public ExampleTravelingMerchantShop(int npcType) : base(npcType) { }

		public override IEnumerable<Entry> ActiveEntries => Pools.SelectMany(p => p.Entries).Where(e => !e.Disabled);

		public Pool AddPool(string name, int slots) {
			var pool = new Pool(name, slots, new List<Entry>());
			Pools.Add(pool);
			return pool;
		}

		// Some methods to add a pool with a single 项
		public void Add(Item item, params Condition[] conditions) => AddPool(item.ModItem?.FullName ?? $"Terraria/{item.type}", slots: 1).Add(item, conditions);
		public void Add<T>(params Condition[] conditions) where T : ModItem => Add(ModContent.ItemType<T>(), conditions);
		public void Add(int item, params Condition[] conditions) => Add(ContentSamples.ItemsByType[item], conditions);

		// 在这里 is where we actually 'roll' the contents 的 商店
		public List<Item> GenerateNewInventoryList() {
			var items = new List<Item>();
			foreach (var pool in Pools) {
				items.AddRange(pool.PickItems());
			}
			return items;
		}

		public override void FillShop(ICollection<Item> items, NPC npc) {
			// use the items which were selected when the NPC spawned.
			foreach (var item in ExampleTravelingMerchant.shopItems) {
				// 使 sure to add a clone 的 项, in case 任何 ModifyActiveShop hooks adjust the 项 when the 商店 is opened
				items.Add(item.Clone());
			}
		}

		public override void FillShop(Item[] items, NPC npc, out bool overflow) {
			overflow = false;
			int i = 0;
			// use the items which were selected when the NPC spawned.
			foreach (var item in ExampleTravelingMerchant.shopItems) {

				if (i == items.Length - 1) {
					// leave the last 槽位 empty for selling
					overflow = true;
					return;
				}

				// 使 sure to add a clone 的 项, in case 任何 ModifyActiveShop hooks adjust the 项 when the 商店 is opened
				items[i++] = item.Clone();
			}
		}
	}
}
