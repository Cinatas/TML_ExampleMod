using ExampleMod.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleGravityDebuffBullet : ModProjectile
	{
		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The 长度 of old 位置 to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording 模式
		}

		public override void SetDefaults() {
			Projectile.width = 8; // The 宽度 of 弹幕 hitbox
			Projectile.height = 8; // The 高度 of 弹幕 hitbox
			Projectile.aiStyle = 1; // The ai style 的 弹幕, please 引用 the source code of Terraria
			Projectile.friendly = true; // Can the 弹幕 deal 伤害 to enemies?
			Projectile.hostile = false; // Can the 弹幕 deal 伤害 到 玩家?
			Projectile.DamageType = DamageClass.Ranged; // Is the 弹幕 shoot by a ranged 武器?
			Projectile.penetrate = 5; // How many monsters the 弹幕 can penetrate. (OnTileCollide below also decrements penetrate for bounces 以及)
			Projectile.timeLeft = 600; // The live 时间 对于 弹幕 (60 = 1 second, so 600 is 10 seconds)
			Projectile.alpha = 255; // The transparency 的 弹幕, 255 for completely transparent. (aiStyle 1 quickly fades the 弹幕 in) Make sure to 删除 this if you aren't using an aiStyle that fades in. You'll wonder why your 弹幕 is invisible.
			Projectile.light = 0.5f; // How much light emit around the 弹幕
			Projectile.ignoreWater = true; // Does the 弹幕's 速度 be influenced by water?
			Projectile.tileCollide = true; // Can the 弹幕 collide with tiles?
			Projectile.extraUpdates = 1; // 设置 to above 0 if you want the 弹幕 to 更新 multiple 时间 in a 帧

			AIType = ProjectileID.Bullet; // Act exactly like default 子弹
		}

		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			// Redraw the 弹幕 与 颜色 not influenced by light
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}

		public override void OnKill(int timeLeft) {
			// This code and the similar code above in OnTileCollide 生成 dust 从 tiles collided with. SoundID.Item10 is the bounce 声音 you hear.
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(ModContent.BuffType<ExampleGravityDebuff>(), 600);
		}
	}
}