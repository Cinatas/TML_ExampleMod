using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using ExampleMod.Common.Systems;
using System.Linq;

namespace ExampleMod.Content.NPCs.TownPets
{
	[AutoloadHead]
	public class ExampleTownPet : ModNPC
	{
		// Where our additional head sprites 将 stored.
		internal static int HeadIndex1;
		internal static int HeadIndex2;
		internal static int HeadIndex3;
		internal static int HeadIndex4;
		internal static int HeadIndex5;
		internal static int HeadIndex6;

		private static ITownNPCProfile NPCProfile;

		public override void Load() {
			// 添加s our variant heads 到 NPCHeadLoader.
			HeadIndex1 = Mod.AddNPCHeadTexture(Type, $"{Texture}_1_Head");
			HeadIndex2 = Mod.AddNPCHeadTexture(Type, $"{Texture}_2_Head");
			HeadIndex3 = Mod.AddNPCHeadTexture(Type, $"{Texture}_3_Head");
			HeadIndex4 = Mod.AddNPCHeadTexture(Type, $"{Texture}_4_Head");
			HeadIndex5 = Mod.AddNPCHeadTexture(Type, $"{Texture}_5_Head");
			HeadIndex6 = Mod.AddNPCHeadTexture(Type, $"{Texture}_6_Head");
		}

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 27; // The 数字 of frames our 精灵 has.
			NPCID.Sets.ExtraFramesCount[Type] = 20; // The 数字 of frames after the walking frames.
			NPCID.Sets.AttackFrameCount[Type] = 0; // Town Pets don't have any attacking frames.
			NPCID.Sets.DangerDetectRange[Type] = 250; // How far away the NPC will detect danger. Measured in pixels.
			NPCID.Sets.AttackType[Type] = -1; // Town Pets do not 攻击. The default for this set is -1, so it is safe to 删除 this line if you wish.
			NPCID.Sets.AttackTime[Type] = -1; // Town Pets do not 攻击. The default for this set is -1, so it is safe to 删除 this line if you wish.
			NPCID.Sets.AttackAverageChance[Type] = 1;  // Town Pets do not 攻击. The default for this set is 1, so it is safe to 删除 this line if you wish.
			NPCID.Sets.HatOffsetY[Type] = -2; // An 偏移 for where the party hat sits 在 精灵.
			NPCID.Sets.ShimmerTownTransform[Type] = false; // Town Pets don't have a Shimmer variant.
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Shimmer] = true; // But they are still immune to Shimmer.
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; // And Confused.
			NPCID.Sets.ExtraTextureCount[Type] = 0; // Even though we have several variation textures, we don't use this set. The default for this set is 0, so it is safe to 删除 this line if you wish.
			NPCID.Sets.NPCFramingGroup[Type] = 8; // How the party hat is animated to 匹配 the walking 动画. Town Cat = 4, Town Dog = 5, Town Bunny = 6, Town Slimes = 7, No 偏移 = 8

			NPCID.Sets.IsTownPet[Type] = true; // Our NPC is a Town 宠物
			NPCID.Sets.CannotSitOnFurniture[Type] = false; // 真 默认情况下 which means they cannot sit in chairs. 真 means they can sit on furniture like the Town Cat.
			NPCID.Sets.TownNPCBestiaryPriority.Add(Type); // Puts our NPC with all 的 other Town NPCs.
			NPCID.Sets.PlayerDistanceWhilePetting[Type] = 32; // 距离 the 玩家 stands 从 Town 宠物 to 宠物.
			NPCID.Sets.IsPetSmallForPetting[Type] = true; // If set to 真, the 玩家's arm 将 angled down while petting.

