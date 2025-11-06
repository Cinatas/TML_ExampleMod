using ExampleMod.Content.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	// 此文件 shows an animated 弹幕
	// 此文件 also shows advanced drawing to 中心 the drawn 弹幕 correctly
	public class ExampleAdvancedAnimatedProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			// Total 计数 动画 frames
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults() {
			Projectile.width = 40; // The 宽度 of 弹幕 hitbox
			Projectile.height = 40; // The 高度 of 弹幕 hitbox

			Projectile.friendly = true; // Can the 弹幕 deal 伤害 to enemies?
			Projectile.DamageType = DamageClass.Melee; // Is the 弹幕 shoot by a ranged 武器?
			Projectile.ignoreWater = true; // Does the 弹幕's 速度 be influenced by water?
			Projectile.tileCollide = false; // Can the 弹幕 collide with tiles?
			Projectile.penetrate = -1; // Look at comments ExamplePiercingProjectile

			Projectile.alpha = 255; // How transparent to draw this 弹幕. 0 to 255. 255 is completely transparent.
		}

		// 允许s you to determine the 颜色 and transparency in which a 弹幕 is drawn
		// 返回 空 to use the default 颜色 (normally light and 增益 颜色)
		// 返回s 空 默认情况下.
		public override Color? GetAlpha(Color lightColor) {
			// 返回 颜色.White;
			return new Color(255, 255, 255, 0) * Projectile.Opacity;
		}

		public override void AI() {
			// All projectiles have timers that 帮助 to 延迟 certain events
			// 弹幕.ai[0], 弹幕.ai[1] — timers that are automatically synchronized 在 客户端 and 服务器
			// 弹幕.localAI[0], 弹幕.localAI[0] — only 在 客户端
			// 在 this example, a 计时器 is 用于 控制 the fade in / out and despawn 的 弹幕
			Projectile.ai[0] += 1f;

			FadeInAndOut();

			// Slow down
			Projectile.velocity *= 0.98f;

			// 循环 through the 4 动画 frames, spending 5 ticks on each
			// 弹幕.帧 — 索引 of current 帧
			if (++Projectile.frameCounter >= 5) {
				Projectile.frameCounter = 0;
				// 或更多 compactly 弹幕.帧 = ++弹幕.帧 % Main.projFrames[弹幕.类型];
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
					Projectile.frame = 0;
			}

			// Despawn this 弹幕 after 1 second (60 ticks)
			// 你 can use 弹幕.timeLeft = 60f in SetDefaults() for same 目标
			if (Projectile.ai[0] >= 60f)
				Projectile.Kill();

			// 设置 两者 方向 and spriteDirection to 1 or -1 (右 and 左 respectively)
			// 弹幕.方向 is automatically set correctly in 弹幕.更新, but we 需要 set it here or the textures will draw incorrectly 在 1st 帧.
			Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : -1;

			Projectile.rotation = Projectile.velocity.ToRotation();
			// Since our 精灵 has an orientation, we 需要 adjust 旋转 to compensate 对于 draw flipping
			if (Projectile.spriteDirection == -1) {
				Projectile.rotation += MathHelper.Pi;
				// 对于 vertical sprites use MathHelper.PiOver2
			}
		}

		// M任何 projectiles fade in 以便 when they 生成 they don't overlap the gun muzzle they appear from
		public void FadeInAndOut() {
			// 如果 last 少于 50 ticks — fade in, than more — fade out
			if (Projectile.ai[0] <= 50f) {
				// Fade in
				Projectile.alpha -= 25;
				// Cap alpha before 计时器 reaches 50 ticks
				if (Projectile.alpha < 100)
					Projectile.alpha = 100;

				return;
			}

			// Fade out
			Projectile.alpha += 25;
			// Cal alpha 到 最大 255(complete transparent)
			if (Projectile.alpha > 255)
				Projectile.alpha = 255;
		}

		// Some advanced drawing because the 纹理 图像 isn't centered or symmetrical
		// 如果 you don't 想要 manually drawing you can use vanilla 弹幕 rendering offsets
		// 在这里 you can check it https://github.com/tModLoader/tModLoader/wiki/Basic-弹幕#horizontal-精灵-example
		public override bool PreDraw(ref Color lightColor) {
			// SpriteEffects helps to flip 纹理 horizontally and vertically
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (Projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			// 获取ting 纹理 of 弹幕
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			// Calculating frameHeight and current Y pos dependence of 帧
			// 如果 纹理 without 动画 frameHeight is always 纹理.高度 and startY is always 0
			int frameHeight = texture.Height / Main.projFrames[Type];
			int startY = frameHeight * Projectile.frame;

			// 获取 this 帧 on 纹理
			Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

			// Alternatively, you can 跳过 defining frameHeight and startY and use this:
			// Rectangle sourceRectangle = 纹理.帧(1, Main.projFrames[类型], frameY: 弹幕.帧);

			Vector2 origin = sourceRectangle.Size() / 2f;

			// 如果 图像 isn't centered or symmetrical you can specify 原点 的 精灵
			// (0,0) 对于 upper-左 corner
			float offsetX = 20f;
			origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

			// 如果 精灵 is vertical
			// float offsetY = 20f;
			// 原点.Y = (float)(弹幕.spriteDirection == 1 ? sourceRectangle.高度 - offsetY : offsetY);


			// 应用ing lighting and draw current 帧
			Color drawColor = Projectile.GetAlpha(lightColor);
			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

			// It's important to 返回 假, 否则 we also draw the original 纹理.
			return false;
		}
	}

	// 这是 a simple 项 即 based 在 NebulaBlaze and shoots ExampleAdvancedAnimatedProjectile to showcase it.
	internal class ExampleAdvancedAnimatedProjectileItem : ModItem
	{
		public override string Texture => $"Terraria/Images/Item_{ItemID.NebulaBlaze}";

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.NebulaBlaze);
			Item.mana = 3;
			Item.damage = 3;
			Item.shoot = ModContent.ProjectileType<ExampleAdvancedAnimatedProjectile>();
		}
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
