using ExampleMod.Content.Biomes;
using ExampleMod.Content.Buffs;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace ExampleMod.Content.NPCs
{
	// Party Zombie is a pretty basic clone of a vanilla NPC. To learn how to further adapt vanilla NPC behaviors, see https://github.com/tModLoader/tModLoader/wiki/Advanced-Vanilla-Code-Adaption#example-npc-npc-clone-with-modified-弹幕-hoplite
	public class PartyZombie : ModNPC
	{
		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

			NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.Skeleton;

			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers() { // Influences how the NPC looks 在 Bestiary
				Velocity = 1f // 绘制s the NPC 在 bestiary as if its walking +1 tiles 在 x 方向
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults() {
			NPC.width = 18;
			NPC.height = 40;
			NPC.damage = 14;
			NPC.defense = 6;
			NPC.lifeMax = 200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = 60f;
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = 3; // Fighter AI, important to choose the aiStyle that matches the NPCID that we 想要 mimic

			AIType = NPCID.Zombie; // 使用 vanilla zombie's 类型 when executing AI code. (This also means it will 尝试 despawn during daytime)
			AnimationType = NPCID.Zombie; // 使用 vanilla zombie's 类型 when executing 动画 code. Important to also 匹配 Main.npcFrameCount[NPC.类型] in SetStaticDefaults.
			Banner = Item.NPCtoBanner(NPCID.Zombie); // 使 this NPC get affected by the normal zombie banner.
			BannerItem = Item.BannerToItem(Banner); // 使 kills of this NPC go towards dropping the banner it's associated with.
			SpawnModBiomes = new int[1] { ModContent.GetInstance<ExampleSurfaceBiome>().Type }; // Associates this NPC 与 ExampleSurfaceBiome in Bestiary
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			// Since Party Zombie is essentially just another variation of Zombie, we'd like to mimic the Zombie drops.
			// 要 do this, we can 任一 (1) 复制 the drops 从 Zombie directly or (2) just recreate the drops in our code.
			// (1) Copying the drops directly means that if Terraria updates and changes the Zombie drops, your ModNPC will also inherit the changes automatically.
			// (2) Recreating the drops can give you more 控制 if desired but requires consulting the wiki, bestiary, or source code 然后 writing 放下 code.

			// (1) This example shows copying the drops directly. For consistency and mod 兼容性, we suggest using the smallest positive NPCID when dealing with npcs with m任何 variants and shared 放下 pools.
			var zombieDropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.Zombie, false); // 假 is important here
			foreach (var zombieDropRule in zombieDropRules) {
				// 在 this foreach 循环, we simple add each 放下 到 PartyZombie 放下 pool. 
				npcLoot.Add(zombieDropRule);
			}

			// (2) This example shows recreating the drops. This code is commented out because we are using the previous 方法 instead.
			// npcLoot.Add(ItemDropRule.Common(ItemID.Shackle, 50)); // 放下 shackles with a 1 out of 50 概率.
			// npcLoot.Add(ItemDropRule.Common(ItemID.ZombieArm, 250)); // 放下 zombie arm with a 1 out of 250 概率.

			// 最后, we can add additional drops. M任何 Zombie variants have their own unique drops: https://terraria.fandom.com/wiki/Zombie
			npcLoot.Add(ItemDropRule.Common(ItemID.Confetti, 100)); // 1% 概率 to 放下 Confetti
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			return SpawnCondition.OverworldNightMonster.Chance * 0.2f; // 生成 with 1/5th the 概率 of a regular zombie.
		}

		public override void AI() {
			if (NPC.wet) {
				if (NPC.honeyWet) { // 删除s the effects of honey's fall rate making the NPC fall normally in honey
					NPC.GravityMultiplier /= NPC.GravityWetMultipliers[LiquidID.Honey];
					NPC.MaxFallSpeedMultiplier /= NPC.MaxFallSpeedWetMultipliers[LiquidID.Honey];
				}
				else if (!NPC.lavaWet && !NPC.shimmerWet) { // 删除s water falls 速度 effects, then adds honey falls 速度 effects, making the NPC fall 在 honey rate in water
					NPC.GravityMultiplier *= NPC.GravityWetMultipliers[LiquidID.Honey] / NPC.GravityWetMultipliers[LiquidID.Water];
					NPC.MaxFallSpeedMultiplier *= NPC.MaxFallSpeedWetMultipliers[LiquidID.Honey] / NPC.MaxFallSpeedWetMultipliers[LiquidID.Water];
				}
			}
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// 我们 can use AddRange 代替 calling Add 多个 times in 顺序 to add 多个 items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// 设置s the spawning conditions of this NPC 即 listed 在 bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// 设置s the 描述 of this NPC 即 listed 在 bestiary.
				new FlavorTextBestiaryInfoElement("This type of zombie for some reason really likes to spread confetti around. Otherwise, it behaves just like a normal zombie."),

				// 默认情况下 the last added IBestiaryBackgroundImagePathAndColorProvider 将 用于 show the 背景 图像.
				// 示例SurfaceBiome ModBiomeBestiaryInfoElement is automatically populated into bestiaryEntry.Info prior to this 方法 being called
				// so we use this line to tell the game to prioritize a specific InfoElement for sourcing the 背景 图像.
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<ExampleSurfaceBiome>().ModBiomeBestiaryInfoElement),
			});
		}

		public override void HitEffect(NPC.HitInfo hit) {
			// 生成 confetti when this zombie is hit.

			for (int i = 0; i < 10; i++) {
				int dustType = Main.rand.Next(139, 143);
				var dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);

				dust.velocity.X += Main.rand.NextFloat(-0.05f, 0.05f);
				dust.velocity.Y += Main.rand.NextFloat(-0.05f, 0.05f);

				dust.scale *= 1f + Main.rand.NextFloat(-0.03f, 0.03f);
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
			// 在这里 we can make things happen if this NPC hits a 玩家 via its hitbox (not projectiles it shoots, this is handled 在 弹幕 code usually)
			// 常见 use is applying buffs/debuffs:

			int buffType = ModContent.BuffType<AnimatedBuff>();
			// Alternatively, you can use a vanilla 增益: int buffType = BuffID.Slow;

			int timeToAdd = 5 * 60; //This makes it 5 seconds, one second is 60 ticks
			target.AddBuff(buffType, timeToAdd);
		}

		public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers) {
			if (modifiers.DamageType.CountsAsClass(DamageClass.Magic)) {
				// 此示例 shows how PartyZombie reduces magic 伤害 by 75%. We use FinalDamage here rather than SourceDamage since we are affecting how the npc reacts 到 伤害.
				// Conceptually, the source dealing the 伤害 isn't interpreted as weaker, but rather this NPC has a resistance to this 伤害 source.
				modifiers.FinalDamage *= 0.25f;
			}
		}
	}
}
