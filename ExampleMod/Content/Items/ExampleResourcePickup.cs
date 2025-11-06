using ExampleMod.Common.Players;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	// 此类 showcases a "pickup". Also known as a power-up.
	// Pickup refers to items that don't enter then 库存 when picked up, but rather have some other 效果 when obtained.
	// Pickups usually provide resources 到 玩家, 例如 hearts providing life or stars providing 魔力. Nebula 护甲 boosters are another example.
	// 此示例 drops from enemies when Example 资源 is low, 类似于 how hearts and stars only 放下 if the 玩家 is lacking 生命值 or 魔力.
	// 参见 ExampleResourcePickupGlobalNPC 对于 项 放下 code.
	public class ExampleResourcePickup : ModItem {
		public static readonly int ExampleResourceHealAmount = 50;

		public override LocalizedText Tooltip => LocalizedText.Empty;

		public override void SetStaticDefaults() {
			ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
			ItemID.Sets.IgnoresEncumberingStone[Type] = true;
			ItemID.Sets.IsAPickup[Type] = true;
			ItemID.Sets.ItemSpawnDecaySpeed[Type] = 4;
		}

		public override void SetDefaults() {
			Item.height = 12;
			Item.width = 12;
		}

		public override bool OnPickup(Player player) {
			// 当 the 项 is picked up, heal the 玩家's ExampleResource stat and 生成 and 同步 the corresponding CombatText
			player.GetModPlayer<ExampleResourcePlayer>().HealExampleResource(ExampleResourceHealAmount);

			// 我们 需要 play this ourselves since we are returning 假 meaning it won't play automatically.
			SoundEngine.PlaySound(SoundID.Grab, player.Center);

			// 我们 返回 假 to 防止 the 项 from going in到 players 库存.
			return false;
		}

		// Since ItemID.Sets.IsAPickup is 真, we don't 需要 override the ItemSpace hook to 允许 picking up the 项 when 库存 is full

		// 我们 can override CanPickup to 防止 attempting to pick up this 项 when at max ExampleResource, but hearts and stars do not do this so we won't 任一.

		// GrabRange 可以 用于 implement effects 类似于 Heartreach 药水 or Celestial Magnet.
		public override void GrabRange(Player player, ref int grabRange) {
			if (player.GetModPlayer<ExampleResourcePlayer>().exampleResourceMagnet) {
				grabRange += ExampleResourcePlayer.exampleResourceMagnetGrabRange;
			}
		}
	}
}
