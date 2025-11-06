using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ExampleMod.Common.Configs.ModConfigShowcases
{
	// ModConfigShowcaseAcceptClientChanges showcases the AcceptClientChanges 方法.
	// In multiplayer all connected clients can attempt to change ServerSide configs.
	// 默认情况下 changes that don't require a 重新加载 将 accepted, but modders can use AcceptClientChanges to further 限制 this behavior.
	// 使用 the AcceptClientChanges 方法 to determine if the clients changes to this 配置 将 accepted and relayed to all other clients.
	public class ModConfigShowcaseAcceptClientChanges : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		public bool OnlyChangeableDuringNight;

		public int SomeNumber;

		// This LocalizedText 将 used for a rejection 消息. Since it is static it won't be part 的 配置 shown in-game.
		public static LocalizedText RejectChangesDaytime { get; private set; }

		public override void OnLoaded() {
			RejectChangesDaytime = this.GetLocalization(nameof(RejectChangesDaytime));
		}

		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message) {
			// If OnlyChangeableDuringNight has changed and it is day 时间, we 拒绝 the changes.
			// 这是一个 toy example. A real mod might have some logic that ReloadRequired wouldn't be suitable for.
			if (Main.dayTime && ((ModConfigShowcaseAcceptClientChanges)pendingConfig).OnlyChangeableDuringNight != OnlyChangeableDuringNight) {
				// The NetworkText 类 ensures that messages are shown to clients 在 客户端's selected language. The NetworkText.FromKey and LocalizedText.ToNetworkText methods 可以 用于 create a NetworkText 对象 from a 翻译 键.
				// The NetworkText.FromLiteral 方法 allows sending a 字符串 directly but is not recommended to use.
				message = RejectChangesDaytime.ToNetworkText();
				return false; // 返回 假 to 拒绝 the changes
			}

			// This code limits changes to only the local 主机 (the 主机 and play 客户端).
			if (!NetMessage.DoesPlayerSlotCountAsAHost(whoAmI)) {
				message = NetworkText.FromKey("tModLoader.ModConfigRejectChangesNotHost"); // "Only the 主机 can change this 配置"
				return false;
			}
			// Note: The local 主机 approach is a simple, but won't work for users hosting tModLoader on a dedicated 服务器.
			// 有 currently no tModLoader provided 认证 mechanism, but some mods implement their own systems to determine
			// if a remote 客户端 应该 treated as the 主机.
			// The Shorter 重生 mod, 例如, uses the 用户 permissions system of HEROs Mod (a mod 即 intended for
			// multiplayer administration) to determine if a particular 用户 has 权限 to change the 配置:
			// https://github.com/JavidPack/ShorterRespawn/blob/1.4/ShorterRespawnConfig.cs#L85

			// 接受 the changes. This is the default behavior 的 AcceptClientChanges 方法.
			return true;
		}
	}
}
