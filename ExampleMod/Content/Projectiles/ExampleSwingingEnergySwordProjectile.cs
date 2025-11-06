using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	// 这是 a 复制 的 Excalibur's 弹幕
	public class ExampleSwingingEnergySwordProjectile : ModProjectile
	{

		// 我们 could use a vanilla 纹理 if we want instead of supplying our own.
		// public override 字符串 纹理 => "Terraria/Images/Projectile_" + ProjectileID.Excalibur;

		public override void SetStaticDefaults() {
			// 如果 a Jellyfish is zapping and we 攻击 it with this 弹幕, it will deal 伤害 to us.
			// This set has the projectiles 对于 Night's Edge, Excalibur, Terra Blade (关闭 范围), and The Horseman's Blade (关闭 范围).
			// This set does not have the 真 Night's Edge, 真 Excalibur, or the long 范围 Terra Beam projectiles.
			ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
			Main.projFrames[Type] = 4; // This 弹幕 has 4 frames.
		}

		public override void SetDefaults() {
			// 宽度 and 高度 don't really matter here because we have custom collision.
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = 3; // The 弹幕 can hit 3 enemies.
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.ownerHitCheck = true; // A line of sight check so the 弹幕 can't deal 伤害 through tiles.
			Projectile.ownerHitCheckDistance = 300f; // The 最大 范围 th在 弹幕 can hit a 目标. 300 pixels is 18.75 tiles.
			Projectile.usesOwnerMeleeHitCD = true; // This will make the 弹幕 apply the standard 数字 of immunity frames as normal melee attacks.
			// Normally, projectiles die after they have hit all the enemies they can.
			// But, for this case, we want the 弹幕 to 继续 to live so we can have the visuals 的 swing.
			Projectile.stopsDealingDamageAfterPenetrateHits = true;

			// 我们 将 using custom AI for this 弹幕. The original Excalibur uses aiStyle 190.
			Projectile.aiStyle = -1;
			// 弹幕.aiStyle = ProjAIStyleID.NightsEdge; // 190
			// AIType = ProjectileID.Excalibur;

			// 如果 you are using custom AI, add this line. Otherwise, visuals from Flasks will 生成 在 中心 的 弹幕 instead of around the arc.
			// 我们 will 生成 the visuals around the arc ourselves 在 AI().
			Projectile.noEnchantmentVisuals = true;
		}

		public override void AI() {
			// 在 our 项, we 生成 the 弹幕 与 方向, max 时间, and 缩放
			// 弹幕.ai[0] == 方向
			// 弹幕.ai[1] == max 时间
			// 弹幕.ai[2] == 缩放
			// 弹幕.localAI[0] == current 时间

			// Terra Blade makes an extra 声音 when spawning.
			// if (弹幕.localAI[0] == 0f) {
			// 	SoundEngine.PlaySound(SoundID.Item60 with { 音量 = 0.65f }, 弹幕.位置);
			// }

			Projectile.localAI[0]++; // Current 时间 th在 弹幕 has been alive.
			Player player = Main.player[Projectile.owner];
			float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1]; // The current 时间 over the max 时间.
			float direction = Projectile.ai[0];
			float velocityRotation = Projectile.velocity.ToRotation();
			float adjustedRotation = MathHelper.Pi * direction * percentageOfLife + velocityRotation + direction * MathHelper.Pi + player.fullRotation;
			Projectile.rotation = adjustedRotation; // 设置 the 旋转 to our 到 new 旋转 we calculated.

			float scaleMulti = 0.6f; // Excalibur, Terra Blade, and The Horseman's Blade is 0.6f; 真 Excalibur is 1f; default is 0.2f 
			float scaleAdder = 1f; // Excalibur, Terra Blade, and The Horseman's Blade is 1f; 真 Excalibur is 1.2f; default is 1f 

			Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;
			Projectile.scale = scaleAdder + percentageOfLife * scaleMulti;

			// other sword projectiles that use AI Style 190 have different effects.
			// 此示例 only includes the Excalibur.
			// Look at AI_190_NightsEdge() in 弹幕.cs 对于 others.

			// 在这里 we 生成 some dust inside the arc 的 swing.
			float dustRotation = Projectile.rotation + Main.rand.NextFloatDirection() * MathHelper.PiOver2 * 0.7f;
			Vector2 dustPosition = Projectile.Center + dustRotation.ToRotationVector2() * 84f * Projectile.scale;
			Vector2 dustVelocity = (dustRotation + Projectile.ai[0] * MathHelper.PiOver2).ToRotationVector2();
			if (Main.rand.NextFloat() * 2f < Projectile.Opacity) {
				// Original Excalibur 颜色: 颜色.金币, 颜色.White
				Color dustColor = Color.Lerp(Color.SkyBlue, Color.White, Main.rand.NextFloat() * 0.3f);
				Dust coloredDust = Dust.NewDustPerfect(Projectile.Center + dustRotation.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), DustID.FireworksRGB, dustVelocity * 1f, 100, dustColor, 0.4f);
				coloredDust.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
				coloredDust.noGravity = true;
			}

			if (Main.rand.NextFloat() * 1.5f < Projectile.Opacity) {
				// Original Excalibur 颜色: 颜色.White
				Dust.NewDustPerfect(dustPosition, DustID.TintableDustLighted, dustVelocity, 100, Color.SkyBlue * Projectile.Opacity, 1.2f * Projectile.Opacity);
			}

			Projectile.scale *= Projectile.ai[2]; // 设置 the 缩放 的 弹幕 到 缩放 的 项.

			// 如果 the 弹幕 is as old as the max 动画 时间, kill the 弹幕.
			if (Projectile.localAI[0] >= Projectile.ai[1]) {
				Projectile.Kill();
			}

			// This for 循环 spawns the visuals when using Flasks (武器 imbues)
			for (float i = -MathHelper.PiOver4; i <= MathHelper.PiOver4; i += MathHelper.PiOver2) {
				Rectangle rectangle = Utils.CenteredRectangle(Projectile.Center + (Projectile.rotation + i).ToRotationVector2() * 70f * Projectile.scale, new Vector2(60f * Projectile.scale, 60f * Projectile.scale));
				Projectile.EmitEnchantmentVisualsAt(rectangle.TopLeft(), rectangle.Width, rectangle.Height);
			}
		}

		// 在这里 is where we have our custom collision.
		// This collision will only run if the 弹幕 is within 范围 of 目标 与 范围 being 弹幕.ownerHitCheckDistance
		// Or if the 弹幕 hasn't already hit all 的 targets it can with 弹幕.penetrate
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			// 这是 how large the circumference is, aka how big the 范围 is. Vanilla uses 94f to 匹配 it 到 大小 的 纹理.
			float coneLength = 94f * Projectile.scale;
			// This 数字 affects how much the 开始 and 结束 的 collision 将 rotated.
			// Bigger Pi numbers will 旋转 the collision 计数器 clockwise.
			// Smaller Pi numbers will 旋转 the collision clockwise.
			// (弹幕.ai[0] is the 方向)
			float collisionRotation = MathHelper.Pi * 2f / 25f * Projectile.ai[0];
			float maximumAngle = MathHelper.PiOver4; // The maximumAngle is used to 限制 the 旋转 to create a dead 区域.
			float coneRotation = Projectile.rotation + collisionRotation;

			// Uncomment this line for a visual representation 的 cone. The dusts are not perfect, but it gives a general idea.
			// Dust.NewDustPerfect(弹幕.中心 + coneRotation.ToRotationVector2() * coneLength, DustID.Pixie, Vector2.Zero);
			// Dust.NewDustPerfect(弹幕.中心, DustID.BlueFairy, new Vector2((float)Math.Cos(maximumAngle) * 弹幕.ai[0], (float)Math.Sin(maximumAngle)) * 5f); // Assumes collisionRotation was not changed

			// 首先, we check to see if our first cone intersects the 目标.
			if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation, maximumAngle)) {
				return true;
			}

			// first cone isn't the entire swinging arc, though, so we need to check a second cone 对于 back 的 arc.
			float backOfTheSwing = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] * 0.3f, Projectile.ai[1] * 0.5f, 1f, 0f);
			if (backOfTheSwing > 0f) {
				float coneRotation2 = coneRotation - MathHelper.PiOver4 * Projectile.ai[0] * backOfTheSwing;

				// Uncomment this line for a visual representation 的 cone. The dusts are not perfect, but it gives a general idea.
				// Dust.NewDustPerfect(弹幕.中心 + coneRotation2.ToRotationVector2() * coneLength, DustID.Enchanted_Pink, Vector2.Zero);
				// Dust.NewDustPerfect(弹幕.中心, DustID.BlueFairy, new Vector2((float)Math.Cos(backOfTheSwing) * -弹幕.ai[0], (float)Math.Sin(backOfTheSwing)) * 5f); // Assumes collisionRotation was not changed

				if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation2, maximumAngle)) {
					return true;
				}
			}

			return false;
		}

		public override void CutTiles() {
			// 在这里 we calculate where the 弹幕 can destroy grass, pots, Queen Bee Larva, etc.
			Vector2 starting = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
			Vector2 ending = (Projectile.rotation + MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
			float width = 60f * Projectile.scale;
			Utils.PlotTileLine(Projectile.Center + starting, Projectile.Center + ending, width, DelegateMethods.CutTiles);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			// Vanilla has several particles that can easily be used anywhere.
			// particles 从 Particle Orchestra are predefined by vanilla and most can 不 customized that much.
			// 使用 auto complete to see the other ParticleOrchestraType types there are.
			// 在这里 we are spawning the Excalibur particle randomly inside 的 目标's hitbox.
			ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
				new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
				Projectile.owner);

			// 你 could also 生成 dusts 在 敌人 位置. Here is simple an example:
			// Dust.NewDust(Main.rand.NextVector2FromRectangle(目标.Hitbox), 0, 0, ModContent.DustType<Content.Dusts.Sparkle>());

			// 设置 the 目标's hit 方向 to away 从 玩家 so the knockback is 在 correct 方向.
			hit.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info) {
			ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
				new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
				Projectile.owner);

			info.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
		}

		// Taken from Main.DrawProj_Excalibur()
		// Look 在 source code 对于 other sword types.
		public override bool PreDraw(ref Color lightColor) {
			Vector2 position = Projectile.Center - Main.screenPosition;
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle sourceRectangle = texture.Frame(1, 4); // The sourceRectangle says which 帧 to use.
			Vector2 origin = sourceRectangle.Size() / 2f;
			float scale = Projectile.scale * 1.1f;
			SpriteEffects spriteEffects = ((!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None); // Flip the 精灵 based 在 方向 it is facing.
			float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1]; // The current 时间 over the max 时间.
			float lerpTime = Utils.Remap(percentageOfLife, 0f, 0.6f, 0f, 1f) * Utils.Remap(percentageOfLife, 0.6f, 1f, 1f, 0f);
			float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
			lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);

			Color backDarkColor = new Color(60, 160, 180); // Original Excalibur 颜色: 颜色(180, 160, 60)
			Color middleMediumColor = new Color(80, 255, 255); // Original Excalibur 颜色: 颜色(255, 255, 80)
			Color frontLightColor = new Color(150, 240, 255); // Original Excalibur 颜色: 颜色(255, 240, 150)

			Color whiteTimesLerpTime = Color.White * lerpTime * 0.5f;
			whiteTimesLerpTime.A = (byte)(whiteTimesLerpTime.A * (1f - lightingColor));
			Color faintLightingColor = whiteTimesLerpTime * lightingColor * 0.5f;
			faintLightingColor.G = (byte)(faintLightingColor.G * lightingColor);
			faintLightingColor.B = (byte)(faintLightingColor.R * (0.25f + lightingColor * 0.75f));

			// Back part
			Main.EntitySpriteDraw(texture, position, sourceRectangle, backDarkColor * lightingColor * lerpTime, Projectile.rotation + Projectile.ai[0] * MathHelper.PiOver4 * -1f * (1f - percentageOfLife), origin, scale, spriteEffects, 0f);
			// Very faint part affected by the light 颜色
			Main.EntitySpriteDraw(texture, position, sourceRectangle, faintLightingColor * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
			// Middle part
			Main.EntitySpriteDraw(texture, position, sourceRectangle, middleMediumColor * lightingColor * lerpTime * 0.3f, Projectile.rotation, origin, scale, spriteEffects, 0f);
			// Front part
			Main.EntitySpriteDraw(texture, position, sourceRectangle, frontLightColor * lightingColor * lerpTime * 0.5f, Projectile.rotation, origin, scale * 0.975f, spriteEffects, 0f);
			// Thin 顶部 line (final 帧)
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.White * 0.6f * lerpTime, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
			// Thin middle line (final 帧)
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.White * 0.5f * lerpTime, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, scale * 0.8f, spriteEffects, 0f);
			// Thin 底部 line (final 帧)
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.White * 0.4f * lerpTime, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, scale * 0.6f, spriteEffects, 0f);

			// This draws some sparkles around the circumference 的 swing.
			for (float i = 0f; i < 8f; i += 1f) {
				float edgeRotation = Projectile.rotation + Projectile.ai[0] * i * (MathHelper.Pi * -2f) * 0.025f + Utils.Remap(percentageOfLife, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0];
				Vector2 drawPos = position + edgeRotation.ToRotationVector2() * ((float)texture.Width * 0.5f - 6f) * scale;
				DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, new Color(255, 255, 255, 0) * lerpTime * (i / 9f), middleMediumColor, percentageOfLife, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(percentageOfLife, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
			}

			// This draws a large star sparkle 在 front 的 弹幕.
			Vector2 drawPos2 = position + (Projectile.rotation + Utils.Remap(percentageOfLife, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0]).ToRotationVector2() * ((float)texture.Width * 0.5f - 4f) * scale;
			DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos2, new Color(255, 255, 255, 0) * lerpTime * 0.5f, middleMediumColor, percentageOfLife, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(percentageOfLife, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale);

			// Uncomment this line for a visual representation 的 弹幕's 大小.
			// Main.EntitySpriteDraw(TextureAssets.MagicPixel.值, 位置, sourceRectangle, 颜色.Orange * 0.75f, 0f, 原点, 缩放, spriteEffects);

			return false;
		}

		// Copied from Main.DrawPrettyStarSparkle() 即 private
		private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawPos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness) {
			Texture2D sparkleTexture = TextureAssets.Extra[98].Value;
			Color bigColor = shineColor * opacity * 0.5f;
			bigColor.A = 0;
			Vector2 origin = sparkleTexture.Size() / 2f;
			Color smallColor = drawColor * 0.5f;
			float lerpValue = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
			Vector2 scaleLeftRight = new Vector2(fatness.X * 0.5f, scale.X) * lerpValue;
			Vector2 scaleUpDown = new Vector2(fatness.Y * 0.5f, scale.Y) * lerpValue;
			bigColor *= lerpValue;
			smallColor *= lerpValue;
			// Bright, large part
			Main.EntitySpriteDraw(sparkleTexture, drawPos, null, bigColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight, dir);
			Main.EntitySpriteDraw(sparkleTexture, drawPos, null, bigColor, 0f + rotation, origin, scaleUpDown, dir);
			// Dim, small part
			Main.EntitySpriteDraw(sparkleTexture, drawPos, null, smallColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight * 0.6f, dir);
			Main.EntitySpriteDraw(sparkleTexture, drawPos, null, smallColor, 0f + rotation, origin, scaleUpDown * 0.6f, dir);
		}
	}
}