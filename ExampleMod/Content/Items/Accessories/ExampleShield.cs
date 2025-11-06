using ExampleMod.Content.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Shield)] // 加载 the spritesheet you create as a shield 对于 玩家 when it is equipped.
	public class ExampleShield : ModItem
	{
		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(10);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;

			Item.defense = 1000;
			Item.lifeRegen = 10;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetDamage(DamageClass.Generic) += 1f; // Increase ALL 玩家 伤害 by 100%
			player.endurance = 1f - (0.1f * (1f - player.endurance));  // The 百分比 of 伤害 reduction
			player.GetModPlayer<ExampleDashPlayer>().DashAccessoryEquipped = true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<ExampleWorkbench>()
				.Register();
		}
	}

	public class ExampleDashPlayer : ModPlayer
	{
		// These indicate what 方向 is what 在 计时器 arrays used
		public const int DashDown = 0;
		public const int DashUp = 1;
		public const int DashRight = 2;
		public const int DashLeft = 3;

		public const int DashCooldown = 50; // 时间 (frames) between starting dashes. If this is shorter than DashDuration you can 开始 a new dash before an old one has finished
		public const int DashDuration = 35; // 持续时间 的 dash afterimage 效果 in frames

		// initial 速度.  10 速度 is about 37.5 tiles/second or 50 mph
		public const float DashVelocity = 10f;

		// directi在 玩家 has double tapped.  Defaults to -1 for no dash double tap
		public int DashDir = -1;

		// fields related 到 dash 饰品
		public bool DashAccessoryEquipped;
		public int DashDelay = 0; // frames remaining till we can dash again
		public int DashTimer = 0; // frames remaining 在 dash

		public override void ResetEffects() {
			// 重置 our equipped 标志. If the 饰品 is equipped somewhere, ExampleShield.UpdateAccessory 将 called and set the 标志 before PreUpdateMovement
			DashAccessoryEquipped = false;

			// 重置Effects is called not long after 玩家.doubleTapCardinalTimer's values have been set
			// 当 a directional 键 is pressed and released, vanilla starts a 15 tick (1/4 second) 计时器 during which a second press activates a dash
			// 如果 the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
			if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15) {
				DashDir = DashDown;
			}
			else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15) {
				DashDir = DashUp;
			}
			else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15 && Player.doubleTapCardinalTimer[DashLeft] == 0) {
				DashDir = DashRight;
			}
			else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15 && Player.doubleTapCardinalTimer[DashRight] == 0) {
				DashDir = DashLeft;
			}
			else {
				DashDir = -1;
			}
		}

		// 这是 the perfect place to apply dash movement, it's after the vanilla movement code, and before the 玩家's 位置 is modified 基于 速度.
		// 如果 they double tapped this 帧, they'll 移动 fast this 帧
		public override void PreUpdateMovement() {
			// if the 玩家 can use our dash, has double tapped in a 方向, and our dash isn't currently on cooldown
			if (CanUseDash() && DashDir != -1 && DashDelay == 0) {
				Vector2 newVelocity = Player.velocity;

				switch (DashDir) {
					// 仅 apply the dash 速度 if our current 速度 在 wanted 方向 is less than DashVelocity
					case DashUp when Player.velocity.Y > -DashVelocity:
					case DashDown when Player.velocity.Y < DashVelocity: {
							// Y-速度 is set here
							// 如果 the 方向 requested was DashUp, then we adjust the 速度 to make the dash appear "faster" 由于 gravity being immediately in 效果
							// This adjustment is roughly 1.3x the intended dash 速度
							float dashDirection = DashDir == DashDown ? 1 : -1.3f;
							newVelocity.Y = dashDirection * DashVelocity;
							break;
						}
					case DashLeft when Player.velocity.X > -DashVelocity:
					case DashRight when Player.velocity.X < DashVelocity: {
							// X-速度 is set here
							float dashDirection = DashDir == DashRight ? 1 : -1;
							newVelocity.X = dashDirection * DashVelocity;
							break;
						}
					default:
						return; // not moving fast enough, so don't 开始 our dash
				}

				// 开始 our dash
				DashDelay = DashCooldown;
				DashTimer = DashDuration;
				Player.velocity = newVelocity;

				// 在这里 you'd be 能够 set an 效果 that happens when the dash first activates
				// Some examples include:  the larger smoke 效果 从 Master Ninja Gear and Tabi
			}

			if (DashDelay > 0)
				DashDelay--;

			if (DashTimer > 0) { // dash is active
				// 这是 where we set the afterimage 效果.  You can 替换 these two lines with whatever you 想要 happen during the dash
				// Some examples include:  spawning dust where the 玩家 is, adding buffs, making the 玩家 immune, etc.
				// 在这里 we take advantage of "玩家.eocDash" and "玩家.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage 效果
				Player.eocDash = DashTimer;
				Player.armorEffectDrawShadowEOCShield = true;

				// 计数 down frames remaining
				DashTimer--;
			}
		}

		private bool CanUseDash() {
			return DashAccessoryEquipped
				&& Player.dashType == DashID.None // 玩家 doesn't have Tabi or EoCShield equipped (give priority to those dashes)
				&& !Player.setSolar // 玩家 isn't wearing solar 护甲
				&& !Player.mount.Active; // 玩家 isn't mounted, since dashes on a 坐骑 look weird
		}
	}
}
