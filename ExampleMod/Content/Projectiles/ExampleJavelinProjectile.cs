using ExampleMod.Content.Dusts;
using ExampleMod.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	// This 弹幕 showcases advanced AI code. Of particular note is a showcase on how projectiles can stick to NPCs in a manner similar 到 behavior of vanilla weapons 例如 Bone Javelin, Daybreak, Blood Butcherer, Stardust 单元格 仆从, and Tentacle Spike. This code is modeled closely after Bone Javelin.
	public class ExampleJavelinProjectile : ModProjectile
	{
		// These properties wrap the usual ai arrays for cleaner and easier to understand code.
		// Are we sticking to a 目标?
		public bool IsStickingToTarget {
			get => Projectile.ai[0] == 1f;
			set => Projectile.ai[0] = value ? 1f : 0f;
		}

		// 索引 的 current 目标
		public int TargetWhoAmI {
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public int GravityDelayTimer {
			get => (int)Projectile.ai[2];
			set => Projectile.ai[2] = value;
		}

		public float StickTimer {
			get => Projectile.localAI[0];
			set => Projectile.localAI[0] = value;
		}

		public override void SetStaticDefaults() {
			ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 16; // The 宽度 of 弹幕 hitbox
			Projectile.height = 16; // The 高度 of 弹幕 hitbox
			Projectile.aiStyle = 0; // The ai style 的 弹幕 (0 means custom AI). F或更多 please 引用 the source code of Terraria
			Projectile.friendly = true; // Can the 弹幕 deal 伤害 to enemies?
			Projectile.hostile = false; // Can the 弹幕 deal 伤害 到 玩家?
			Projectile.DamageType = DamageClass.Ranged; // 使 the 弹幕 deal ranged 伤害. You can set in to DamageClass.Throwing, but 即 not used by 任何 vanilla items
			Projectile.penetrate = 2; // How m任何 monsters the 弹幕 can penetrate.
			Projectile.timeLeft = 600; // The live 时间 对于 弹幕 (60 = 1 second, so 600 is 10 seconds)
			Projectile.alpha = 255; // The transparency 的 弹幕, 255 for completely transparent. Our custom AI below fades our 弹幕 in. 确保 to 删除 this if you aren't using an aiStyle that fades in.
			Projectile.light = 0.5f; // How much light emit around the 弹幕
			Projectile.ignoreWater = true; // Does the 弹幕's 速度 be influenced by water?
			Projectile.tileCollide = true; // Can the 弹幕 collide with tiles?
			Projectile.hide = true; // 使 the 弹幕 completely invisible. 我们需要 this to draw our 弹幕 behind enemies/tiles in DrawBehind()
		}

		private const int GravityDelay = 45;

		public override void AI() {
			UpdateAlpha();
			// 运行 任一 the Sticky AI or Normal AI
			// Separating into different methods helps keeps your AI clean
			if (IsStickingToTarget) {
				StickyAI();
			}
			else {
				NormalAI();
			}
		}

		private void NormalAI() {
			GravityDelayTimer++; // doesn't make sense.

			// 对于 一点 while, the javelin will travel 与 same 速度, but after this, the javelin drops 速度 very quickly.
			if (GravityDelayTimer >= GravityDelay) {
				GravityDelayTimer = GravityDelay;

				// wind resistance
				Projectile.velocity.X *= 0.98f;
				// gravity
				Projectile.velocity.Y += 0.35f;
			}

			// 偏移 the 旋转 by 90 degrees because the 精灵 is oriented vertically.
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

			// 生成 some 随机 dusts as the javelin travels
			if (Main.rand.NextBool(3)) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, ModContent.DustType<Sparkle>(), Projectile.velocity.X * .2f, Projectile.velocity.Y * .2f, 200, Scale: 1.2f);
				dust.velocity += Projectile.velocity * 0.3f;
				dust.velocity *= 0.2f;
			}
			if (Main.rand.NextBool(4)) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, ModContent.DustType<Sparkle>(),
					0, 0, 254, Scale: 0.3f);
				dust.velocity += Projectile.velocity * 0.5f;
				dust.velocity *= 0.5f;
			}
		}

		private const int StickTime = 60 * 15; // 15 seconds
		private void StickyAI() {
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			StickTimer += 1f;

			// Every 30 ticks, the javelin will perform a hit 效果
			bool hitEffect = StickTimer % 30f == 0f;
			int npcTarget = TargetWhoAmI;
			if (StickTimer >= StickTime || npcTarget < 0 || npcTarget >= 200) { // If the 索引 is past its limits, kill it
				Projectile.Kill();
			}
			else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage) {
				// 如果 the 目标 is active and can take 伤害
				// 设置 the 弹幕's 位置 relative 到 目标's 中心
				Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
				Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
				if (hitEffect) {
					// Perform a hit 效果 here, causing the npc to react as if hit.
					// 注意 that this does NOT 伤害 the NPC, the 伤害 is done through the 减益.
					Main.npc[npcTarget].HitEffect(0, 1.0);
				}
			}
			else { // 否则, kill the 弹幕
				Projectile.Kill();
			}
		}

		public override void OnKill(int timeLeft) {
			SoundEngine.PlaySound(SoundID.Dig, Projectile.position); // Play a death 声音
			Vector2 usePos = Projectile.position; // 位置 to use for dusts

			// 偏移 the 旋转 by 90 degrees because the 精灵 is oriented vertically.
			Vector2 rotationVector = (Projectile.rotation - MathHelper.ToRadians(90f)).ToRotationVector2(); // 旋转 vector to use for dust 速度
			usePos += rotationVector * 16f;

			// 生成 some dusts upon javelin death
			for (int i = 0; i < 20; i++) {
				// 创建 a new dust
				Dust dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, DustID.Tin);
				dust.position = (dust.position + Projectile.Center) / 2f;
				dust.velocity += rotationVector * 2f;
				dust.velocity *= 0.5f;
				dust.noGravity = true;
				usePos -= rotationVector * 8f;
			}

			// 确保 to only 生成 items if you are the 弹幕 所有者.
			// 这是 an important check as Kill() is called on clients, and you only want the 项 to 放下 once
			if (Projectile.owner == Main.myPlayer) {
				// 放下 a javelin 项, 1 in 18 概率 (~5.5% 概率)
				int item = 0;
				if (Main.rand.NextBool(18)) {
					item = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.getRect(), ModContent.ItemType<ExampleJavelin>());
				}

				// 同步 the 放下 for multiplayer
				// 注意 the usage of Terraria.ID.MessageID, please use this!
				if (Main.netMode == NetmodeID.MultiplayerClient && item >= 0) {
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);
				}
			}
		}

		private const int MaxStickingJavelin = 6; // 这是 max amount of javelins 能够 be attached to a single NPC
		private readonly Point[] stickingJavelins = new Point[MaxStickingJavelin]; // The 点 数组 holding for sticking javelins

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			IsStickingToTarget = true; // we are sticking to a 目标
			TargetWhoAmI = target.whoAmI; // 设置 the 目标 whoAmI
			Projectile.velocity = (target.Center - Projectile.Center) *
				0.75f; // 更改 速度 基于 delta 中心 of targets (difference between entity centers)
			Projectile.netUpdate = true; // netUpdate this javelin
			Projectile.damage = 0; // 使 sure the sticking javelins do not deal 伤害 任何more

			// 示例JavelinBuff handles the 伤害 over 时间 (DoT)
			target.AddBuff(ModContent.BuffType<Buffs.ExampleJavelinDebuff>(), 900);

			// KillOldestJavelin will kill the oldest 弹幕 stuck 到 specified npc.
			// It only works if ai[0] is 1 when sticking and ai[1] is the 目标 npc 索引, 即 what IsStickingToTarget and TargetWhoAmI correspond to.
			Projectile.KillOldestJavelin(Projectile.whoAmI, Type, target.whoAmI, stickingJavelins);
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
			// 对于 going through platforms and such, javelins use a tad smaller 大小
			width = height = 10; // notice we set the 宽度 到 高度, the 高度 to 10. so 两者 are 10
			return true;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			// By shrinking 目标 hitboxes by a small amount, this 弹幕 only hits if it more directly hits the 目标.
			// This helps the javelin stick in a visually appealing place with在 目标 精灵.
			if (targetHitbox.Width > 8 && targetHitbox.Height > 8) {
				targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
			}
			// 返回 if the hitboxes intersects, which means the javelin collides or not
			return projHitbox.Intersects(targetHitbox);
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
			// 如果 attached to an NPC, draw behind tiles (and the npc) if that NPC is behind tiles, 否则 just behind the NPC.
			if (IsStickingToTarget) {
				int npcIndex = TargetWhoAmI;
				if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active) {
					if (Main.npc[npcIndex].behindTiles) {
						behindNPCsAndTiles.Add(index);
					}
					else {
						behindNPCsAndTiles.Add(index);
					}

					return;
				}
			}
			// Since we aren't attached, add to this 列表
			behindNPCsAndTiles.Add(index);
		}

		// 更改 this 数字 if you 想要 alter how the alpha changes
		private const int AlphaFadeInSpeed = 25;

		private void UpdateAlpha() {
			// Slowly 删除 alpha as it is present
			if (Projectile.alpha > 0) {
				Projectile.alpha -= AlphaFadeInSpeed;
			}

			// 如果 alpha gets lower than 0, set it to 0
			if (Projectile.alpha < 0) {
				Projectile.alpha = 0;
			}
		}
	}
}
