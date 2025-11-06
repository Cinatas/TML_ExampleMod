using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	// 示例Flail and ExampleFlailProjectile show the 最小 amount of code needed for a flail using the existing vanilla code and behavior. ExampleAdvancedFlail and ExampleAdvancedFlailProjectile 需要 be consulted if more advanced customization is desired, or if you 想要 learn more advanced modding techniques.
	// 示例FlailProjectile is a 复制 的 Sunfury flail 弹幕.
	internal class ExampleFlailProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.netImportant = true; // 这确保 th在 弹幕 is synced when other players jo在 世界.
			Projectile.width = 22; // The 宽度 of your 弹幕
			Projectile.height = 22; // The 高度 of your 弹幕
			Projectile.friendly = true; // Deals 伤害 to enemies
			Projectile.penetrate = -1; // Infinite pierce
			Projectile.DamageType = DamageClass.Melee; // Deals melee 伤害
			Projectile.scale = 0.8f;
			Projectile.usesLocalNPCImmunity = true; // 使用d for hit cooldown changes 在 ai hook
			Projectile.localNPCHitCooldown = 10; // This facilitates custom hit cooldown logic

			// 在这里 we reuse the flail 弹幕 aistyle and set the aitype 到 Sunfury. These lines will get our 弹幕 to behave exactly like Sunfury would. This only affects the AI code, you'll 需要 adapt other code 对于 other behaviors you wish to use.
			Projectile.aiStyle = ProjAIStyleID.Flail;
			AIType = ProjectileID.Sunfury;

			// These 帮助 中心 the 弹幕 as it rotates since its hitbox and 缩放 doesn't 匹配 the actual 纹理 大小
			DrawOffsetX = -6;
			DrawOriginOffsetY = -6;
		}

		// All 的 following methods are additional behaviors of Sunfury that are not automatically inherited by ExampleFlailProjectile through the use of 弹幕.aiStyle and AIType. You'll 需要 查找 corresponding code 在 decompiled source code if you wish to clone a different vanilla 弹幕 as a starting 点.

		// 绘制 the 弹幕 in full brightness, ignoring lighting conditions.
		public override Color? GetAlpha(Color lightColor) {
			return Color.White;
		}

		// 在 PreDrawExtras, we trick the game into thinking the 弹幕 is actually a Sunfury 弹幕. After PreDrawExtras, the Terraria code will draw the chain. Drawing the chain ourselves is quite complicated, ExampleAdvancedFlailProjectile has an example of that. Then, in PreDraw, we restore the 弹幕.类型 back to normal so we don't 中断 任何thing.  
		public override bool PreDrawExtras() {
			Projectile.type = ProjectileID.Sunfury;
			return base.PreDrawExtras();
		}
		public override bool PreDraw(ref Color lightColor) {
			Projectile.type = ModContent.ProjectileType<ExampleFlailProjectile>();

			// This code handles the after images.
			if (Projectile.ai[0] == 1f) {
				Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
				Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
				Vector2 drawOrigin = new Vector2(projectileTexture.Width, projectileTexture.Height) / 2f;
				Color drawColor = Projectile.GetAlpha(lightColor);
				drawColor.A = 127;
				drawColor *= 0.5f;
				int launchTimer = (int)Projectile.ai[1];
				if (launchTimer > 5) {
					launchTimer = 5;
				}

				SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

				for (float transparency = 1f; transparency >= 0f; transparency -= 0.125f) {
					float opacity = 1f - transparency;
					Vector2 drawAdjustment = Projectile.velocity * -launchTimer * transparency;
					Main.EntitySpriteDraw(projectileTexture, drawPosition + drawAdjustment, null, drawColor * opacity, Projectile.rotation, drawOrigin, Projectile.scale * 1.15f * MathHelper.Lerp(0.5f, 1f, opacity), spriteEffects, 0);
				}
			}

			return base.PreDraw(ref lightColor);
		}

		// Another thing that won't automatically be inherited by using 弹幕.aiStyle and AIType are effects that happen when the 弹幕 hits something. Here we see the code responsible for applying the OnFire 减益 to players and enemies.
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			if (Main.rand.NextBool(2)) {
				target.AddBuff(BuffID.OnFire, 300);
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info) {
			if (Main.rand.NextBool(4)) {
				target.AddBuff(BuffID.OnFire, 180, quiet: false);
			}
		}

		// 最后, you can slightly customize the AI if you read and understand the vanilla aiStyle source code. You can't customize the 范围, retract speeds, or 任何thing else. If you 需要 customize those things, you'll 需要 follow ExampleAdvancedFlailProjectile. This example spawns a 手榴弹 右 when the flail starts to retract. 
		public override void AI() {
			// only reason this code works is because the 作者 read the vanilla code and comprehended it well enough to tack on additional logic.
			if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 2f && Projectile.ai[1] == 0f) {
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileID.Grenade, Projectile.damage, Projectile.knockBack, Main.myPlayer);
				Projectile.ai[1]++;
			}
		}
	}
}
