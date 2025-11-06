using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	// 此示例 attempts to showcase most 的 common boot 饰品 effects.
	// Of particular note is a showcase 的 correct approaches to various movement 速度 modifications.
	[AutoloadEquip(EquipType.Shoes)]
	public class ExampleBoots : ModItem
	{
		public static readonly int MoveSpeedBonus = 8;
		public static readonly int LavaImmunityTime = 2;

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBonus, LavaImmunityTime);

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 22;

			Item.accessory = true;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.buyPrice(gold: 1); // Equivalent to 项.buyPrice(0, 1, 0, 0);
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			// These 2 stat changes are equal 到 Lightning Boots
			player.moveSpeed += MoveSpeedBonus / 100f; // 修改 the 玩家 movement 速度 奖励.
			player.accRunSpeed = 6.75f; // 设置s the players sprint 速度 in boots.

			// 玩家.maxRunSpeed and 玩家.runAcceleration are usually not set by boots and should 不 changed in UpdateAccessory due 到 logic 顺序. See ExampleStatBonusAccessoryPlayer.PostUpdateRunSpeeds for an example of adjusting those 速度 stats.

			// 确定s whether the boots 计数 as 火箭 boots
			// 0 - These are not 火箭 boots
			// Anything else - These are 火箭 boots
			player.rocketBoots = 2;

			// 设置s which dust and 声音 to use 对于 火箭 flight
			// 1 - 火箭 Boots
			// 2 - Fairy Boots, Spectre Boots, Lightning Boots
			// 3 - Frostspark Boots
			// 4 - Terrraspark Boots
			// 5 - Hellfire Treads
			player.vanityRocketBoots = 2;

			player.waterWalk2 = true; // 允许s walking on all liquids without falling into it
			player.waterWalk = true; // 允许s walking on water, honey, and shimmer without falling into it
			player.iceSkate = true; // Grant the 玩家 improved 速度 on ice and not breaking thin ice when falling onto it
			player.desertBoots = true; // Grants the 玩家 increased movement 速度 while running on sand
			player.fireWalk = true; // Grants the 玩家 immunity from Meteorite and Hellstone 图格 伤害
			player.noFallDmg = true; // Grants the 玩家 the Lucky Horseshoe 效果 of nullifying fall 伤害
			player.lavaRose = true; // Grants the Lava Rose 效果
			player.lavaMax += LavaImmunityTime * 60; // Grants the 玩家 2 additional seconds of lava immunity

			// 玩家.DoBootsEffect(玩家.DoBootsEffect_PlaceFlowersOnTile); // 生成s flowers when walking on normal or Hallowed grass

			// These effects are visual only. These are replicated in UpdateVanity below so they apply for vanity equipment.
			if (!hideVisual) {
				player.CancelAllBootRunVisualEffects(); // This ensures that boot visual effects don't overlap if multiple are equipped

				// Hellfire Treads sprint dust. F或更多 info on sprint dusts see 玩家.SpawnFastRunParticles() 方法 in 玩家.cs
				player.hellfireTreads = true;
				// Other boot run visual effects include: sailDash, coldDash, desertDash, fairyBoots

				if (!player.mount.Active || player.mount.Type != MountID.WallOfFleshGoat) {
					// 生成s flames when walking, like Flame Waker Boots. We also check the Goat Skull 坐骑 so the effects don't overlap.
					player.DoBootsEffect(player.DoBootsEffect_PlaceFlamesOnTile);
				}
			}
		}

		public override void UpdateVanity(Player player) {
			// This code is a 复制 的 visual effects code in UpdateAccessory above
			player.CancelAllBootRunVisualEffects();
			player.vanityRocketBoots = 2;
			player.hellfireTreads = true;
			if (!player.mount.Active || player.mount.Type != MountID.WallOfFleshGoat) {
				player.DoBootsEffect(player.DoBootsEffect_PlaceFlamesOnTile);
			}
		}
	}
}