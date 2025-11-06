using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	// Shortsword projectiles are handled in a special way with how they draw and 伤害 things
	// "hitbox" itself is closer 到 玩家, the 精灵 is centered on it
	// 然而 the interactions 与 世界 will occur 偏移 from this hitbox, closer 到 sword's 提示 (CutTiles, Colliding)
	// Values chosen mostly correspond to Iron Shortsword
	public class ExampleShortswordProjectile : ModProjectile
	{
		public const int FadeInDuration = 7;
		public const int FadeOutDuration = 4;

		public const int TotalDuration = 16;

		// "宽度" 的 blade
		public float CollisionWidth => 10f * Projectile.scale;

		public int Timer {
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void SetDefaults() {
			Projectile.Size = new Vector2(18); // This sets 宽度 and 高度 到 same 值 (important when projectiles can 旋转)
			Projectile.aiStyle = -1; // 使用 our own AI to customize how it behaves, if you don't want that, keep this at ProjAIStyleID.ShortSword. You would still 需要 use the code in SetVisualOffsets() though
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.scale = 1f;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.ownerHitCheck = true; // 防止s hits through tiles. Most melee weapons that use projectiles have this
			Projectile.extraUpdates = 1; // 更新 1+extraUpdates times per tick
			Projectile.timeLeft = 360; // This 值 does not matter since we manually kill it earlier, it just has to be higher than the 持续时间 we use in AI
			Projectile.hide = true; // 重要 when used alongside 玩家.heldProj. "Hidden" projectiles have special draw conditions
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];

			Timer += 1;
			if (Timer >= TotalDuration) {
				// Kill the 弹幕 if it reaches it's intended lifetime
				Projectile.Kill();
				return;
			}
			else {
				// 重要 so th在 精灵 draws "in" the 玩家's hand and not fully in front or behind the 玩家
				player.heldProj = Projectile.whoAmI;
			}

			// Fade in and out
			// 获取LerpValue returns a 值 between 0f and 1f - if clamped is 真 - representing how far 计时器 got along the "距离" defined by the first two parameters
			// first call handles the fade in, the second one the fade out.
			// 注意 the second call's parameters are swapped, this means the result 将 reverted
			Projectile.Opacity = Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true) * Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true);

			// Keep locked on到 玩家, but extend further based 在 given 速度 (Requires ShouldUpdatePosition returning 假 to work)
			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
			Projectile.Center = playerCenter + Projectile.velocity * (Timer - 1f);

			// 设置 spriteDirection 基于 moving 左 or 右. 左 -1, 右 1
			Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();

			// 点 towards where it is moving, applied 偏移 for 顶部 右 的 精灵 respecting spriteDirection
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;

			// code in this 方法 is important to align the 精灵 与 hitbox how we want it to
			SetVisualOffsets();
		}

		private void SetVisualOffsets() {
			// 32 is the 精灵 大小 (here 两者 宽度 and 高度 equal)
			const int HalfSpriteWidth = 32 / 2;
			const int HalfSpriteHeight = 32 / 2;

			int HalfProjWidth = Projectile.width / 2;
			int HalfProjHeight = Projectile.height / 2;

			// Vanilla configuration for "hitbox in middle of 精灵"
			DrawOriginOffsetX = 0;
			DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
			DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);

			// Vanilla configuration for "hitbox towards the 结束"
			//if (弹幕.spriteDirection == 1) {
			//	DrawOriginOffsetX = -(HalfProjWidth - HalfSpriteWidth);
			//	DrawOffsetX = (int)-DrawOriginOffsetX * 2;
			//	DrawOriginOffsetY = 0;
			//}
			//else {
			//	DrawOriginOffsetX = (HalfProjWidth - HalfSpriteWidth);
			//	DrawOffsetX = 0;
			//	DrawOriginOffsetY = 0;
			//}
		}

		public override bool ShouldUpdatePosition() {
			// 更新 弹幕.中心 manually
			return false;
		}

		public override void CutTiles() {
			// "cutting tiles" refers to breaking pots, grass, queen bee larva, etc.
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Vector2 start = Projectile.Center;
			Vector2 end = start + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 10f;
			Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			// "Hit 任何thing between the 玩家 and the 提示 的 sword"
			// shootSpeed is 2.1f for 引用, so this is basically plotting 12 pixels ahead 从 中心
			Vector2 start = Projectile.Center;
			Vector2 end = start + Projectile.velocity * 6f;
			float collisionPoint = 0f; // 不要 need that 变量, but required as 参数
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
		}
	}
}
