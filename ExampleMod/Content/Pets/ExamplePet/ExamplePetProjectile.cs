using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Pets.ExamplePet
{
	public class ExamplePetProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			Main.projFrames[Projectile.type] = 4;
			Main.projPet[Projectile.type] = true;

			// This code is needed to customize the vanity 宠物 显示 在 玩家 select 屏幕. Quick explanation:
			// * It uses fluent API syntax, just like 配方
			// * You 开始 with ProjectileID.Sets.SimpleLoop, specifying the 开始 and 结束 frames 以及 as the 速度, and optionally if it should animate 从 结束 after reaching the 结束, effectively "bouncing"
			// * To 停止 the 动画 if the 玩家 is not highlighted/is standing, as done by most grounded pets, add a .WhenNotSelected(0, 0) (you can customize it just like SimpleLoop)
			// * To set 偏移 and 方向, use .WithOffset(x, y) and .WithSpriteDirection(-1)
			// * To further customize the behavior and 动画 的 宠物 (as its AI does not run), you have access to 一些 vanilla presets in DelegateMethods.CharacterPreview to use via .WithCode(). You can also make your own, showcased in MinionBossPetProjectile
			ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Projectile.type], 6)
				.WithOffset(-10, -20f)
				.WithSpriteDirection(-1)
				.WithCode(DelegateMethods.CharacterPreview.Float);
		}

		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.ZephyrFish); // 复制 the stats 的 Zephyr Fish

			AIType = ProjectileID.ZephyrFish; // Mimic as the Zephyr Fish during AI.
		}

		public override bool PreAI() {
			Player player = Main.player[Projectile.owner];

			player.zephyrfish = false; // Relic from AIType

			return true;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];

			// Keep the 弹幕 from disappearing 只要 the 玩家 isn't dead and has the 宠物 增益.
			if (!player.dead && player.HasBuff(ModContent.BuffType<ExamplePetBuff>())) {
				Projectile.timeLeft = 2;
			}
		}
	}
}