			// Influences how the NPC looks 在 Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new() {
				Velocity = 0.25f, // 绘制s the NPC 在 bestiary as if its walking +0.25 tiles 在 x 方向
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			NPCProfile = new ExampleTownPetProfile(); // Assign our profile.
		}

		public override void SetDefaults() {
			NPC.townNPC = true; // Town Pets are still considered Town NPCs
			NPC.friendly = true;
			NPC.width = 20;
			NPC.height = 20;
			NPC.aiStyle = NPCAIStyleID.Passive;
			NPC.damage = 10;
			NPC.defense = 15;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.knockBackResist = 0.5f;
			NPC.housingCategory = 1; // This means it can share a house with a normal Town NPC.
			AnimationType = NPCID.TownBunny; // This example matches the animations 的 Town Bunny.
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("Mods.ExampleMod.Bestiary.ExampleTownPet")
			});
		}

		public override bool CanTownNPCSpawn(int numTownNPCs) {
			// 如果 we've used the License, our Town 宠物 can freely 重生.
			if (ExampleTownPetSystem.boughtExampleTownPet) {
				return true;
			}
			return false;
		}

		public override ITownNPCProfile TownNPCProfile() {
			// Vanilla Town Pets use Profiles.VariantNPCProfile() to set the variants, but that doesn't work for us because
			// it uses Main.Assets.请求<T>() which won't 查找 mod assets (ModContent.请求<T>() is needed instead).
			// So, we make our own NPCProfile. (See Below)
			return NPCProfile;
		}

		// 创建 a bunch of lists for our names. Each variant gets its own 列表 of names.
		// 你 can these lists as long or as short as you'd like. 
		public readonly List<string> NameList0 = new() {
			"Monochromatic", "Grayscale", "Unpainted"
		};
		public readonly List<string> NameList1 = new() {
			"Red", "Crimson", "Cinnabar", "Maroon", "Scarlet"
		};
		public readonly List<string> NameList3 = new() {
			"Blue", "Azure", "Sky", "Ultramarine", "Navy"
		};
		public readonly List<string> NameList4 = new() {
			"Cyan", "Turquoise", "Teal", "Aqua", "Keppel"
		};
		public readonly List<string> NameList5 = new() {
			"Yellow", "Gold", "Cream", "Lemon", "Solar"
		};
		public readonly List<string> NameList6 = new() {
			"Magenta", "Pink", "Purple", "Violet", "Fuchsia"
		};

		public override List<string> SetNPCNameList() {
			return NPC.townNpcVariationIndex switch { // 更改 the 名称 based 在 variation.
				0 => NameList0,
				1 => NameList1, // Variant 1 将 the Shimmered variant if your NPC has a shimmer variant.
				// Green (2) variant shows one approach to localizing Town NPC names.
				// One additional benefit of this approach is a 分离 mod can add a Mods.ExampleMod.NPCs.ExampleTownPet.Names.Green.Emerald 键 and it will automatically be used as an 名称 选项.
				2 => Language.FindAll(Lang.CreateDialogFilter(this.GetLocalizationKey("Names.Green"))).Select(x => x.Value).ToList(),
				3 => NameList3,
				4 => NameList4,
				5 => NameList5,
				6 => NameList6,
				_ => NameList0
			};
		}

		public override string GetChat() {
			WeightedRandom<string> chat = new();

			chat.Add("*Example Town Pet noises*");

			return chat;
		}

		public override void SetChatButtons(ref string button, ref string button2) {
			button = Language.GetTextValue("UI.PetTheAnimal"); // Automatically translated to say "宠物"
		}

		public override bool PreAI() {
			// 如果 your Town 宠物 can sit in chairs with NPCID.Sets.CannotSitOnFurniture[类型] = 假
			// 我们 want to 移动 the Town NPC up visually to 匹配 the 高度 的 chair.
			// NPC.ai[0] is set to 5f for Town NPC AI when they are sitting in a chair.
			if (NPC.ai[0] == 5f) {
				DrawOffsetY = -10; // 记住: Negative Y is up. So, this is moving the NPC up visually by 10 pixels.
			}
			else {
				DrawOffsetY = 0; // 重置 it back to 0 when not sitting in a chair.
			}
			// Do not try to add or subtract 从 DrawOffsetY. It'll cause the 精灵 to change its 高度 every 帧 which will make it go off 的 屏幕.

			// 如果 your Town 宠物 doesn't sit in furniture, you can 删除 this entire PreAI() 方法.

			return base.PreAI();
		}

		public override void ChatBubblePosition(ref Vector2 position, ref SpriteEffects spriteEffects) {
			// 如果 your Town 宠物 can sit in chairs with NPCID.Sets.CannotSitOnFurniture[类型] = 假
			// and you've done the above DrawOffsetY to raise it up 到 chair's 高度,
			// you'll notice the chat bubble that appears when hovering over them doesn't get raised up.
			// So, let's 移动 it up 以及.
			if (NPC.ai[0] == 5f) { // (Sitting in a chair.)
				position.Y -= 18f; // 移动 upwards.
			}
		}

		/*
		public override void EmoteBubblePosition(ref Vector2 position, ref SpriteEffects spriteEffects) {
			// 在这里 is an example of how we can modify the emote bubble.

			// Flip the emote bubble and 移动 it.
			spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			position.X += NPC.width * NPC.direction;

			// (Town Pets can't use emotes, so this example here won't actually do anything.)
			// (For a working example, see ExampleMod/Common/GlobalNPCs/GuideGlobalNPC.cs)
		}
		*/

		public override void PartyHatPosition(ref Vector2 position, ref SpriteEffects spriteEffects) {
			// With this hook, we have full 控制 over the 位置 的 party hat.
			// 我们 have already set NPCID.Sets.HatOffsetY[类型] = -2 in SetStaticDefaults which will 移动 the party hat up 2 pixels at all times.
			// 我们 also set PCID.Sets.NPCFramingGroup[类型] = 8 in SetStaticDefaults.
			// NPCFramingGroup is used vertically 偏移 the party hat to 匹配 the animations 的 NPC.
			// 分组 8 has no inherit offsets 对于 party hat.

			int frame = NPC.frame.Y / NPC.frame.Height; // The current 帧.
			int xOffset = 8; // 移动 the party hat forward so it is actually 在 Town 宠物's head.
			// Then 移动 the party hat 左/右 depending 在 帧.
			// These numbers were achieved by measuring the 精灵 relative 到 "normal" 位置 的 party hat.
			switch (frame) {
				case 1:
				case 2:
				case 7:
				case 9:
					xOffset -= 2;
					break;
				case 3:
				case 8:
					xOffset -= 4;
					break;
				case 11:
				case 15:
				case 16:
				case 17:
				case 26:
					xOffset += 2;
					break;
				case 12:
				case 13:
				case 14:
				case 18:
				case 24:
				case 25:
					xOffset += 4;
					break;
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
					xOffset += 6;
					break;
				default:
					break;
			}
			position.X += xOffset * NPC.spriteDirection;

			// 我们 set NPCID.Sets.HatOffsetY[类型] = -2 so that means every 帧 is moved up 2 additional units.
			int yOffset = 0;
			// Then 移动 the party hat up/down depending 在 帧.
			// These numbers were achieved by measuring the 精灵 relative 到 "normal" 位置 的 party hat.
			switch (frame) {
				case 3:
				case 4:
					yOffset -= 2;
					break;
				case 5:
				case 6:
				case 10:
				case 17:
				case 26:
					yOffset += 2;
					break;
				case 18:
				case 25:
					yOffset += 4;
					break;
				case 11:
				case 15:
				case 16:
				case 24:
					yOffset += 6;
					break;
				case 19:
				case 20:
				case 23:
					yOffset += 8;
					break;
				case 21:
				case 22:
					yOffset += 10;
					break;
				case 12:
				case 14:
					yOffset += 12;
					break;
				case 13:
					yOffset += 14;
					break;
				default:
					break;
			}
			position.Y += yOffset;

			// 移动 it up to 匹配 the 位置 的 head when sitting in a chair.
			if (NPC.ai[0] == 5f) {
				position.Y += -10;
			}
		}
	}

	public class ExampleTownPetProfile : ITownNPCProfile
	{
		private static readonly string filePath = "ExampleMod/Content/NPCs/TownPets/ExampleTownPet"; // The 路径 to our base 纹理.

		// 加载 all of our textures only one 时间 during mod 加载 时间.
		private readonly Asset<Texture2D> variant0 = ModContent.Request<Texture2D>(filePath);
		private readonly Asset<Texture2D> variant1 = ModContent.Request<Texture2D>($"{filePath}_1");
		private readonly Asset<Texture2D> variant2 = ModContent.Request<Texture2D>($"{filePath}_2");
		private readonly Asset<Texture2D> variant3 = ModContent.Request<Texture2D>($"{filePath}_3");
		private readonly Asset<Texture2D> variant4 = ModContent.Request<Texture2D>($"{filePath}_4");
		private readonly Asset<Texture2D> variant5 = ModContent.Request<Texture2D>($"{filePath}_5");
		private readonly Asset<Texture2D> variant6 = ModContent.Request<Texture2D>($"{filePath}_6");
		private readonly int headIndex0 = ModContent.GetModHeadSlot($"{filePath}_Head");

		public int RollVariation() {
			int random = Main.rand.Next(7); // 7 variants; 0 through 6.

			// 如果 your Town 宠物 has a shimmer variant:
			// townNpcVariationIndex of 1 makes it shimmered, even when it isn't.
			// if (随机 == 1) {
			//	随机 = 7; // So variation 1 becomes 数字 7.
			// }

			return random;
		}

		public string GetNameForVariant(NPC npc) => npc.getNewNPCName(); // Reroll the 名称 每次 the Town 宠物 spawns or changes variant.

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) {
			return npc.townNpcVariationIndex switch {
				0 => variant0,
				1 => variant1, // Variant 1 将 the Shimmered variant if your NPC has a shimmer variant.
				2 => variant2,
				3 => variant3,
				4 => variant4,
				5 => variant5,
				6 => variant6,
				_ => variant0
			};
		}

		public int GetHeadTextureIndex(NPC npc) {
			return npc.townNpcVariationIndex switch {
				0 => headIndex0,
				1 => ExampleTownPet.HeadIndex1, // Variant 1 将 the Shimmered variant if your NPC has a shimmer variant.
				2 => ExampleTownPet.HeadIndex2,
				3 => ExampleTownPet.HeadIndex3,
				4 => ExampleTownPet.HeadIndex4,
				5 => ExampleTownPet.HeadIndex5,
				6 => ExampleTownPet.HeadIndex6,
				_ => headIndex0
			};
		}
	}
}