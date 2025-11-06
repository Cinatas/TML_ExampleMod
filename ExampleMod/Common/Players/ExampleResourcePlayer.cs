using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	public class ExampleResourcePlayer : ModPlayer
	{
		// 在这里我们创建一个自定义资源，类似于魔力或生命值。
		// 创建一些变量来定义我们的示例资源的当前值以及当前最大值。我们还包括一个临时最大值，以及一些变量来处理此资源的自然再生。
		public int exampleResourceCurrent; // Current 值 of our example 资源
		public const int DefaultExampleResourceMax = 100; // 默认 最大 值 of example 资源
		public int exampleResourceMax; // 缓冲区 变量 即 用于 重置 最大 资源 to default 值 in ResetDefaults().
		public int exampleResourceMax2; // 最大 amount of our example 资源. We will change that 变量 to increase 最大 amount of our 资源
		public float exampleResourceRegenRate; // By changing that 变量 we can increase/decrease regeneration rate of our 资源
		internal int exampleResourceRegenTimer = 0; // A 变量 即 required for our 计时器
		public bool exampleResourceMagnet = false;
		public static readonly int exampleResourceMagnetGrabRange = 300;
		public static readonly Color HealExampleResourceColor = new(187, 91, 201); // The 颜色 to use with CombatText when replenishing exampleResourceCurrent

		// 为了使示例资源示例简单明了，已省略了类似于魔力和生命值的完全功能资源所需的几件事。 
		// Here are additional things you might 需要 implement if you intend to make a custom 资源:
		// - Multiplayer Syncing: The current example doesn't require MP code, but pretty much 任何 additional functionality will require this. ModPlayer.SendClientChanges and CopyClientState 将 necessary, 以及 as SyncPlayer if you 允许 the 用户 to increase exampleResourceMax.
		// - 保存/加载 permanent changes to max 资源: You'll 需要 implement 保存/加载 to remember increases to your exampleResourceMax cap.

		public override void Initialize() {
			exampleResourceMax = DefaultExampleResourceMax;
		}

		public override void ResetEffects() {
			ResetVariables();
		}

		public override void UpdateDead() {
			ResetVariables();
		}

		// 我们需要 this to ensure that regeneration rate and 最大 amount are 重置 to default values after increasing when conditions are 不再 satisfied (e.g. we unequip an 饰品 that increases our 资源)
		private void ResetVariables() {
			exampleResourceRegenRate = 1f;
			exampleResourceMax2 = exampleResourceMax;
			exampleResourceMagnet = false;
		}

		public override void PostUpdateMiscEffects() {
			UpdateResource();
		}

		public override void PostUpdate() {
			CapResourceGodMode();
		}

		// Lets do all our logic 对于 custom 资源 here, 例如 limiting it, increasing it and so on.
		private void UpdateResource() {
			// For our 资源 lets make it regen slowly over 时间 to keep it simple, let's use exampleResourceRegenTimer to 计数 up to whatever 值 we want, then increase currentResource.
			exampleResourceRegenTimer++; // Increase it by 60 per second, or 1 per tick.

			// A simple 计时器 that goes up to 1 second, increases the exampleResourceCurrent by 1 然后 resets back to 0.
			if (exampleResourceRegenTimer > 60 / exampleResourceRegenRate) {
				exampleResourceCurrent += 1;
				exampleResourceRegenTimer = 0;
			}

			// 限制 exampleResourceCurrent from going over the 限制 imposed by exampleResourceMax.
			exampleResourceCurrent = Utils.Clamp(exampleResourceCurrent, 0, exampleResourceMax2);
		}

		private void CapResourceGodMode() {
			if (Main.myPlayer == Player.whoAmI && Player.creativeGodMode) {
				exampleResourceCurrent = exampleResourceMax2;
			}
		}

		// HealExampleResource will increase the actual ExampleResource stat, then HealExampleResourceEffect spawns a CombatText visual and SendExampleResourceEffectMessage/HandleExampleResourceEffectMessage 处理 syncing that visual to other players.
		public void HealExampleResource(int healAmount) {
			exampleResourceCurrent = Math.Clamp(exampleResourceCurrent + healAmount, 0, exampleResourceMax2);
			if (Main.myPlayer == Player.whoAmI) {
				HealExampleResourceEffect(healAmount);
			}
		}

		// Responsible for spawning and syncing just the CombatText
		public void HealExampleResourceEffect(int healAmount) {
			CombatText.NewText(Player.getRect(), HealExampleResourceColor, healAmount);
			if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer) {
				SendExampleResourceEffectMessage(Player.whoAmI, healAmount);
			}
		}

		// These methods 处理 syncing the CombatText that indicates th在 玩家 has healed some amount of ExampleResource
		public static void HandleExampleResourceEffectMessage(BinaryReader reader, int whoAmI) {
			int player = reader.ReadByte();
			if (Main.netMode == NetmodeID.Server) {
				player = whoAmI;
			}

			int healAmount = reader.ReadInt32();
			if (player != Main.myPlayer) {
				Main.player[player].GetModPlayer<ExampleResourcePlayer>().HealExampleResourceEffect(healAmount);
			}

			if (Main.netMode == NetmodeID.Server) {
				// If the 服务器 receives this 消息, it sends it to all other clients to 同步 the effects.
				SendExampleResourceEffectMessage(player, healAmount);
			}
		}

		public static void SendExampleResourceEffectMessage(int whoAmI, int healAmount) {
			// This code is called by 两者 the initial 
			ModPacket packet = ModContent.GetInstance<ExampleMod>().GetPacket();
			packet.Write((byte)ExampleMod.MessageType.ExampleResourceEffect);
			packet.Write((byte)whoAmI);
			packet.Write(healAmount);
			packet.Send(ignoreClient: whoAmI);
		}
	}
}
