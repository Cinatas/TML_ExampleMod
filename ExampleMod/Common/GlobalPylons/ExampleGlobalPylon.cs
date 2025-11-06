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

			// 由于我们没有阻止任何内容，只是更改 NPC 计数，我们可以返回默认方法返回的内容，即 null（又名原版行为）
			return base.ValidTeleportCheck_PreNPCCount(pylonInfo, ref defaultNecessaryNPCCount);
		}

		public override bool PreDrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, ref TeleportPylonInfo pylonInfo, ref bool isNearPylon, ref Color drawColor, ref float deselectedScale, ref float selectedScale) {
			// 如果我们想改变所有地图图标的颜色怎么办？
			// 如果我们不在晶塔附近，我们将把所有晶塔图标的颜色转换为更红
			if (!isNearPylon) {
				drawColor = Color.Lerp(drawColor, Color.Red, 0.75f);
			}

			// 由于我们实际上并没有阻止绘制任何地图图标，我们可以返回默认值，在这种情况下为 null（又名原版行为）
			return base.PreDrawMapIcon(ref context, ref mouseOverText, ref pylonInfo, ref isNearPylon, ref drawColor, ref deselectedScale, ref selectedScale);
		}

		public override bool? PreCanPlacePylon(int x, int y, int tileType, TeleportPylonType pylonType) {
			// 如果我们想覆盖晶塔放置的功能怎么办？
			// 例如，让我们始终允许玩家放置万能晶塔，即使它们已经存在于世界中：
			if (pylonType == TeleportPylonType.Victory) {
				return true;
			}
			// What if we wanted to change something for a modded type? If you have strong reference to the modded pylon in question,
			// you can simply use the class:
			if (pylonType == ModContent.PylonType<ExamplePylonTileAdvanced>()) {
				return null; //We don't want to *actually* change any functionality of the advanced pylon, so we return null.
				//Obviously, if you wanted to actually change something about the modded pylon, you'd return something other than null here.
			}

			return base.PreCanPlacePylon(x, y, tileType, pylonType);
		}

		public override bool? ValidTeleportCheck_PreBiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) {
			// What if we want to do something based on the type of pylon in particular? Well all we have to do is check the pylon's type!
			// Let's allow the Jungle Pylon to work in the snow, for example:
			if (pylonInfo.TypeOfPylon == TeleportPylonType.Jungle) {
				// If another mod tries to mess with Jungle pylons, we don't want to return a forceful false, if applicable. If Jungle AND snow
				// are both false, we will return null to allow for other mods to try and change things based on the Jungle pylon.
				// Granted that no other mod does anything to change the null value, the teleportation process will fail, under the above circumstances.
				return sceneData.EnoughTilesForJungle || sceneData.EnoughTilesForSnow ? true : null;
			}

			return base.ValidTeleportCheck_PreBiomeRequirements(pylonInfo, sceneData);
		}

		public override void PostValidTeleportCheck(TeleportPylonInfo destinationPylonInfo, TeleportPylonInfo nearbyPylonInfo, ref bool destinationPylonValid, ref bool validNearbyPylonFound, ref string errorKey) {
			// Since there is not an explicit hook for it (since it's too specific), what if we wanted to nullify vanilla's check to prevent accessing the Lihzahrd Temple early with a pylon?

			// We just need to check that to see if the Lihzahrd Temple check is the actual error we got (not some other error) which in this case is done by checking the error key.
			// We also do another quick check to make sure that we are still near a valid pylon.
			// If that is true, we can set destinationPylonValid to true, overriding the teleportation prevention.
			if (validNearbyPylonFound && errorKey == "Net.CannotTeleportToPylonBecauseAccessingLihzahrdTempleEarly") {
				destinationPylonValid = true;
			}
		}
	}
}
