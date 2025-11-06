using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace ExampleMod.Projectiles
{
	// 以下 laser shows a channeled ability, after charging up the laser 将 fired
	// Using custom drawing, dust effects, and custom collision checks for tiles
	public class ExampleLaser : ModProjectile
	{
		// 使用 a different style for constant so it is very 清除 in code when a constant is used

		// The 最大 charge 值
		private const float MAX_CHARGE = 50f;
		//The 距离 charge particle 从 玩家 中心
		private const float MOVE_DISTANCE = 60f;

		// The actual 距离 is stored 在 ai0 字段
		// By making a 属性 to 处理 this it makes our life easier, and the 可访问性 more readable
		public float Distance {
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		// The actual charge 值 is stored 在 localAI0 字段
		public float Charge {
			get => projectile.localAI[0];
			set => projectile.localAI[0] = value;
		}

		// Are we at max charge? With c#6 you can simply use => which indicates this is a get only 属性
		public bool IsAtMaxCharge => Charge == MAX_CHARGE;

		public override void SetDefaults() {
			projectile.width = 10;
			projectile.height = 10;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.magic = true;
			projectile.hide = true;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			// We 开始 drawing the laser if we have charged up
			if (IsAtMaxCharge) {
				DrawLaser(spriteBatch, Main.projectileTexture[projectile.type], Main.player[projectile.owner].Center,
					projectile.velocity, 10, projectile.damage, -1.57f, 1f, 1000f, Color.White, (int)MOVE_DISTANCE);
			}
			return false;
		}

		// The core 函数 of drawing a laser
		public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50) {
			float r = unit.ToRotation() + rotation;

			// 绘制s the laser 'body'
			for (float i = transDist; i <= Distance; i += step) {
				Color c = Color.White;
				var origin = start + i * unit;
				spriteBatch.Draw(texture, origin - Main.screenPosition,
					new Rectangle(0, 26, 28, 26), i < transDist ? Color.Transparent : c, r,
					new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
			}

			// 绘制s the laser 'tail'
			spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
				new Rectangle(0, 0, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);

			// 绘制s the laser 'head'
			spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
				new Rectangle(0, 52, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
		}

		// 更改 the way of collision check 的 弹幕
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			// 我们可以 only collide if we are at max charge, 即 when the laser is actually fired
			if (!IsAtMaxCharge) return false;

			Player player = Main.player[projectile.owner];
			Vector2 unit = projectile.velocity;
			float point = 0f;
			// 运行 an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
			// 它将 look for collisions 在 given line using AABB
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center,
				player.Center + unit * Distance, 22, ref point);
		}

		// 设置 custom immunity 时间 on hitting an NPC
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
			target.immune[projectile.owner] = 5;
		}

		// The AI 的 弹幕
		public override void AI() {
			Player player = Main.player[projectile.owner];
			projectile.position = player.Center + projectile.velocity * MOVE_DISTANCE;
			projectile.timeLeft = 2;

			// By separating large AI into methods it becomes very easy to see the flow 的 AI in a broader sense
			// 首先 we 更新 玩家 variables that are needed to 通道 the laser
			// Then we run our charging laser logic
			// If we are fully charged, we proceed to 更新 the laser's 位置
			// 最后 we 生成 some effects like dusts and light

			UpdatePlayer(player);
			ChargeLaser(player);

			// If laser is not charged yet, 停止 the AI here.
			if (Charge < MAX_CHARGE) return;

			SetLaserPosition(player);
			SpawnDusts(player);
			CastLights();
		}

		private void SpawnDusts(Player player) {
			Vector2 unit = projectile.velocity * -1;
			Vector2 dustPos = player.Center + projectile.velocity * Distance;

			for (int i = 0; i < 2; ++i) {
				float num1 = projectile.velocity.ToRotation() + (Main.rand.Next(2) == 1 ? -1.0f : 1.0f) * 1.57f;
				float num2 = (float)(Main.rand.NextDouble() * 0.8f + 1.0f);
				Vector2 dustVel = new Vector2((float)Math.Cos(num1) * num2, (float)Math.Sin(num1) * num2);
				Dust dust = Main.dust[Dust.NewDust(dustPos, 0, 0, 226, dustVel.X, dustVel.Y)];
				dust.noGravity = true;
				dust.scale = 1.2f;
				dust = Dust.NewDustDirect(Main.player[projectile.owner].Center, 0, 0, 31,
					-unit.X * Distance, -unit.Y * Distance);
				dust.fadeIn = 0f;
				dust.noGravity = true;
				dust.scale = 0.88f;
				dust.color = Color.Cyan;
			}

			if (Main.rand.NextBool(5)) {
				Vector2 offset = projectile.velocity.RotatedBy(1.57f) * ((float)Main.rand.NextDouble() - 0.5f) * projectile.width;
				Dust dust = Main.dust[Dust.NewDust(dustPos + offset - Vector2.One * 4f, 8, 8, 31, 0.0f, 0.0f, 100, new Color(), 1.5f)];
				dust.velocity *= 0.5f;
				dust.velocity.Y = -Math.Abs(dust.velocity.Y);
				unit = dustPos - Main.player[projectile.owner].Center;
				unit.Normalize();
				dust = Main.dust[Dust.NewDust(Main.player[projectile.owner].Center + 55 * unit, 8, 8, 31, 0.0f, 0.0f, 100, new Color(), 1.5f)];
				dust.velocity = dust.velocity * 0.5f;
				dust.velocity.Y = -Math.Abs(dust.velocity.Y);
			}
		}

		/*
		* Sets the end 的 laser position based on where it collides with something
		*/
		private void SetLaserPosition(Player player) {
			for (Distance = MOVE_DISTANCE; Distance <= 2200f; Distance += 5f) {
				var start = player.Center + projectile.velocity * Distance;
				if (!Collision.CanHit(player.Center, 1, 1, start, 1, 1)) {
					Distance -= 5f;
					break;
				}
			}
		}

		private void ChargeLaser(Player player) {
			// Kill the 弹幕 if the 玩家 stops channeling
			if (!player.channel) {
				projectile.Kill();
			}
			else {
				// Do we still have enough 魔力? If not, we kill the 弹幕 because we cannot use it 任何more
				if (Main.time % 10 < 1 && !player.CheckMana(player.inventory[player.selectedItem].mana, true)) {
					projectile.Kill();
				}
				Vector2 offset = projectile.velocity;
				offset *= MOVE_DISTANCE - 20;
				Vector2 pos = player.Center + offset - new Vector2(10, 10);
				if (Charge < MAX_CHARGE) {
					Charge++;
				}
				int chargeFact = (int)(Charge / 20f);
				Vector2 dustVelocity = Vector2.UnitX * 18f;
				dustVelocity = dustVelocity.RotatedBy(projectile.rotation - 1.57f);
				Vector2 spawnPos = projectile.Center + dustVelocity;
				for (int k = 0; k < chargeFact + 1; k++) {
					Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f - chargeFact * 2);
					Dust dust = Main.dust[Dust.NewDust(pos, 20, 20, 226, projectile.velocity.X / 2f, projectile.velocity.Y / 2f)];
					dust.velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * (10f - chargeFact * 2f) / 10f;
					dust.noGravity = true;
					dust.scale = Main.rand.Next(10, 20) * 0.05f;
				}
			}
		}

		private void UpdatePlayer(Player player) {
			// Multiplayer support here, only run this code if the 客户端 running it is the 所有者 的 弹幕
			if (projectile.owner == Main.myPlayer) {
				Vector2 diff = Main.MouseWorld - player.Center;
				diff.Normalize();
				projectile.velocity = diff;
				projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
				projectile.netUpdate = true;
			}
			int dir = projectile.direction;
			player.ChangeDir(dir); // 设置 玩家 方向 to where we are shooting
			player.heldProj = projectile.whoAmI; // 更新 玩家's held 弹幕
			player.itemTime = 2; // 设置 项 时间 to 2 frames while we are used
			player.itemAnimation = 2; // 设置 项 动画 时间 to 2 frames while we are used
			player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * dir, projectile.velocity.X * dir); // 设置 the 项 旋转 to where we are shooting
		}

		private void CastLights() {
			// Cast a light along the line 的 laser
			DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * (Distance - MOVE_DISTANCE), 26, DelegateMethods.CastLight);
		}

		public override bool ShouldUpdatePosition() => false;

		/*
		* Update CutTiles so the laser will cut tiles (like grass)
		*/
		public override void CutTiles() {
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Vector2 unit = projectile.velocity;
			Utils.PlotTileLine(projectile.Center, projectile.Center + unit * Distance, (projectile.width + 16) * projectile.scale, DelegateMethods.CutTiles);
		}
	}
}
