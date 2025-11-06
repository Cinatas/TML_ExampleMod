using ExampleMod.Content.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.Map;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalPylons
{
	/// <summary>
	/// 一个示例和展示 GlobalPylon 类中一些钩子的例子，这些钩子允许我们更改功能
	/// 适用于我们想要的任何类型的晶塔。我们在此类中所做的更改可能不是最实用的，因为它们更多地是
	/// 展示你可以用钩子做什么。
	/// </summary>.
	public class ExampleGlobalPylon : GlobalPylon
	{
		public override bool? ValidTeleportCheck_PreNPCCount(TeleportPylonInfo pylonInfo, ref int defaultNecessaryNPCCount) {
			// 由于我们有这种能力，我们可以允许玩家在夜间传送到任何晶塔，即使那里没有 NPC。
			if (!Main.dayTime) {
				defaultNecessaryNPCCount = 0;
			}

			// 由于我们没有阻止任何内容，只是更改 NPC 计数，我们可以返回默认方法返回的内容，即 空（又名原版行为）
			return base.ValidTeleportCheck_PreNPCCount(pylonInfo, ref defaultNecessaryNPCCount);
		}

		public override bool PreDrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, ref TeleportPylonInfo pylonInfo, ref bool isNearPylon, ref Color drawColor, ref float deselectedScale, ref float selectedScale) {
			// 如果我们想改变所有地图图标的颜色怎么办？
			// 如果我们不在晶塔附近，我们将把所有晶塔图标的颜色转换为更红
			if (!isNearPylon) {
				drawColor = Color.Lerp(drawColor, Color.Red, 0.75f);
			}

			// 由于我们实际上并没有阻止绘制任何地图图标，我们可以返回默认值，在这种情况下为 空（又名原版行为）
			return base.PreDrawMapIcon(ref context, ref mouseOverText, ref pylonInfo, ref isNearPylon, ref drawColor, ref deselectedScale, ref selectedScale);
		}

		public override bool? PreCanPlacePylon(int x, int y, int tileType, TeleportPylonType pylonType) {
			// 如果我们想覆盖晶塔放置的功能怎么办？
			// 例如，让我们始终允许玩家放置万能晶塔，即使它们已经存在于世界中：
			if (pylonType == TeleportPylonType.Victory) {
				return true;
			}
			// What if we wanted to change something for a modded 类型? If you have strong 引用 到 modded pylon in question,
			// 你可以 simply use the 类:
			if (pylonType == ModContent.PylonType<ExamplePylonTileAdvanced>()) {
				return null; //We don't 想要 *actually* change 任何 functionality 的 advanced pylon, so we 返回 空.
				//Obviously, if you wanted to actually change something about the modded pylon, you'd 返回 something other than 空 here.
			}

			return base.PreCanPlacePylon(x, y, tileType, pylonType);
		}

		public override bool? ValidTeleportCheck_PreBiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) {
			// What if we 想要 do something based 在 类型 of pylon in particular? Well all we 必须 do is check the pylon's 类型!
			// Let's 允许 the Jungle Pylon to work 在 snow, 例如:
			if (pylonInfo.TypeOfPylon == TeleportPylonType.Jungle) {
				// If another mod tries to mess with Jungle pylons, we don't 想要 返回 a forceful 假, 如果适用. If Jungle AND snow
				// are 两者 假, we will 返回 空 to 允许 for other mods to try and change things based 在 Jungle pylon.
				// Granted that no other mod does 任何thing to change the 空 值, the teleportation 过程 will fail, under the above circumstances.
				return sceneData.EnoughTilesForJungle || sceneData.EnoughTilesForSnow ? true : null;
			}

			return base.ValidTeleportCheck_PreBiomeRequirements(pylonInfo, sceneData);
		}

		public override void PostValidTeleportCheck(TeleportPylonInfo destinationPylonInfo, TeleportPylonInfo nearbyPylonInfo, ref bool destinationPylonValid, ref bool validNearbyPylonFound, ref string errorKey) {
			// Since there is not an explicit hook for it (since it's too specific), what if we wanted to nullify vanilla's check to 防止 accessing the Lihzahrd Temple early with a pylon?

			// We just 需要 check that to see if the Lihzahrd Temple check is the actual 错误 we got (not some other 错误) which in this case is done by checking the 错误 键.
			// We also do another quick check to make sure that we are still near a valid pylon.
			// If 即 真, we can set destinationPylonValid to 真, overriding the teleportation prevention.
			if (validNearbyPylonFound && errorKey == "Net.CannotTeleportToPylonBecauseAccessingLihzahrdTempleEarly") {
				destinationPylonValid = true;
			}
		}
	}
}
