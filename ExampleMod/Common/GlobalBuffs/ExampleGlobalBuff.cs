using ExampleMod.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalBuffs
{
	// 展示如何使用所有增益效果
	public class ExampleGlobalBuff : GlobalBuff
	{
		public static LocalizedText RemainingTimeText { get; private set; }

		public override void SetStaticDefaults() {
			RemainingTimeText = Mod.GetLocalization($"{nameof(ExampleGlobalBuff)}.RemainingTime");
		}

		public override void Update(int type, Player player, ref int buffIndex) {
			// 如果玩家在已有超过 5 个其他增益/减益效果时获得冰冻减益，则将最大持续时间限制为 3 秒
			if (type == BuffID.Chilled && buffIndex >= 5) {
				int limit = 3 * 60;
				if (player.buffTime[buffIndex] > limit) {
					player.buffTime[buffIndex] = limit;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, int type, int buffIndex, ref BuffDrawParams drawParams) {
			// 使篝火增益具有不同的颜色并轻微抖动
			if (type == BuffID.Campfire) {
				drawParams.DrawColor = Main.DiscoColor * Main.buffAlpha[buffIndex];

				Vector2 shake = new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3));

				drawParams.Position += shake;
				drawParams.TextPosition += shake;
			}

			// 如果增益不在钩爪/坐骑/宠物装备页面中绘制，并且增益是三个指定的之一：
			if (Main.EquipPage != 2 && (type == BuffID.Regeneration || type == BuffID.Ironskin || type == BuffID.Swiftness)) {
				// 使每个增益的文本上下移动 6 像素，每个偏移 4 个刻度
				int interval = 60;
				float time = ((int)Main.GameUpdateCount + 4 * buffIndex) % interval / (float)interval;

				int offset = (int)(6 * time);

				ref Vector2 textPos = ref drawParams.TextPosition; // 你可以使用 ref 局部变量来持续修改同一变量
				textPos.Y += offset;
			}

			// 返回 true 以让游戏绘制增益图标。
			return true;
		}

		private static string randomBuffTextCache;
		private static int randomBuffTypeCache;

		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare) {
			// 此代码为合适的增益添加更可扩展的剩余时间工具提示
			Player player = Main.LocalPlayer;

			int buffIndex = player.FindBuffIndex(type);
			if (buffIndex < 0 || buffIndex >= player.buffTime.Length) {
				return;
			}

			if (Main.TryGetBuffTime(buffIndex, out int buffTimeValue) && buffTimeValue > 2) {
				string text = Lang.LocalizedDuration(new System.TimeSpan(0, 0, buffTimeValue / 60), abbreviated: false, showAllAvailableUnits: true);
				tip += "\n" + RemainingTimeText.Format(text);
			}

			// 此代码展示了调整 buffName。通过激活蛋糕片方块来试试
			if (player.HasBuff(BuffID.SugarRush) && buffName.Length > 2) {
				if (Main.GameUpdateCount % 10 == 0 || randomBuffTypeCache != type) {
					if (randomBuffTypeCache != type) {
						randomBuffTextCache = buffName;
						randomBuffTypeCache = type;
					}
					char[] characters = randomBuffTextCache.ToCharArray();
					int n = characters.Length;
					int swaps = Main.rand.Next(1, randomBuffTextCache.Length / 2);
					for (int swap = 0; swap < swaps; swap++) {

						int a = Main.rand.Next(n - 1);
						int b = Main.rand.Next(a + 1, n);
						Utils.Swap(ref characters[a], ref characters[b]);
					}
					randomBuffTextCache = new string(characters);
				}
				buffName = randomBuffTextCache;
			}
		}

		public override bool RightClick(int type, int buffIndex) {
			// 此代码使玩家在静止时无法通过右键单击图标移除 "ExampleDefenseBuff"
			if (type == ModContent.BuffType<ExampleDefenseBuff>() && Main.LocalPlayer.velocity == Vector2.Zero) {
				Main.NewText("Cannot cancel this buff while stationary!");
				return false;
			}

			return base.RightClick(type, buffIndex);
		}
	}
}
