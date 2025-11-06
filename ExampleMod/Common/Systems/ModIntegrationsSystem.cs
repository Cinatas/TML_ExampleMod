using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace ExampleMod.Common.Systems
{
	// 展示使用其他模组的 Mod.Call 来促进模组集成/兼容性/支持
	// Mod.Call 在此处解释 https://github.com/tModLoader/tModLoader/wiki/Expert-Cross-Mod-Content#call-aka-modcall-intermediate
	// 这只是展示了实现此类集成的一种方式，你可以自由探索自己的选项和其他模组示例

	// 你需要查找模组开发者提供的关于他们希望你如何添加模组兼容性的资源
	// 这可以是他们的主页、创意工坊页面、wiki、GitHub、Discord、其他联系方式等。
	// If the mod is 打开 source, you can visit its code distribution platform (usually GitHub) and look for "Call" in its Mod 类

	// 另外 到 examples shown here, ExampleMod also integrates 与 Census Mod (https://steamcommunity.com/sharedfiles/filedetails/?ID=2687866031)
	// That integration is done solely through localization files, look for "Census.SpawnCondition" 在 .hjson files. 
	public class ModIntegrationsSystem : ModSystem
	{
		public override void PostSetupContent() {
			// Most often, mods require you to use the PostSetupContent hook to call their methods. This guarantees 各种 数据 is initialized and set up properly

			// Boss Checklist shows comprehensive information about bosses in its own 用户界面. 我们可以 customize it:
			// https://forums.terraria.org/索引.php?threads/.50668/
			DoBossChecklistIntegration();

			// 我们可以 integrate with other mods here by following the same pattern. Some modders may prefer a ModSystem for each mod they integrate with, or some other design.
		}

		private void DoBossChecklistIntegration() {
			// The mods homepage links to its own wiki where the calls are explained: https://github.com/JavidPack/BossChecklist/wiki/%5B1.4.4%5D-Boss-日志-Entry-Mod-Call
			// If we navigate the wiki, 我们可以 查找 the "LogBoss" 方法, which we want 在这种情况下
			// A feature 的 call is that it will create an entry 在 localization 文件 的 specified NPC 类型 for its 生成 info, so 确保 to visit the localization 文件 after your mod runs once to edit it

			if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod)) {
				return;
			}

			// For some messages, mods might not have them at release, so we 需要 验证 when the last 迭代 的 方法 variation was first added 到 mod, 在这种情况下 1.6
			// Usually mods 任一 provide that informati在mselves in some way, or it's found 在 GitHub through commit history/blame
			if (bossChecklistMod.Version < new Version(1, 6)) {
				return;
			}

			// The "LogBoss" 方法 requires m任何 parameters, defined separately below:

			// Your entry 键 可以 used by other developers to submit mod-collaborative 数据 to your entry. It should 不 changed once defined
			string internalName = "MinionBoss";

			// 值 inferred from Boss progression, see the wiki for details
			float weight = 0.7f;

			// 使用d for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedMinionBoss;

			// The NPC 类型 的 Boss
			int bossType = ModContent.NPCType<Content.NPCs.MinionBoss.MinionBossBody>();

			// The 项 用于 summ在 Boss with (如果可用)
			int spawnItem = ModContent.ItemType<Content.Items.Consumables.MinionBossSummonItem>();

			// "collectibles" like relic, 奖杯, mask, 宠物
			List<int> collectibles = new List<int>()
			{
				ModContent.ItemType<Content.Items.Placeable.Furniture.MinionBossRelic>(),
				ModContent.ItemType<Content.Pets.MinionBossPet.MinionBossPetItem>(),
				ModContent.ItemType<Content.Items.Placeable.Furniture.MinionBossTrophy>(),
				ModContent.ItemType<Content.Items.Armor.Vanity.MinionBossMask>()
			};

			// 默认情况下, it draws the first 帧 的 Boss, omit if you don't need custom drawing
			// But we 想要 draw the bestiary 纹理 instead, so we create the code for that to draw centered 在 intended 位置
			var customPortrait = (SpriteBatch sb, Rectangle rect, Color color) => {
				Texture2D texture = ModContent.Request<Texture2D>("ExampleMod/Assets/Textures/Bestiary/MinionBoss_Preview").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				sb.Draw(texture, centered, color);
			};

			bossChecklistMod.Call(
				"LogBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>() {
					["spawnItems"] = spawnItem,
					["collectibles"] = collectibles,
					["customPortrait"] = customPortrait
					// Other optional arguments 根据需要 are inferred 从 wiki
				}
			);

			// Other bosses or additional Mod.Call 可以 made here.
		}
	}
}
