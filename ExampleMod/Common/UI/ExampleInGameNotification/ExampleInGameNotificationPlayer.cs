using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ExampleMod.Common.UI.ExampleInGameNotification
{
	// 此类用于在游戏中推送我们的示例通知。
	// 参见 ExampleJoinWorldInGameNotification。
	public class ExampleInGameNotificationPlayer : ModPlayer
	{
		public override void OnEnterWorld()
		{
			// 当我们加入世界时显示我们的加入通知。
			// 这应该只为我们的玩家显示。
			if (Player.whoAmI == Main.myPlayer)
				InGameNotificationsTracker.AddNotification(new ExampleJoinWorldInGameNotification());
		}
	}
}