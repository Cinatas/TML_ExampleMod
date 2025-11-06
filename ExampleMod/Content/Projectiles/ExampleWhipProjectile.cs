using ExampleMod.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleWhipProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			// 这使 the 弹幕 use whip collision detection and allows flasks to be applied to it.
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() {
			// 此方法 quickly sets the whip's properties.
			Projectile.DefaultToWhip();

			// use these to change 从 vanilla defaults
			// 弹幕.WhipSettings.Segments = 20;
			// 弹幕.WhipSettings.RangeMultiplier = 1f;
		}

		private float Timer {
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private float ChargeTime {
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		// 此示例 uses PreAI to implement a charging mechanic.
		// 如果 you 删除 this, also 删除 项.通道 = 真 从 项's SetDefaults.
		public override bool PreAI() {
			Player owner = Main.player[Projectile.owner];

			// 像 other whips, this whip updates twice per 帧 (弹幕.extraUpdates = 1), so 120 is equal to 1 second.
			if (!owner.channel || ChargeTime >= 120) {
				return true; // Let the vanilla whip AI run.
			}

			if (++ChargeTime % 12 == 0) // 1 segment per 12 ticks of charge.
				Projectile.WhipSettings.Segments++;

			// Increase 范围 up to 2x for full charge.
			Projectile.WhipSettings.RangeMultiplier += 1 / 120f;

			// 重置 the 动画 and 项 计时器 while charging.
			owner.itemAnimation = owner.itemAnimationMax;
			owner.itemTime = owner.itemTimeMax;

			return false; // 防止 the vanilla whip AI from running.
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(ModContent.BuffType<ExampleWhipDebuff>(), 240);
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(Projectile.damage * 0.5f); // Multihit 惩罚. Decrease the 伤害 the more enemies the whip hits.
		}

		// 此方法 draws a line between all points 的 whip, in case there's empty space between the sprites.
		private void DrawLine(List<Vector2> list) {
			Texture2D texture = TextureAssets.FishingLine.Value;
			Rectangle frame = texture.Frame();
			Vector2 origin = new Vector2(frame.Width / 2, 2);

			Vector2 pos = list[0];
			for (int i = 0; i < list.Count - 1; i++) {
				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2;
				Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
				Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}
		}

		public override bool PreDraw(ref Color lightColor) {
			List<Vector2> list = new List<Vector2>();
			Projectile.FillWhipControlPoints(Projectile, list);

			DrawLine(list);

			//Main.DrawWhip_WhipBland(弹幕, 列表);
			// code below is for custom drawing.
			// 如果 you don't want that, you can 删除 it all and instead call one of vanilla's DrawWhip methods, like above.
			// 然而, 你必须 adhere to how they draw if you do.

			SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++) {
				// These two values are set to suit this 弹幕's 精灵, but won't necessarily work for your own.
				// 你 can change them if they don't!
				Rectangle frame = new Rectangle(0, 0, 10, 26); // The 大小 的 处理 (measured in pixels)
				Vector2 origin = new Vector2(5, 8); // 偏移 for where the 玩家's hand will 开始 measured 从 顶部 左 的 图像.
				float scale = 1;

				// These statements determine what part 的 spritesheet to draw 对于 current segment.
				// They can also be changed to suit your 精灵.
				if (i == list.Count - 2) {
					// 这是 the head 的 whip. You 需要 measure the 精灵 to figure out these values.
					frame.Y = 74; // 距离 从 顶部 的 精灵 到 开始 的 帧.
					frame.Height = 18; // 高度 的 帧.

					// 对于 a more impactful look, this scales the 提示 的 whip up when fully extended, and down when curled up.
					Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
					float t = Timer / timeToFlyOut;
					scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
				}
				else if (i > 10) {
					// Third segment
					frame.Y = 58;
					frame.Height = 16;
				}
				else if (i > 5) {
					// 其次 Segment
					frame.Y = 42;
					frame.Height = 16;
				}
				else if (i > 0) {
					// 首先 Segment
					frame.Y = 26;
					frame.Height = 16;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2; // This 弹幕's 精灵 faces down, so PiOver2 is 用于 correct 旋转.
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

				pos += diff;
			}
			return false;
		}
	}
}
