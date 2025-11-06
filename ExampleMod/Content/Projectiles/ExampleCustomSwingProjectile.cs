using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	// 示例CustomSwingSword is an example of a sword with a custom swing using a held 弹幕
	// 这是 great if you want to make melee weapons with complex swing behavior
	// 注意 that this 弹幕 only covers 2 relatively simple swings, everything else is up to you
	// Aside 从 custom 动画, the custom collision code in Colliding is very important to this 武器
	public class ExampleCustomSwingProjectile : ModProjectile
	{
		// 我们 define some constants that determine the swing 范围 的 sword
		// Not that we use multipliers here since that simplifies the amount of tweaks 对于se interactions
		// 你 could change the values or even 替换 them entirely, but they are tweaked with looks in mind
		private const float SWINGRANGE = 1.67f * (float)Math.PI; // The 角度 a swing 攻击 covers (300 deg)
		private const float FIRSTHALFSWING = 0.45f; // How much 的 swing happens before it reaches the 目标 角度 (in 关系 to swingRange)
		private const float SPINRANGE = 3.5f * (float)Math.PI; // The 角度 a spin 攻击 covers (630 degrees)
		private const float WINDUP = 0.15f; // How far back the 玩家's hand goes when winding their 攻击 (in 关系 to swingRange)
		private const float UNWIND = 0.4f; // When should the sword 开始 disappearing
		private const float SPINTIME = 2.5f; // How much longer a spin is than a swing

		private enum AttackType // Which 攻击 is being performed
		{
			// Swings are normal sword swings that 可以 slightly aimed
			// Swings goes through the full 循环 of animations
			Swing,
			// Spins are swings that go full circle
			// They are slower and deal more knockback
			Spin,
		}

		private enum AttackStage // What 阶段 的 攻击 is being executed, see functions found in AI for 描述
		{
			Prepare,
			Execute,
			Unwind
		}

		// These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
		private AttackType CurrentAttack {
			get => (AttackType)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}

		private AttackStage CurrentStage {
			get => (AttackStage)Projectile.localAI[0];
			set {
				Projectile.localAI[0] = (float)value;
				Timer = 0; // 重置 the 计时器 when the 弹幕 switches states
			}
		}

		// Variables to keep 跟踪 of during runtime
		private ref float InitialAngle => ref Projectile.ai[1]; // 角度 aimed in (with constraints)
		private ref float Timer => ref Projectile.ai[2]; // 计时器 to keep 跟踪 of progression of each 阶段
		private ref float Progress => ref Projectile.localAI[1]; // 位置 of sword relative to initial 角度
		private ref float Size => ref Projectile.localAI[2]; // 大小 of sword

		// 我们 define timing functions for each 阶段, taking into account melee 攻击 速度
		// 注意 that you can change this to suit the need of your 弹幕
		private float prepTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float execTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float hideTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

		public override string Texture => "ExampleMod/Content/Items/Weapons/ExampleCustomSwingSword"; // 使用 纹理 of 项 as 弹幕 纹理
		private Player Owner => Main.player[Projectile.owner];

		public override void SetStaticDefaults() {
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 46; // Hitbox 宽度 of 弹幕
			Projectile.height = 48; // Hitbox 高度 of 弹幕
			Projectile.friendly = true; // 弹幕 hits enemies
			Projectile.timeLeft = 10000; // 时间 it takes for 弹幕 to expire
			Projectile.penetrate = -1; // 弹幕 pierces infinitely
			Projectile.tileCollide = false; // 弹幕 does not collide with tiles
			Projectile.usesLocalNPCImmunity = true; // 使用s local immunity frames
			Projectile.localNPCHitCooldown = -1; // We set this to -1 to make sure the 弹幕 doesn't hit twice
			Projectile.ownerHitCheck = true; // 使 sure the 所有者 的 弹幕 has line of sight 到 目标 (aka can't hit things through 图格).
			Projectile.DamageType = DamageClass.Melee; // 弹幕 is a melee 弹幕
		}

		public override void OnSpawn(IEntitySource source) {
			Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
			float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();

			if (CurrentAttack == AttackType.Spin) {
				InitialAngle = (float)(-Math.PI / 2 - Math.PI * 1 / 3 * Projectile.spriteDirection); // 对于 spin, starting 角度 is designated based on 方向 of hit
			}
			else {
				if (Projectile.spriteDirection == 1) {
					// 然而, we 限制 the rangle of possible directions so it does not look too ridiculous
					targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
				}
				else {
					if (targetAngle < 0) {
						targetAngle += 2 * (float)Math.PI; // This makes the 范围 continuous for easier operations
					}

					targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
				}

				InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection; // 否则, we calculate the 角度
			}
		}

		public override void SendExtraAI(BinaryWriter writer) {
			// 弹幕.spriteDirection for this 弹幕 is derived 从 鼠标 位置 的 所有者 in OnSpawn, as such it needs to be synced. spriteDirection is not one 的 fields automatically synced over the 网络. All 弹幕.ai slots are used already, so we will 同步 it manually. 
			writer.Write((sbyte)Projectile.spriteDirection);
		}

		public override void ReceiveExtraAI(BinaryReader reader) {
			Projectile.spriteDirection = reader.ReadSByte();
		}

		public override void AI() {
			// Extend use 动画 until 弹幕 is killed
			Owner.itemAnimation = 2;
			Owner.itemTime = 2;

			// Kill the 弹幕 if the 玩家 dies or gets crowd controlled
			if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed) {
				Projectile.Kill();
				return;
			}

			// AI depends on 阶段 and 攻击
			// 注意 th在se stages are to facilitate the scaling 效果 在 beginning and 结束
			// 如果 this is not desirable for you, feel free to simplify
			switch (CurrentStage) {
				case AttackStage.Prepare:
					PrepareStrike();
					break;
				case AttackStage.Execute:
					ExecuteStrike();
					break;
				default:
					UnwindStrike();
					break;
			}

			SetSwordPosition();
			Timer++;
		}

		public override bool PreDraw(ref Color lightColor) {
			// 计算 原点 of sword (hilt) based on orientation and 偏移 sword 旋转 (as sword is angled in its 精灵)
			Vector2 origin;
			float rotationOffset;
			SpriteEffects effects;

			if (Projectile.spriteDirection > 0) {
				origin = new Vector2(0, Projectile.height);
				rotationOffset = MathHelper.ToRadians(45f);
				effects = SpriteEffects.None;
			}
			else {
				origin = new Vector2(Projectile.width, Projectile.height);
				rotationOffset = MathHelper.ToRadians(135f);
				effects = SpriteEffects.FlipHorizontally;
			}

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

			// Since we are doing a custom draw, 防止 it from normally drawing
			return false;
		}

		// 查找 the 开始 and 结束 的 sword and use a line collider to check for collision with enemies
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
			float collisionPoint = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
		}

		// Do a similar collision check for tiles
		public override void CutTiles() {
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
		}

		// 我们 make it so th在 弹幕 can only do 伤害 in its release and unwind phases
		public override bool? CanDamage() {
			if (CurrentStage == AttackStage.Prepare)
				return false;
			return base.CanDamage();
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
			// 使 knockback go away from 玩家
			modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;

			// 如果 the NPC is hit by the spin 攻击, increase knockback slightly
			if (CurrentAttack == AttackType.Spin)
				modifiers.Knockback += 1;
		}

		// 函数 to easily set 弹幕 and arm 位置
		public void SetSwordPosition() {
			Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress; // 设置 弹幕 旋转

			// 设置 composite arm allows you to set the 旋转 的 arm and stretch 的 front and back arms independently
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // 设置 arm 位置 (90 degree 偏移 since arm starts lowered)
			Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // 获取 位置 of hand

			armPosition.Y += Owner.gfxOffY;
			Projectile.Center = armPosition; // 设置 弹幕 to arm 位置
			Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly 缩放 up the 弹幕 and also take into account melee 大小 modifiers

			Owner.heldProj = Projectile.whoAmI; // 设置 held 弹幕 to this 弹幕
		}

		// 函数 facilitating the taking out 的 sword
		private void PrepareStrike() {
			Progress = WINDUP * SWINGRANGE * (1f - Timer / prepTime); // 计算s 旋转 from initial 角度
			Size = MathHelper.SmoothStep(0, 1, Timer / prepTime); // 使 sword slowly increase in 大小 as we prepare to strike until it reaches max

			if (Timer >= prepTime) {
				SoundEngine.PlaySound(SoundID.Item1); // Play sword 声音 here since playing it on 生成 is too early
				CurrentStage = AttackStage.Execute; // If 攻击 is over prep 时间, we go to next 阶段
			}
		}

		// 函数 facilitating the first half 的 swing
		private void ExecuteStrike() {
			if (CurrentAttack == AttackType.Swing) {
				Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) * Timer / execTime);

				if (Timer >= execTime) {
					CurrentStage = AttackStage.Unwind;
				}
			}
			else {
				Progress = MathHelper.SmoothStep(0, SPINRANGE, (1f - UNWIND / 2) * Timer / (execTime * SPINTIME));

				if (Timer == (int)(execTime * SPINTIME * 3 / 4)) {
					SoundEngine.PlaySound(SoundID.Item1); // Play sword 声音 again
					Projectile.ResetLocalNPCHitImmunity(); // 重置 the local npc hit immunity for second half of spin
				}

				if (Timer >= execTime * SPINTIME) {
					CurrentStage = AttackStage.Unwind;
				}
			}
		}

		// 函数 facilitating the latter half 的 swing where the sword disappears
		private void UnwindStrike() {
			if (CurrentAttack == AttackType.Swing) {
				Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) + UNWIND * Timer / hideTime);
				Size = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime); // 使 sword slowly decrease in 大小 as we 结束 the swing to make a smooth hiding 动画

				if (Timer >= hideTime) {
					Projectile.Kill();
				}
			}
			else {
				Progress = MathHelper.SmoothStep(0, SPINRANGE, (1f - UNWIND / 2) + UNWIND / 2 * Timer / (hideTime * SPINTIME / 2));
				Size = 1f - MathHelper.SmoothStep(0, 1, Timer / (hideTime * SPINTIME / 2));

				if (Timer >= hideTime * SPINTIME / 2) {
					Projectile.Kill();
				}
			}
		}
	}
}