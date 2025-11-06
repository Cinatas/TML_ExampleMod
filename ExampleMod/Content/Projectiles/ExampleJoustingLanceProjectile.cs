using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleJoustingLanceProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			// 这将 cause the 玩家 to dismount if they are hit by another Jousting Lance.
			// Since no enemies use Jousting Lances, this will only cause the 玩家 to dismount in PVP.
			ProjectileID.Sets.DismountsPlayersOnHit[Type] = true;

			// 这将 确保 the 速度 的 弹幕 will always be the shoot 速度 set 在 项.
			// Since the 速度 的 弹幕 affects how far out the jousting lance will 生成, we want the
			// 速度 to always be the same 即使 the 玩家 has increased 攻击 速度.
			ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.netImportant = true; // 同步 this 弹幕 if a 玩家 joins mid game.

			// 宽度 and 高度 do not affect the collision 的 Jousting Lance because we calculate that separately (see Colliding() below)
			Projectile.width = 25;
			Projectile.height = 25;

			// aiStyle 19 is the AI for Spears. Jousting Lances use the Spear AI. If you set the aiStyle to 19, 确保 to set the AIType so it actually behaves like a Jousting Lance.
			// Since we are using custom AI below, we set the aiStyle to -1.
			Projectile.aiStyle = -1;

			Projectile.alpha = 255; // The transparency 的 弹幕, 255 for completely transparent. Our 弹幕 will fade in (see the AI() below).
			Projectile.friendly = true; // 玩家 shot 弹幕. Does 伤害 to enemies but not to friendly Town NPCs.
			Projectile.penetrate = -1; // Infinite penetration. The 弹幕 can hit an infinite 数字 of enemies.
			Projectile.tileCollide = false; // 不要 kill the 弹幕 if it hits a 图格.
			Projectile.scale = 1f; // The 缩放 的 弹幕. This only effects the drawing and the 宽度 的 collision.
			Projectile.hide = true; // We are drawing the 弹幕 ourselves. See PreDraw() below.
			Projectile.ownerHitCheck = true; // 使 sure the 所有者 的 弹幕 has line of sight 到 目标 (aka can't hit things through 图格).
			Projectile.DamageType = DamageClass.MeleeNoSpeed; // 设置 the 伤害 to melee 伤害.

			// Act like the normal Jousting Lance. Use this if you set the aiStyle to 19.
			// AIType = ProjectileID.JoustingLance; 
		}

		// 这是 the behavior 的 Jousting Lances.
		public override void AI() {
			Player owner = Main.player[Projectile.owner]; // 获取 the 所有者 的 弹幕.
			Projectile.direction = owner.direction; // 方向 将 -1 when facing 左 and +1 when facing 右. 
			owner.heldProj = Projectile.whoAmI; // 设置 the 所有者's held 弹幕 to this 弹幕. heldProj is used so th在 弹幕 将 killed when the 玩家 drops or swap items.

			int itemAnimationMax = owner.itemAnimationMax;
			// 记住, frames 计数 down from itemAnimationMax to 0
			// 帧 at which the lance is fully extended. Hold at this 帧 before retracting.
			// 缩放 factor (0.34f) means the last 34% 的 动画 将 used for retracting.
			int holdOutFrame = (int)(itemAnimationMax * 0.34f);
			if (owner.channel && owner.itemAnimation < holdOutFrame) {
				owner.SetDummyItemTime(holdOutFrame); // 这使 it so the 弹幕 never dies while we are holding it (except when we take 伤害, see ExampleJoustingLancePlayer).
			}

			// 如果 the Jousting Lance is 不再 being used, kill the 弹幕.
			if (owner.ItemAnimationEndingOrEnded) {
				Projectile.Kill();
				return;
			}

			int itemAnimation = owner.itemAnimation;
			// 扩展名 and retraction factors (0-1). As the 动画 plays out, 扩展名 goes from 0-1 and stays at 1 while holding, then retraction goes from 0-1.
			float extension = 1 - Math.Max(itemAnimation - holdOutFrame, 0) / (float)(itemAnimationMax - holdOutFrame);
			float retraction = 1 - Math.Min(itemAnimation, holdOutFrame) / (float)holdOutFrame;

			// Distances are in pixels
			float extendDist = 24; // How far to fly out during 扩展名
			float retractDist = extendDist / 2; // How far to fly back during retraction
			float tipDist = 98 + extension * extendDist - retraction * retractDist; // If your Jousting Lance is larger or smaller than the standard 大小, it is recommended to change the shoot 速度 的 项 代替 this 值.

			Vector2 center = owner.RotatedRelativePoint(owner.MountedCenter); // 获取 the 中心 的 所有者. This accounts 对于 玩家 being shifted up or down while riding a 坐骑, sitting in a chair, etc.
			Projectile.Center = center; // 设置 the 中心 的 弹幕 到 中心 的 所有者. 弹幕.中心 is now actually the 提示 的 Jousting Lance.
			Projectile.position += Projectile.velocity * tipDist; // The 弹幕 速度 contains the orientation 的 lance, multiply it by the tipDist to positi在 提示.

			// 设置 the 旋转 的 弹幕.
			// 对于 引用, 0 is the 顶部 左, 180 degrees or pi radians is the 底部 右.
			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + (float)Math.PI * 3 / 4f;

			// Fade the 弹幕 in when it first spawns
			Projectile.alpha -= 40;
			if (Projectile.alpha < 0) {
				Projectile.alpha = 0;
			}

			// Hallowed and Shadow Jousting Lance 生成 dusts when the 玩家 is moving above a certain 速度.
			float minimumDustVelocity = 6f;

			// This Vector2.Dot is the dot product between the 弹幕's 速度 and the 玩家's 速度 normalized to be between -1 and 1.
			// What 这意味着 in this context is th在 速度 值 将 closer to positive 1 if the 玩家 is moving 在 same 方向 as the directi在 lance was shot.
			// 示例： if the lance is shot up and 到 右, the 值 here 将 closer to 1 if the 玩家 is also moving up and 到 右.
			float movementInLanceDirection = Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.UnitX * owner.direction), owner.velocity.SafeNormalize(Vector2.UnitX * owner.direction));

			float playerVelocity = owner.velocity.Length();

			if (playerVelocity > minimumDustVelocity && movementInLanceDirection > 0.8f) {
				// 概率 对于 dust to 生成. The actual 概率 (see below) is 1/dustChance. We make the 概率 higher the faster the 玩家 is moving by making the denominator smaller.
				int dustChance = 8;
				if (playerVelocity > minimumDustVelocity + 1f) {
					dustChance = 5;
				}
				if (playerVelocity > minimumDustVelocity + 2f) {
					dustChance = 2;
				}

				// 设置 your dust types here.
				int dustTypeCommon = ModContent.DustType<Dusts.Sparkle>();
				int dustTypeRare = DustID.WhiteTorch;

				int offset = 4; // This 偏移 will affect how much the dust spreads out.

				// 生成 the dusts based 在 dustChance. The dusts are spawned 在 提示 的 Jousting Lance.
				if (Main.rand.NextBool(dustChance)) {
					int newDust = Dust.NewDust(Projectile.Center - new Vector2(offset, offset), offset * 2, offset * 2, dustTypeCommon, Projectile.velocity.X * 0.2f + (Projectile.direction * 3), Projectile.velocity.Y * 0.2f, 100, default, 1.2f);
					Main.dust[newDust].noGravity = true;
					Main.dust[newDust].velocity *= 0.25f;
					newDust = Dust.NewDust(Projectile.Center - new Vector2(offset, offset), offset * 2, offset * 2, dustTypeCommon, 0f, 0f, 150, default, 1.4f);
					Main.dust[newDust].velocity *= 0.25f;
				}

				if (Main.rand.NextBool(dustChance + 3)) {
					Dust.NewDust(Projectile.Center - new Vector2(offset, offset), offset * 2, offset * 2, dustTypeRare, 0f, 0f, 150, default, 1.4f);
				}
			}
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
			// 这将 increase or decrease the knockback 的 Jousting Lance 取决于 how fast the 玩家 is moving.
			modifiers.Knockback *= Main.player[Projectile.owner].velocity.Length() / 7f;

			// 这将 increase or decrease the 伤害 的 Jousting Lance 取决于 how fast the 玩家 is moving.
			modifiers.SourceDamage *= 0.1f + Main.player[Projectile.owner].velocity.Length() / 7f * 0.9f;
		}

		// 这是 the custom collision that Jousting Lances uses. 
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			float rotationFactor = Projectile.rotation + (float)Math.PI / 4f; // The 旋转 的 Jousting Lance.
			float scaleFactor = 95f; // How far back the hit-line 将 从 提示 的 Jousting Lance. You will 需要 modify this if you have a longer or shorter Jousting Lance. Vanilla uses 95f
			float widthMultiplier = 23f; // How thick the hit-line is. Increase or decrease this 值 if your Jousting Lance is thicker or thinner. Vanilla uses 23f
			float collisionPoint = 0f; // collisionPoint is needed for CheckAABBvLineCollision(), but it isn't used for our collision here. Keep it at 0f.

			// This Rectangle is the 宽度 and 高度 的 Jousting Lance's hitbox 即 used 对于 first 步骤 of collision.
			// 你 will 需要 modify the last two numbers if you have a bigger or smaller Jousting Lance.
			// Vanilla uses (0, 0, 300, 300) which 即 quite large 对于 大小 的 Jousting Lance.
			// 大小 doesn't matter too much because this rectangle is only a basic check 对于 collision (the hit-line is much more important).
			Rectangle lanceHitboxBounds = new Rectangle(0, 0, 300, 300);

			// 设置 the 位置 的 large rectangle.
			lanceHitboxBounds.X = (int)Projectile.position.X - lanceHitboxBounds.Width / 2;
			lanceHitboxBounds.Y = (int)Projectile.position.Y - lanceHitboxBounds.Height / 2;

			// 这是 the back 的 hit-line with 弹幕.中心 being the 提示 的 Jousting Lance.
			Vector2 hitLineEnd = Projectile.Center + rotationFactor.ToRotationVector2() * scaleFactor;

			// following is for debugging the 大小 的 hit line. This will 允许 you to easily see where it starts and ends.
			// Dust.NewDustPerfect(弹幕.中心, DustID.Pixie, 速度: Vector2.Zero, 缩放: 0.5f);
			// Dust.NewDustPerfect(hitLineEnd, DustID.Pixie, 速度: Vector2.Zero, 缩放: 0.5f);

			// 首先 check that our large rectangle intersects 与 目标 hitbox.
			// Then we check to see if a line 从 提示 的 Jousting Lance 到 "结束" 的 lance intersects 与 目标 hitbox.
			if (lanceHitboxBounds.Intersects(targetHitbox)
				&& Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, hitLineEnd, widthMultiplier * Projectile.scale, ref collisionPoint)) {
				return true;
			}
			return false;
		}

		// 我们 需要 draw the 弹幕 manually. If you don't include this, the Jousting Lance will 不 aligned 与 玩家.
		public override bool PreDraw(ref Color lightColor) {

			// SpriteEffects change which directi在 精灵 is drawn.
			SpriteEffects spriteEffects = SpriteEffects.None;

			// 获取 纹理 of 弹幕.
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			// 获取 the currently selected 帧 在 纹理.
			Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);

			// 原点 在这种情况下 is (0, 0) of our 弹幕 because 弹幕.中心 is the 提示 of our Jousting Lance.
			Vector2 origin = Vector2.Zero;

			// 旋转 的 弹幕.
			float rotation = Projectile.rotation;

			// 如果 the 弹幕 is facing 右, we 需要 旋转 it by -90 degrees, 移动 the 原点, and flip the 精灵 horizontally.
			// 这将 make it so the 底部 的 精灵 is correctly facing down when shot 到 右.
			if (Projectile.direction > 0) {
				rotation -= (float)Math.PI / 2f;
				origin.X += sourceRectangle.Width;
				spriteEffects = SpriteEffects.FlipHorizontally;
			}

			// 位置 的 精灵. Not subtracting Main.玩家[弹幕.所有者].gfxOffY will cause the 精灵 to bounce when walking up blocks.
			Vector2 position = new(Projectile.Center.X, Projectile.Center.Y - Main.player[Projectile.owner].gfxOffY);

			// 应用 lighting and draw our 弹幕
			Color drawColor = Projectile.GetAlpha(lightColor);

			Main.EntitySpriteDraw(texture,
				position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, rotation, origin, Projectile.scale, spriteEffects, 0);

			// following is for debugging the 大小 的 collision rectangle. Set this 到 same 大小 as the one you have in Colliding().
			// Rectangle lanceHitboxBounds = new Rectangle(0, 0, 300, 300);
			// Main.EntitySpriteDraw(TextureAssets.MagicPixel.值,
			// 	new Vector2((int)弹幕.中心.X - lanceHitboxBounds.宽度 / 2, (int)弹幕.中心.Y - lanceHitboxBounds.高度 / 2) - Main.screenPosition,
			// 	lanceHitboxBounds, 颜色.Orange * 0.5f, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

			// It's important to 返回 假, 否则 we also draw the original 纹理.
			return false;
		}
	}
}