using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleLastPrismBeam : ModProjectile
	{
		// 一个 helpful math constant for performing beam angling calculations.
		private const float PiBeamDivisor = MathHelper.Pi / ExampleLastPrismHoldout.NumBeams;

		// How much more 伤害 the beams do when the Prism is fully charged. 伤害 smoothly scales up to this 乘数.
		private const float MaxDamageMultiplier = 1.5f;

		// Beams increase their 缩放 from 0 to this 值 as the Prism charges up.
		private const float MaxBeamScale = 1.8f;

		// Beams reduce their spread to zero as the Prism charges up. This controls the 最大 spread.
		private const float MaxBeamSpread = 2f;

		// 最大 possible 范围 的 beam. Don't set this too high or it will cause significant lag.
		private const float MaxBeamLength = 2400f;

		// 宽度 的 beam in pixels 对于 purposes of 图格 collision.
		// This should generally be 左 at 1, otherwise the beam tends to 停止 early when touching tiles.
		private const float BeamTileCollisionWidth = 1f;

		// 宽度 的 beam in pixels 对于 purposes of entity hitbox collision.
		// This gets scaled 与 beam's 缩放 值, so as the beam visually grows its hitbox gets wider 以及.
		private const float BeamHitboxCollisionWidth = 22f;

		// 数字 of sample points to use when performing a collision hitscan 对于 beam.
		// More points theoretically leads to a higher 品质 result, but can cause more lag. 3 tends to be enough.
		private const int NumSamplePoints = 3;

		// How quickly the beam adjusts to sudden changes in 长度.
		// Every 帧, the beam replaces this ratio of its current 长度 with its intended 长度.
		// Generally you shouldn't need to change this.
		// 设置ting it too low will make the beam lazily pass through walls before being blocked by them.
		private const float BeamLengthChangeFactor = 0.75f;

		// charge 百分比 required 在 主机 prism 对于 beam to begin visual effects (e.g. impact dust).
		private const float VisualEffectThreshold = 0.1f;

		// Each Last Prism beam draws two lasers separately: an inner beam and an outer beam. This controls their opacity.
		private const float OuterBeamOpacityMultiplier = 0.75f;
		private const float InnerBeamOpacityMultiplier = 0.1f;

		// 最大 brightness 的 light emitted by the beams. Brightness scales from 0 to this 值 as the Prism's charge increases.
		private const float BeamLightBrightness = 0.75f;

		// These variables 控制 the beam's potential coloration.
		// As a 值, hue ranges from 0f to 1f, both of which are pure red. The laser beams vary from 0.57 to 0.75, which winds up being a blue-to-purple gradient.
		// Saturation ranges from 0f to 1f and controls how greyed out the 颜色 is. 0 is fully grayscale, 1 is vibrant, intense 颜色.
		// Lightness ranges from 0f to 1f and controls how dark or light the 颜色 is. 0 is 音高 black. 1 is pure white.
		private const float BeamColorHue = 0.57f;
		private const float BeamHueVariance = 0.18f;
		private const float BeamColorSaturation = 0.66f;
		private const float BeamColorLightness = 0.53f;

		// This 属性 encloses the internal AI 变量 弹幕.ai[0]. It makes the code easier to read.
		private float BeamID {
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		// This 属性 encloses the internal AI 变量 弹幕.ai[1].
		private float HostPrismIndex {
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		// This 属性 encloses the internal AI 变量 弹幕.localAI[1].
		// Normally, localAI is not synced over the 网络. This beam manually syncs this 变量 using SendExtraAI and ReceiveExtraAI.
		private float BeamLength {
			get => Projectile.localAI[1];
			set => Projectile.localAI[1] = value;
		}

		public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			// beam itself still stops on tiles, but its invisible "source" 弹幕 ignores them.
			// This prevents the beams from vanishing if the 玩家 shoves the Prism into a 墙.
			Projectile.tileCollide = false;

			// 使用 local NPC immunity allows each beam to strike independently from one another.
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		// Send beam 长度 over the 网络 to 防止 hitbox-affecting and thus cascading desyncs in multiplayer.
		public override void SendExtraAI(BinaryWriter writer) => writer.Write(BeamLength);
		public override void ReceiveExtraAI(BinaryReader reader) => BeamLength = reader.ReadSingle();

		public override void AI() {
			// 如果 something has gone wrong with either the beam or the 主机 Prism, destroy the beam.
			Projectile hostPrism = Main.projectile[(int)HostPrismIndex];
			if (Projectile.type != ModContent.ProjectileType<ExampleLastPrismBeam>() || !hostPrism.active || hostPrism.type != ModContent.ProjectileType<ExampleLastPrismHoldout>()) {
				Projectile.Kill();
				return;
			}

			// Grab some variables 从 主机 Prism.
			Vector2 hostPrismDir = Vector2.Normalize(hostPrism.velocity);
			float chargeRatio = MathHelper.Clamp(hostPrism.ai[0] / ExampleLastPrismHoldout.MaxCharge, 0f, 1f);

			// 更新 the beam's 伤害 every 帧 based on charge and the 主机 Prism's 伤害.
			Projectile.damage = (int)(hostPrism.damage * GetDamageMultiplier(chargeRatio));

			// beam cannot strike enemies until the 主机 Prism is at a certain charge 级别.
			Projectile.friendly = hostPrism.ai[0] > ExampleLastPrismHoldout.DamageStart;

			// This 偏移 is used to make each individual beam orient differently based on its Beam ID.
			float beamIdOffset = BeamID - ExampleLastPrismHoldout.NumBeams / 2f + 0.5f;
			float beamSpread;
			float spinRate;
			float beamStartSidewaysOffset;
			float beamStartForwardsOffset;

			// Variables 缩放 smoothly while the 主机 Prism is charging up.
			if (chargeRatio < 1f) {
				Projectile.scale = MathHelper.Lerp(0f, MaxBeamScale, chargeRatio);
				beamSpread = MathHelper.Lerp(MaxBeamSpread, 0f, chargeRatio);
				beamStartSidewaysOffset = MathHelper.Lerp(20f, 6f, chargeRatio);
				beamStartForwardsOffset = MathHelper.Lerp(-21f, -17f, chargeRatio);

				// 对于 the first 2/3 of charge 时间, the opacity scales up from 0% to 40%.
				// Spin rate increases slowly during this 时间.
				if (chargeRatio <= 0.66f) {
					float phaseRatio = chargeRatio * 1.5f;
					Projectile.Opacity = MathHelper.Lerp(0f, 0.4f, phaseRatio);
					spinRate = MathHelper.Lerp(20f, 16f, phaseRatio);
				}

				// 对于 the last 1/3 of charge 时间, the opacity scales up from 40% to 100%.
				// Spin rate increases dramatically during this 时间.
				else {
					float phaseRatio = (chargeRatio - 0.66f) * 3f;
					Projectile.Opacity = MathHelper.Lerp(0.4f, 1f, phaseRatio);
					spinRate = MathHelper.Lerp(16f, 6f, phaseRatio);
				}
			}

			// 如果 the 主机 Prism is already at max charge, don't calculate anything. Just use the max values.
			else {
				Projectile.scale = MaxBeamScale;
				Projectile.Opacity = 1f;
				beamSpread = 0f;
				spinRate = 6f;
				beamStartSidewaysOffset = 6f;
				beamStartForwardsOffset = -17f;
			}

			// amount to which the 角度 changes reduces over 时间 so th在 beams look like they are focusing.
			float deviationAngle = (hostPrism.ai[0] + beamIdOffset * spinRate) / (spinRate * ExampleLastPrismHoldout.NumBeams) * MathHelper.TwoPi;

			// This trigonometry calculates where the beam is supposed to be pointing.
			Vector2 unitRot = Vector2.UnitY.RotatedBy(deviationAngle);
			Vector2 yVec = new Vector2(4f, beamStartSidewaysOffset);
			float hostPrismAngle = hostPrism.velocity.ToRotation();
			Vector2 beamSpanVector = (unitRot * yVec).RotatedBy(hostPrismAngle);
			float sinusoidYOffset = unitRot.Y * PiBeamDivisor * beamSpread;

			// 计算 the beam's emanating 位置. 开始 与 Prism's 中心.
			Projectile.Center = hostPrism.Center;
			// 添加 a fixed 偏移 to align 与 Prism's 精灵 sheet.
			Projectile.position += hostPrismDir * 16f + new Vector2(0f, -hostPrism.gfxOffY);
			// 添加 the forwards 偏移, measured in pixels.
			Projectile.position += hostPrismDir * beamStartForwardsOffset;
			// 添加 the sideways 偏移 vector, 即 calculated 对于 current 角度 的 beam and scales 与 beam's sideways 偏移.
			Projectile.position += beamSpanVector;

			// 设置 the beam's 速度 to 点 towards its current spread 方向 and sanity check it. It should have magnitude 1.
			Projectile.velocity = hostPrismDir.RotatedBy(sinusoidYOffset);
			if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero) {
				Projectile.velocity = -Vector2.UnitY;
			}
			Projectile.rotation = Projectile.velocity.ToRotation();

			// 更新 the beam's 长度 by performing a hitscan collision check.
			float hitscanBeamLength = PerformBeamHitscan(hostPrism, chargeRatio >= 1f);
			BeamLength = MathHelper.Lerp(BeamLength, hitscanBeamLength, BeamLengthChangeFactor);

			// This Vector2 stores the beam's hitbox statistics. X = beam 长度. Y = beam 宽度.
			Vector2 beamDims = new Vector2(Projectile.velocity.Length() * BeamLength, Projectile.width * Projectile.scale);

			// 仅 produce dust and cause water ripples if the beam is above a certain charge 级别.
			Color beamColor = GetOuterBeamColor();
			if (chargeRatio >= VisualEffectThreshold) {
				ProduceBeamDust(beamColor);

				// 如果 the game is rendering (i.e. isn't a dedicated 服务器), make the beam disturb water.
				if (Main.netMode != NetmodeID.Server) {
					ProduceWaterRipples(beamDims);
				}
			}

			// 使 the beam cast light along its 长度. The brightness 的 light scales 与 charge.
			// v3_1 is an unnamed decompiled 变量 即 the 颜色 的 light cast by DelegateMethods.CastLight.
			DelegateMethods.v3_1 = beamColor.ToVector3() * BeamLightBrightness * chargeRatio;
			Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * BeamLength, beamDims.Y, new Utils.TileActionAttempt(DelegateMethods.CastLight));
		}

		// 使用s a simple polynomial (x^3) to get sudden but smooth 伤害 increase near the 结束 的 charge-up period.
		private float GetDamageMultiplier(float chargeRatio) {
			float f = chargeRatio * chargeRatio * chargeRatio;
			return MathHelper.Lerp(1f, MaxDamageMultiplier, f);
		}

		private float PerformBeamHitscan(Projectile prism, bool fullCharge) {
			// 默认情况下, the hitscan interpolation starts 在 弹幕's 中心.
			// 如果 the 主机 Prism is fully charged, the interpolation starts 在 Prism's 中心 instead.
			Vector2 samplingPoint = Projectile.Center;
			if (fullCharge) {
				samplingPoint = prism.Center;
			}

			// Overriding that, if the 玩家 shoves the Prism into or through a 墙, the interpolation starts 在 玩家's 中心.
			// This last part prevents the 玩家 from projecting beams through walls under any circumstances.
			Player player = Main.player[Projectile.owner];
			if (!Collision.CanHitLine(player.Center, 0, 0, prism.Center, 0, 0)) {
				samplingPoint = player.Center;
			}

			// Perform a laser scan to calculate the correct 长度 的 beam.
			// Alternatively, if you want the beam to 忽略 tiles, just set it to be the max beam 长度 与 following line.
			// 返回 MaxBeamLength;
			float[] laserScanResults = new float[NumSamplePoints];
			Collision.LaserScan(samplingPoint, Projectile.velocity, 0 * Projectile.scale, MaxBeamLength, laserScanResults);
			float averageLengthSample = 0f;
			for (int i = 0; i < laserScanResults.Length; ++i) {
				averageLengthSample += laserScanResults[i];
			}
			averageLengthSample /= NumSamplePoints;

			return averageLengthSample;
		}

		// 确定s whether the specified 目标 hitbox is intersecting 与 beam.
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			// 如果 the 目标 is touching the beam's hitbox (即 a small rectangle vaguely overlapping the 主机 Prism), that's good enough.
			if (projHitbox.Intersects(targetHitbox)) {
				return true;
			}

			// 否则, perform an AABB line collision check to check the whole beam.
			float _ = float.NaN;
			Vector2 beamEndPos = Projectile.Center + Projectile.velocity * BeamLength;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, BeamHitboxCollisionWidth * Projectile.scale, ref _);
		}

		public override bool PreDraw(ref Color lightColor) {
			// 如果 the beam doesn't have a defined 方向, don't draw anything.
			if (Projectile.velocity == Vector2.Zero) {
				return false;
			}

			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Vector2 centerFloored = Projectile.Center.Floor() + Projectile.velocity * Projectile.scale * 10.5f;
			Vector2 drawScale = new Vector2(Projectile.scale);

			// Reduce the beam 长度 proportional to its square 区域 to reduce 方块 penetration.
			float visualBeamLength = BeamLength - 14.5f * Projectile.scale * Projectile.scale;

			DelegateMethods.f_1 = 1f; // f_1 is an unnamed decompiled 变量 whose 函数 is unknown. Leave it at 1.
			Vector2 startPosition = centerFloored - Main.screenPosition;
			Vector2 endPosition = startPosition + Projectile.velocity * visualBeamLength;

			// 绘制 the outer beam.
			DrawBeam(Main.spriteBatch, texture, startPosition, endPosition, drawScale, GetOuterBeamColor() * OuterBeamOpacityMultiplier * Projectile.Opacity);

			// 绘制 the inner beam, 即 half 大小.
			drawScale *= 0.5f;
			DrawBeam(Main.spriteBatch, texture, startPosition, endPosition, drawScale, GetInnerBeamColor() * InnerBeamOpacityMultiplier * Projectile.Opacity);

			// 返回ing 假 prevents Terraria from trying to draw the 弹幕 itself.
			return false;
		}

		private void DrawBeam(SpriteBatch spriteBatch, Texture2D texture, Vector2 startPosition, Vector2 endPosition, Vector2 drawScale, Color beamColor) {
			Utils.LaserLineFraming lineFraming = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);

			// c_1 is an unnamed decompiled 变量 即 the render 颜色 的 beam drawn by DelegateMethods.RainbowLaserDraw.
			DelegateMethods.c_1 = beamColor;
			Utils.DrawLaser(spriteBatch, texture, startPosition, endPosition, drawScale, lineFraming);
		}

		private Color GetOuterBeamColor() {
			// This hue 计算 produces a unique 颜色 for each beam based on its Beam ID.
			float hue = (BeamID / ExampleLastPrismHoldout.NumBeams) % BeamHueVariance + BeamColorHue;

			// Main.hslToRgb converts Hue, Saturation, Lightness into a 颜色 for general purpose use.
			Color c = Main.hslToRgb(hue, BeamColorSaturation, BeamColorLightness);

			// Manually reduce the opacity 的 颜色 so beams can overlap without completely overwriting each other.
			c.A = 64;
			return c;
		}

		// Inner beams are always pure white so th在y act as a "blindingly bright" 中心 to each laser.
		private Color GetInnerBeamColor() => Color.White;

		private void ProduceBeamDust(Color beamColor) {
			// 创建 one dust per 帧 a small 距离 from where the beam ends.
			const int type = 15;
			Vector2 endPosition = Projectile.Center + Projectile.velocity * (BeamLength - 14.5f * Projectile.scale);

			// Main.rand.NextBool is used to give a 50/50 概率 对于 角度 to 点 到 左 or 右.
			// This gives the dust a 50/50 概率 to fly off on either side 的 beam.
			float angle = Projectile.rotation + (Main.rand.NextBool() ? 1f : -1f) * MathHelper.PiOver2;
			float startDistance = Main.rand.NextFloat(1f, 1.8f);
			float scale = Main.rand.NextFloat(0.7f, 1.1f);
			Vector2 velocity = angle.ToRotationVector2() * startDistance;
			Dust dust = Dust.NewDustDirect(endPosition, 0, 0, type, velocity.X, velocity.Y, 0, beamColor, scale);
			dust.color = beamColor;
			dust.noGravity = true;

			// 如果 the beam is currently large, make the dust faster and larger to 匹配.
			if (Projectile.scale > 1f) {
				dust.velocity *= Projectile.scale;
				dust.scale *= Projectile.scale;
			}
		}

		private void ProduceWaterRipples(Vector2 beamDims) {
			WaterShaderData shaderData = (WaterShaderData)Filters.Scene["WaterDistortion"].GetShader();

			// 一个 universal 时间-based sinusoid which updates extremely rapidly. GlobalTime is 0 to 3600, measured in seconds.
			float waveSine = 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
			Vector2 ripplePos = Projectile.position + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(Projectile.rotation);

			// WaveData is encoded as a 颜色. Not really sure why.
			Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
			shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, Projectile.rotation);
		}


		// Automatically iterates through every 图格 the laser is overlapping to 剪切 grass at all those locations.
		public override void CutTiles() {
			// tilecut_0 is an unnamed decompiled 变量 which tells CutTiles how the tiles are being 剪切 (in this case, via a 弹幕).
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Utils.TileActionAttempt cut = new Utils.TileActionAttempt(DelegateMethods.CutTiles);
			Vector2 beamStartPos = Projectile.Center;
			Vector2 beamEndPos = beamStartPos + Projectile.velocity * BeamLength;

			// PlotTileLine is a 函数 which performs the specified action to all tiles along a drawn line, with a specified 宽度.
			// 在 this case, it is cutting all tiles which 可以 destroyed by Projectiles, 例如 grass or pots.
			Utils.PlotTileLine(beamStartPos, beamEndPos, Projectile.width * Projectile.scale, cut);
		}
	}
}