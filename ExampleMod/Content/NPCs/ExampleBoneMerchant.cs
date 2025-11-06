using ExampleMod.Content.Dusts;
using ExampleMod.Content.EmoteBubbles;
using ExampleMod.Content.Items;
using ExampleMod.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ExampleMod.Content.NPCs
{
	/// <summary>
	/// The main focus of this NPC is to show how to make something similar 到 vanilla bone 商人;
	/// which means th在 NPC will act like any other town NPC but won't have a happiness 按钮, won't appear 在 minimap,
	/// and will 生成 like an 敌人 NPC. If you want a traditional town NPC instead, see <see cref="ExamplePerson"/>.
	/// </summary>
	public class ExampleBoneMerchant : ModNPC
	{
		private static Profiles.StackedNPCProfile NPCProfile;
		private static Asset<Texture2D> shimmerGun;

		public override void Load() {
			shimmerGun = ModContent.Request<Texture2D>(Texture + "_Shimmer_Gun");
		}

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 25; // The amount of frames the NPC has

			NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things 例如 sitting in a chair and talking to other NPCs.
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away 从 中心 的 npc that it tries to 攻击 enemies.
			NPCID.Sets.PrettySafe[Type] = 300;
			NPCID.Sets.AttackType[Type] = 1; // Shoots a 武器.
			NPCID.Sets.AttackTime[Type] = 60; // The amount of 时间 it takes 对于 NPC's 攻击 动画 to be over once it starts.
			NPCID.Sets.AttackAverageChance[Type] = 30;
			NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y 偏移.
			NPCID.Sets.ShimmerTownTransform[NPC.type] = true; // This set says th在 Town NPC has a Shimmered form. Otherwise, the Town NPC 将come transparent when touching Shimmer like other enemies.

			//This sets entry is the most important part of this NPC. Since it is 真, it tells the game that we want this NPC to act like a town NPC without ACTUALLY being one.
			//What that means is: the NPC will have the AI of a town NPC, will 攻击 like a town NPC, and have a 商店 (or any other additional functionality if you wish) like a town NPC.
			//However, the NPC will not have their head displayed 在 地图, will de-生成 when no players are nearby or the 世界 is closed, and will 生成 like any other NPC.
			NPCID.Sets.ActsLikeTownNPC[Type] = true;

			// This prevents the happiness 按钮
			NPCID.Sets.NoTownNPCHappiness[Type] = true;

			//To reiterate, since this NPC isn't technically a town NPC, we need to tell the game that we still want this NPC to have a custom/randomized 名称 when they 生成.
			//In 顺序 to do this, we simply make this hook 返回 真, which will make the game call the TownNPCName 方法 when spawning the NPC to determine the NPC's 名称.
			NPCID.Sets.SpawnsWithCustomName[Type] = true;

			// Connects this NPC with a custom emote.
			// This makes it when the NPC is 在 世界, other NPCs will "talk about him".
			NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<ExampleBoneMerchantEmote>();

			//The vanilla Bone 商人 cannot interact with doors (打开 or 关闭 them, specifically), but if you want your NPC to be able to interact 与m despite this,
			//uncomment this line below.
			//NPCID.Sets.AllowDoorInteraction[类型] = 真;

			// Influences how the NPC looks 在 Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				Velocity = 1f, // 绘制s the NPC 在 bestiary as if its walking +1 tiles 在 x 方向
				Direction = 1 // -1 is 左 and 1 is 右. NPCs are drawn facing the 左 默认情况下 but ExamplePerson 将 drawn facing the 右
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			NPCProfile = new Profiles.StackedNPCProfile(
				new Profiles.DefaultNPCProfile(Texture, -1),
				new Profiles.DefaultNPCProfile(Texture + "_Shimmer", -1)
			);
		}

		public override void SetDefaults() {
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

		//Make sure to 允许 your NPC to chat, since being "like a town NPC" doesn't automatically 允许 for chatting.
		public override bool CanChat() {
			return true;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// 我们 can use AddRange instead of calling Add multiple times in 顺序 to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// 设置s the preferred biomes of this town NPC listed 在 bestiary.
				// With Town NPCs, you usually set this to what 生物群系 it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,

				// 设置s your NPC's flavor 文本 在 bestiary.
				new FlavorTextBestiaryInfoElement("Hailing from a mysterious greyscale cube world, the Example Bone Merchant will show you how to make a mysterious merchant underground with tModLoader."),

				// 你 can add multiple elements if you really wanted to
				// 你 can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.ExampleMod.Bestiary.ExampleBoneMerchant")
			});
		}

		public override void HitEffect(NPC.HitInfo hit) {
			// Causes dust to 生成 when the NPC takes 伤害.
			int num = NPC.life > 0 ? 1 : 5;

			for (int k = 0; k < num; k++) {
				Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
			}

			// 创建 gore when the NPC is killed.
			if (Main.netMode != NetmodeID.Server && NPC.life <= 0) {
				// 检索 the gore types. This NPC only has shimmer variants. (6 total gores)
				string variant = "";
				if (NPC.IsShimmerVariant) variant += "_Shimmer";
				int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
				int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
				int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

				// 生成 the gores. The positions 的 arms and legs are lowered for a more natural look.
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
			}
		}

		public override ITownNPCProfile TownNPCProfile() {
			return NPCProfile;
		}

		public override List<string> SetNPCNameList() {
			return new List<string> {
				"Blocky Bones",
				"Someone's Ribcage",
				"Underground Blockster",
				"Darkness"
			};
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			//如果有的话 玩家 is underground and has an example 项 在ir 库存, the example bone 商人 will have a slight 概率 to 生成.
			if (spawnInfo.Player.ZoneDirtLayerHeight && spawnInfo.Player.inventory.Any(item => item.type == ModContent.ItemType<ExampleItem>())) {
				return 0.34f;
			}

			//Else, the example bone 商人 will not 生成 if the above conditions are not met.
			return 0f;
		}

		public override string GetChat() {
			WeightedRandom<string> chat = new WeightedRandom<string>();

			// These are things th在 NPC has a 概率 of telling you when you talk to it.
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExampleBoneMerchant.StandardDialogue1"));
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExampleBoneMerchant.StandardDialogue2"));
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExampleBoneMerchant.StandardDialogue3"));
			return chat; // chat is implicitly cast to a 字符串.
		}

		public override void SetChatButtons(ref string button, ref string button2) { // Wh在 chat buttons are when you 打开 up the chat 用户界面
			button = Language.GetTextValue("LegacyInterface.28"); //This is the 键 到 word "商店"
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop) {
			if (firstButton) {
				shop = "Shop";
			}
		}

		public override void AddShops() {
			new NPCShop(Type)
				.Add<ExampleItem>()
				.Register();
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
			damage = 20;
			knockback = 2f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
			cooldown = 10;
			randExtraCooldown = 1;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
			projType = ProjectileID.NanoBullet;
			attackDelay = 1;

			// This code progressively delays subsequent shots.
			if (NPC.localAI[3] > attackDelay) {
				attackDelay = 12;
			}
			if (NPC.localAI[3] > attackDelay) {
				attackDelay = 24;
			}
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
			multiplier = 10f;
			randomOffset = 0.2f;
		}

		public override void TownNPCAttackShoot(ref bool inBetweenShots) {
			if (NPC.localAI[3] > 1) {
				inBetweenShots = true;
			}
		}

		public override void DrawTownAttackGun(ref Texture2D item, ref Rectangle itemFrame, ref float scale, ref int horizontalHoldoutOffset) {
			if (!NPC.IsShimmerVariant) {
				// 如果 using an existing 项, use this approach
				int itemType = ModContent.ItemType<ExampleCustomAmmoGun>();
				Main.GetItemDrawFrame(itemType, out item, out itemFrame);
				horizontalHoldoutOffset = (int)Main.DrawPlayerItemPos(1f, itemType).X - 12;
			}
			else {
				// This 纹理 isn't actually an existing 项, but can still be used.
				item = shimmerGun.Value;
				itemFrame = item.Frame();
				horizontalHoldoutOffset = -2;
			}
		}
	}
}
