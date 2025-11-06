using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace ExampleMod.Content.NPCs
{
	/// <summary>
	/// This 文件 shows off a critter npc. The unique thing about critters is how you can catch them with a bug net.
	/// The important bits are: Main.npcCatchable, NPC.catchItem, and 项.makeNPC.
	/// We will also show off adding an 项 to an existing RecipeGroup (see ExampleRecipes.AddRecipeGroups).
	/// Additionally, this example shows an involved IL edit.
	/// </summary>
	public class ExampleCritterNPC : ModNPC
	{
		private const int ClonedNPCID = NPCID.Frog; // Easy to change 类型 for your modder convenience

		public override void Load() {
			IL_Wiring.HitWireSingle += HookFrogStatue;
		}

		/// <summary>
		/// Change the following code 序列 in Wiring.HitWireSingle
		/// <code>
		///case 61:
		///num115 = 361;
		/// </code>
		/// to
		/// <code>
		///case 61:
		///num115 = Main.rand.NextBool() ? 361 : NPC.类型
		/// </code>
		/// This causes the frog statue to 生成 this NPC 50% 的 时间
		/// </summary>
		/// <param 名称="ilContext"> </param>
		private void HookFrogStatue(ILContext ilContext) {
			try {
				// Obtain a cursor positioned before the first 指令 的 方法 the cursor is used for navigating and modifying the il
				ILCursor ilCursor = new ILCursor(ilContext);

				// exact 位置 for this hook is very complex to 搜索 for due 到 hook instructions 不ing unique and buried deep in 控制 flow. Switch statements are sometimes compiled to if-else chains, and 调试 builds litter the code with no-ops and redundant locals.
				// 在 general you 想要 搜索 using structure and 函数 而不是 numerical constants which may change across different versions or compile settings. Using local 变量 indices is almost always a bad idea.
				// 我们 can 搜索 for
				// switch (*)
				//   case 61:
				//     num115 = 361;

				// 在 general you'd 想要 look for a specific switch 变量, or perhaps the containing switch (类型) { case 105: but the generated IL is really 变量 and hard to 匹配 在这种情况下.
				// We'll just use the fact th在re are no other switch statements with case 61

				ILLabel[] targets = null;
				while (ilCursor.TryGotoNext(i => i.MatchSwitch(out targets))) {
					// Some optimizing compilers generate a sub 以便 all the switch cases 开始 at 0:
					// ldc.i4.s 30
					// sub
					// switch
					int offset = 0;
					if (ilCursor.Prev.MatchSub() && ilCursor.Prev.Previous.MatchLdcI4(out offset)) {
						;
					}

					// 获取 the 标签 for case 61: if it exists
					int case61Index = 61 - offset;
					if (case61Index < 0 || case61Index >= targets.Length || targets[case61Index] is not ILLabel target) {
						continue;
					}

					// 移动 the cursor to case 61:
					ilCursor.GotoLabel(target);
					// 移动 the cursor after 361 is pushed on到 堆叠
					ilCursor.Index++;
					// 有 lots of extra checks we could add here to 确保 we're 在 右 spot, 例如 not encountering 任何 branching instructions

					// Now we add additional code to modify the current 值 that 将 assigned to num115
					ilCursor.EmitDelegate((int originalAssign) => Main.rand.NextBool() ? originalAssign : NPC.type);

					// Hook applied successfully
					return;
				}

				// Couldn't 查找 the 右 place to insert.
				throw new Exception("Hook location not found, switch(*) { case 61: ...");
			}
			catch {
				// 如果 there are 任何 failures 与 IL editing, this 方法 will dump the IL to Logs/ILDumps/{Mod 名称}/{方法 名称}.txt
				MonoModHooks.DumpIL(ModContent.GetInstance<ExampleMod>(), ilContext);
			}
		}

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = Main.npcFrameCount[ClonedNPCID]; // 复制 动画 frames
			Main.npcCatchable[Type] = true; // This is for certain release situations

			// These three are typical critter values
			NPCID.Sets.CountsAsCritter[Type] = true;
			NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
			NPCID.Sets.TownCritter[Type] = true;

			// frog is immune to confused
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

			// 这是 so it appears between the frog and the 金币 frog
			NPCID.Sets.NormalGoldCritterBestiaryPriority.Insert(NPCID.Sets.NormalGoldCritterBestiaryPriority.IndexOf(ClonedNPCID) + 1, Type);
		}

		public override void SetDefaults() {
			// 宽度 = 12;
			// 高度 = 10;
			// aiStyle = 7;
			// 伤害 = 0;
			// 防御 = 0;
			// lifeMax = 5;
			// HitSound = SoundID.NPCHit1;
			// DeathSound = SoundID.NPCDeath1;
			// catchItem = 2121;
			// 设置s the above
			NPC.CloneDefaults(ClonedNPCID);

			NPC.catchItem = ModContent.ItemType<ExampleCritterItem>();
			NPC.lavaImmune = true;
			AIType = ClonedNPCID;
			AnimationType = ClonedNPCID;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.AddTags(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
				new FlavorTextBestiaryInfoElement("The most adorable goodest spicy child. Do not dare be mean to him!"));
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			return SpawnCondition.Underworld.Chance * 0.1f;
		}

		public override void HitEffect(NPC.HitInfo hit) {
			if (NPC.life <= 0) {
				for (int i = 0; i < 6; i++) {
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Worm, 2 * hit.HitDirection, -2f);
					if (Main.rand.NextBool(2)) {
						dust.noGravity = true;
						dust.scale = 1.2f * NPC.scale;
					}
					else {
						dust.scale = 0.7f * NPC.scale;
					}
				}
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"{Name}_Gore_Head").Type, NPC.scale);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"{Name}_Gore_Leg").Type, NPC.scale);
			}
		}

		public override Color? GetAlpha(Color drawColor) {
			// 获取Alpha gives our Lava Frog a red glow.
			return drawColor with {
				R = 255,
				// Both these do the same in this situation, using these methods is useful.
				G = Utils.Clamp<byte>(drawColor.G, 175, 255),
				B = Math.Min(drawColor.B, (byte)75),
				A = 255
			};
		}

		public override bool PreAI() {
			// Kills the NPC if it hits water, honey or shimmer
			if (NPC.wet && !Collision.LavaCollision(NPC.position, NPC.width, NPC.height)) { // NPC.lavawet not 100% accurate 对于 frog
				// These 3 lines instantly kill the npc without showing 伤害 numbers, dropping loot, or playing DeathSound. Use this for instant deaths
				NPC.life = 0;
				NPC.HitEffect();
				NPC.active = false;
				SoundEngine.PlaySound(SoundID.NPCDeath16, NPC.position); // plays a fizzle 声音
			}

			return true;
		}

		public override void OnCaughtBy(Player player, Item item, bool failed) {
			if (failed) {
				return;
			}

			Point npcTile = NPC.Center.ToTileCoordinates();

			if (!WorldGen.SolidTile(npcTile.X, npcTile.Y)) { // 检查 if the 图格 the npc resides the most in is non solid
				Tile tile = Main.tile[npcTile];
				tile.LiquidAmount = tile.LiquidType == LiquidID.Lava ? // 检查 if the 图格 has lava in it
					Math.Max((byte)Main.rand.Next(50, 150), tile.LiquidAmount) // If it does, then 顶部 up the amount
					: (byte)Main.rand.Next(50, 150); // If it doesn't, then overwrite the amount. Technically this distinction should never be needed bc it will burn but to be safe it's here
				tile.LiquidType = LiquidID.Lava; // 设置 the liquid 类型 to lava
				WorldGen.SquareTileFrame(npcTile.X, npcTile.Y, true); // 更新 the surrounding 区域 在 tilemap
			}
		}
	}

	public class ExampleCritterItem : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.IsLavaBait[Type] = true; // While this 项 is not bait, this will require a lava bug net to catch.
		}

		public override void SetDefaults() {
			// useStyle = 1;
			// autoReuse = 真;
			// useTurn = 真;
			// useAnimation = 15;
			// useTime = 10;
			// maxStack = CommonMaxStack;
			// consumable = 真;
			// 宽度 = 12;
			// 高度 = 12;
			// 使NPC = 361;
			// noUseGraphic = 真;

			// Cloning ItemID.Frog sets the preceding values
			Item.CloneDefaults(ItemID.Frog);
			Item.makeNPC = ModContent.NPCType<ExampleCritterNPC>();
			Item.value += Item.buyPrice(0, 0, 30, 0); // 使 this critter worth slightly 超过 the frog
			Item.rare = ItemRarityID.Blue;
		}
	}
}