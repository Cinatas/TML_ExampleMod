using ExampleMod.Content;
using ExampleMod.Content.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	/// <summary>
	/// ModPlayer class coupled with <seealso cref="ExampleInfoDisplay"/> and <seealso cref="ExampleInfoAccessory"/> to show off how to properly add a
	/// new info accessory (such as a Radar, Lifeform Analyzer, etc.)
	/// </summary>
	public class ExampleInfoDisplayPlayer : ModPlayer
	{
		// 标志检查信息显示何时应该被激活
		public bool showMinionCount;

		// 确保使用正确的 Reset 钩子。这个是独特的，因为它仍然会
		// 在游戏暂停时调用；这允许信息饰品继续正确更新。
		public override void ResetInfoAccessories() {
			showMinionCount = false;
		}

		// If we have another nearby player on our team, we want to get their info accessories working on us,
		// just like in vanilla. This is what this hook is for.
		public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer) {
			if (otherPlayer.GetModPlayer<ExampleInfoDisplayPlayer>().showMinionCount) {
				showMinionCount = true;
			}
		}
	}
}
