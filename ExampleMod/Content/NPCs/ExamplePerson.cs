using ExampleMod.Common;
using ExampleMod.Common.Configs;
using ExampleMod.Common.Systems;
using ExampleMod.Content.Biomes;
using ExampleMod.Content.Dusts;
using ExampleMod.Content.EmoteBubbles;
using ExampleMod.Content.Items;
using ExampleMod.Content.Items.Accessories;
using ExampleMod.Content.Items.Armor;
using ExampleMod.Content.Projectiles;
using ExampleMod.Content.Tiles;
using ExampleMod.Content.Tiles.Furniture;
using ExampleMod.Content.Walls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace ExampleMod.Content.NPCs
{
	// [AutoloadHead] and NPC.townNPC are extremely important and absolutely 两者 necessary for 任何 Town NPC to work at all.
	[AutoloadHead]
	public class ExamplePerson : ModNPC
	{
		public const string ShopName = "Shop";
		public int NumberOfTimesTalkedTo = 0;

		private static int ShimmerHeadIndex;
		private static Profiles.StackedNPCProfile NPCProfile;

		public override void Load() {
			// 添加s our Shimmer Head 到 NPCHeadLoader.
			ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
		}

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 25; // The total amount of frames the NPC has

			NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things 例如 sitting in a chair and talking to other NPCs. This is the remaining frames after the walking frames.
			NPCID.Sets.AttackFrameCount[Type] = 4; // The amount of frames 在 attacking 动画.
			NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away 从 中心 的 NPC that it tries to 攻击 enemies.
			NPCID.Sets.AttackType[Type] = 0; // The 类型 of 攻击 the Town NPC performs. 0 = throwing, 1 = shooting, 2 = magic, 3 = melee
			NPCID.Sets.AttackTime[Type] = 90; // The amount of 时间 it takes 对于 NPC's 攻击 动画 to be over once it starts.
			NPCID.Sets.AttackAverageChance[Type] = 30; // The denominator 对于 概率 for a Town NPC to 攻击. Lower numbers make the Town NPC appear more aggressive.
			NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y 偏移.
			NPCID.Sets.ShimmerTownTransform[NPC.type] = true; // This set says th在 Town NPC has a Shimmered form. Otherwise, the Town NPC 将come transparent when touching Shimmer like other enemies.

			NPCID.Sets.ShimmerTownTransform[Type] = true; // 允许s for this NPC to have a different 纹理 after touching the Shimmer liquid.

			// Connects this NPC with a custom emote.
			// 这使 it when the NPC is 在 世界, other NPCs will "talk about him".
			// By 设置 this you don't 必须 override the PickEmote 方法 对于 emote to appear.
			NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<ExamplePersonEmote>();

			// Influences how the NPC looks 在 Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				Velocity = 1f, // 绘制s the NPC 在 bestiary as if its walking +1 tiles 在 x 方向
				Direction = 1 // -1 is 左 and 1 is 右. NPCs are drawn facing the 左 默认情况下 but ExamplePerson 将 drawn facing the 右
				// 旋转 = MathHelper.ToRadians(180) // 你可以 also change the 旋转 of an NPC. 旋转 is measured in radians
				// 如果 you 想要 see an example of manually modifying these when the NPC is drawn, see PreDraw
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			// 设置 Example Person's 生物群系 and neighbor preferences 与 NPCHappiness hook. You can add happiness 文本 and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
			// NOTE: The following code uses chaining - a style that works due 到 fact th在 SetXAffection methods 返回 the same NPCHappiness 实例 they're called on.
			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Like) // 示例 Person prefers the forest.
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike) // 示例 Person dislikes the snow.
				.SetBiomeAffection<ExampleSurfaceBiome>(AffectionLevel.Love) // 示例 Person likes the Example Surface 生物群系
				.SetNPCAffection(NPCID.Dryad, AffectionLevel.Love) // Loves living near the dryad.
				.SetNPCAffection(NPCID.Guide, AffectionLevel.Like) // 像s living near the guide.
				.SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike) // Dislikes living near the 商人.
				.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate) // Hates living near the demolitionist.
			; // < Mind the semicolon!

			// This creates a "profile" 例如Person, which allows for different textures during a party and/or while the NPC is shimmered.
			NPCProfile = new Profiles.StackedNPCProfile(
				new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture + "_Party"),
				new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex, Texture + "_Shimmer_Party")
			);
		}

		public override void SetDefaults() {
			NPC.townNPC = true; // 设置s NPC to be a Town NPC
			NPC.friendly = true; // NPC Will not 攻击 玩家
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = 7;
			NPC.damage = 10;
			NPC.defense = 15;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;

			AnimationType = NPCID.Guide;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// 我们 can use AddRange 代替 calling Add 多个 times in 顺序 to add 多个 items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// 设置s the preferred biomes of this town NPC listed 在 bestiary.
				// With Town NPCs, you usually set this to what 生物群系 it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// 设置s your NPC's flavor 文本 在 bestiary.
				new FlavorTextBestiaryInfoElement("Hailing from a mysterious greyscale cube world, the Example Person is here to help you understand everything about tModLoader."),

				// 你 can add 多个 elements if you really wanted to
				// 你 can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.ExampleMod.Bestiary.ExamplePerson")
			});
		}

		// PreDraw hook is useful for drawing things before our 精灵 is drawn or running code before the 精灵 is drawn
		// 返回ing 假 will 允许 you to manually draw your NPC
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
			// This code slowly rotates the NPC 在 bestiary
			// (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.旋转 won't work here as it gets overridden by drawModifiers.旋转 each tick)
			if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers)) {
				drawModifiers.Rotation += 0.001f;

				// 替换 the existing NPCBestiaryDrawModifiers with our new one with an adjusted 旋转
				NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
				NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
			}

			return true;
		}

		public override void HitEffect(NPC.HitInfo hit) {
			int num = NPC.life > 0 ? 1 : 5;

			for (int k = 0; k < num; k++) {
				Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
			}

			// 创建 gore when the NPC is killed.
			if (Main.netMode != NetmodeID.Server && NPC.life <= 0) {
				// 检索 the gore types. This NPC has shimmer and party variants for head, arm, and leg gore. (12 total gores)
				string variant = "";
				if (NPC.IsShimmerVariant) variant += "_Shimmer";
				if (NPC.altTexture == 1) variant += "_Party";
				int hatGore = NPC.GetPartyHatGore();
				int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
				int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
				int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

				// 生成 the gores. The positions 的 arms and legs are lowered for a more natural look.
				if (hatGore > 0) {
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, hatGore);
				}
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
			}
		}

		public override void OnSpawn(IEntitySource source) {
			if(source is EntitySource_SpawnNPC) {
				// 一个 TownNPC is "unlocked" once it successfully spawns in到 世界.
				TownNPCRespawnSystem.unlockedExamplePersonSpawn = true;
			}
		}

		public override bool CanTownNPCSpawn(int numTownNPCs) { // 需要ments 对于 town NPC to 生成.
			if (TownNPCRespawnSystem.unlockedExamplePersonSpawn) {
				// 如果 Example Person has spawned in this 世界 before, we don't require the 用户 satisfying the ExampleItem/ExampleBlock 库存 conditions for a 重生.
				return true;
			}

			foreach (var player in Main.ActivePlayers) {
				// 玩家 has to have 任一 an ExampleItem or an ExampleBlock in 顺序 对于 NPC to 生成
				if (player.inventory.Any(item => item.type == ModContent.ItemType<ExampleItem>() || item.type == ModContent.ItemType<Items.Placeable.ExampleBlock>())) {
					return true;
				}
			}

			return false;
		}

		// 示例 Person needs a house built out of ExampleMod tiles. You can 删除 this whole 方法 in your townNPC 对于 regular house conditions.
		public override bool CheckConditions(int left, int right, int top, int bottom) {
			int score = 0;
			for (int x = left; x <= right; x++) {
				for (int y = top; y <= bottom; y++) {
					int type = Main.tile[x, y].TileType;
					if (type == ModContent.TileType<ExampleBlock>() || type == ModContent.TileType<ExampleChair>() || type == ModContent.TileType<ExampleWorkbench>() || type == ModContent.TileType<ExampleBed>() || type == ModContent.TileType<ExampleDoorOpen>() || type == ModContent.TileType<ExampleDoorClosed>()) {
						score++;
					}

					if (Main.tile[x, y].WallType == ModContent.WallType<ExampleWall>()) {
						score++;
					}
				}
			}

			return score >= ((right - left) * (bottom - top)) / 2;
		}

		public override ITownNPCProfile TownNPCProfile() {
			return NPCProfile;
		}

		public override List<string> SetNPCNameList() {
			return new List<string>() {
				"Someone",
				"Somebody",
				"Blocky",
				"Colorless"
			};
		}

		public override void FindFrame(int frameHeight) {
			/*npc.frame.Width = 40;
			if (((int)Main.time / 10) % 2 == 0)
			{
				npc.frame.X = 40;
			}
			else
			{
				npc.frame.X = 0;
			}*/
		}

		public override string GetChat() {
			WeightedRandom<string> chat = new WeightedRandom<string>();

			int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
			if (partyGirl >= 0 && Main.rand.NextBool(4)) {
				chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.PartyGirlDialogue", Main.npc[partyGirl].GivenName));
			}
			// These are things th在 NPC has a 概率 of telling you when you talk to it.
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.StandardDialogue1"));
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.StandardDialogue2"));
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.StandardDialogue3"));
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.StandardDialogue4"));
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.CommonDialogue"), 5.0);
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.RareDialogue"), 0.1);

			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 10) {
				//This 计数器 is linked to a single 实例 的 NPC, so if ExamplePerson is killed, the 计数器 will 重置.
				chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.TalkALot"));
			}

			string chosenChat = chat; // chat is implicitly cast to a 字符串. This is where the 随机 choice is made.

			// 在这里 is some additional logic based 在 chosen chat line. In this case, we 想要 显示 an 项 在 corner for StandardDialogue4.
			if (chosenChat == Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.StandardDialogue4")) {
				// Main.npcChatCornerItem shows a single 项 在 corner, like the Angler 任务 chat.
				Main.npcChatCornerItem = ItemID.HiveBackpack;
			}

			return chosenChat;
		}

		public override void SetChatButtons(ref string button, ref string button2) { // Wh在 chat buttons are when you 打开 up the chat 用户界面
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = "Awesomeify";
			if (Main.LocalPlayer.HasItem(ItemID.HiveBackpack)) {
				button = "Upgrade " + Lang.GetItemNameValue(ItemID.HiveBackpack);
			}
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop) {
			if (firstButton) {
				// 我们 want 3 different functionalities for chat buttons, so we use HasItem to change 按钮 1 between a 商店 and 升级 action.

				if (Main.LocalPlayer.HasItem(ItemID.HiveBackpack)) {
					SoundEngine.PlaySound(SoundID.Item37); // Reforge/Anvil 声音

					Main.npcChatText = $"I upgraded your {Lang.GetItemNameValue(ItemID.HiveBackpack)} to a {Lang.GetItemNameValue(ModContent.ItemType<WaspNest>())}";

					int hiveBackpackItemIndex = Main.LocalPlayer.FindItem(ItemID.HiveBackpack);
					var entitySource = NPC.GetSource_GiftOrReward();

					Main.LocalPlayer.inventory[hiveBackpackItemIndex].TurnToAir();
					Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<WaspNest>());

					return;
				}

				shop = ShopName; // 名称 的 商店 选项卡 we 想要 打开.
			}
		}

		// Not completely finished, but below is wh在 NPC will 出售
		public override void AddShops() {
			var npcShop = new NPCShop(Type, ShopName)
				.Add<ExampleItem>()
				//.Add<EquipMaterial>()
				//.Add<BossItem>()
				.Add(new Item(ModContent.ItemType<Items.Placeable.Furniture.ExampleWorkbench>()) { shopCustomPrice = Item.buyPrice(copper: 15) }) // 此示例 sets a custom 价格, ExampleNPCShop.cs has more info on custom prices and 货币. 
				.Add<Items.Placeable.Furniture.ExampleChair>()
				.Add<Items.Placeable.Furniture.ExampleDoor>()
				.Add<Items.Placeable.Furniture.ExampleBed>()
				.Add<Items.Placeable.Furniture.ExampleChest>()
				.Add<Items.Tools.ExamplePickaxe>()
				.Add<Items.Tools.ExampleHamaxe>()
				.Add<Items.Consumables.ExampleHealingPotion>(new Condition("Mods.ExampleMod.Conditions.PlayerHasLifeforceBuff", () => Main.LocalPlayer.HasBuff(BuffID.Lifeforce)))
				.Add<Items.Weapons.ExampleSword>(Condition.MoonPhasesQuarter0)
				//.Add<ExampleGun>(条件.MoonPhasesQuarter1)
				.Add<Items.Ammo.ExampleBullet>(Condition.MoonPhasesQuarter1)
				.Add<Items.Weapons.ExampleStaff>(ExampleConditions.DownedMinionBoss)
				.Add<ExampleOnBuyItem>()
				.Add(ItemID.AcornAxe) // Here is an example of how to 出售 an existing vanilla 项.
				.Add<Items.Weapons.ExampleYoyo>(Condition.IsNpcShimmered); // Let's 出售 an yoyo if this NPC is shimmered!

			if (ModContent.GetInstance<ExampleModConfig>().ExampleWingsToggle) {
				npcShop.Add<ExampleWings>(ExampleConditions.InExampleBiome);
			}

			if (ModContent.TryFind("SummonersAssociation/BloodTalisman", out ModItem bloodTalisman)) {
				npcShop.Add(bloodTalisman.Type);
			}
			npcShop.Register(); // 名称 of this 商店 选项卡
		}

		public override void ModifyActiveShop(string shopName, Item[] items) {
			foreach (Item item in items) {
				// 跳过 'air' items and 空 items.
				if (item == null || item.type == ItemID.None) {
					continue;
				}

				// 如果 NPC is shimmered then reduce all prices by 50%.
				if (NPC.IsShimmerVariant) {
					int value = item.shopCustomPrice ?? item.value;
					item.shopCustomPrice = value / 2;
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExampleCostume>()));
		}

		// 使 this Town NPC 传送 到 King and/or Queen statue when triggered. 返回 toKingStatue for only King Statues. 返回 !toKingStatue for only Queen Statues. 返回 真 for 两者.
		public override bool CanGoToStatue(bool toKingStatue) => true;

		// 使 something happen when the npc teleports to a statue. Since this 方法 only runs 服务器 side, 任何 visual effects like dusts or gores 必须 be synced across all clients manually.
		public override void OnGoToStatue(bool toKingStatue) {
			if (Main.netMode == NetmodeID.Server) {
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)ExampleMod.MessageType.ExampleTeleportToStatue);
				packet.Write((byte)NPC.whoAmI);
				packet.Send();
			}
			else {
				StatueTeleport();
			}
		}

		// 创建 a square of pixels around the NPC on 传送.
		public void StatueTeleport() {
			for (int i = 0; i < 30; i++) {
				Vector2 position = Main.rand.NextVector2Square(-20, 21);
				if (Math.Abs(position.X) > Math.Abs(position.Y)) {
					position.X = Math.Sign(position.X) * 20;
				}
				else {
					position.Y = Math.Sign(position.Y) * 20;
				}

				Dust.NewDustPerfect(NPC.Center + position, ModContent.DustType<Sparkle>(), Vector2.Zero).noGravity = true;
			}
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
			projType = ModContent.ProjectileType<SparklingBall>();
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
			multiplier = 12f;
			randomOffset = 2f;
			// SparklingBall is not affected by gravity, so gravityCorrection is 左 alone.
		}

		public override void LoadData(TagCompound tag) {
			NumberOfTimesTalkedTo = tag.GetInt("numberOfTimesTalkedTo");
		}

		public override void SaveData(TagCompound tag) {
			tag["numberOfTimesTalkedTo"] = NumberOfTimesTalkedTo;
		}

		// Let the NPC "talk about" 仆从 Boss
		public override int? PickEmote(Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor) {
			// 默认情况下 this NPC will have a 概率 to use the 仆从 Boss Emote even if 仆从 Boss is not downed yet
			int type = ModContent.EmoteBubbleType<MinionBossEmote>();
			// 如果 the NPC is talking 到 Demolitionist, it 将 more likely to react with angry emote
			if (otherAnchor.entity is NPC { type: NPCID.Demolitionist }) {
				type = EmoteID.EmotionAnger;
			}

			// 使 the selection more likely by adding it 到 列表 多个 times
			for (int i = 0; i < 4; i++) {
				emoteList.Add(type);
			}

			// 使用 this or 返回 空 if you don't 想要 override the emote selection totally
			return base.PickEmote(closestPlayer, emoteList, otherAnchor);
		}
	}
}