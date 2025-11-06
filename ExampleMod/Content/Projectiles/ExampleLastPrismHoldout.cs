using ExampleMod.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleLastPrismHoldout : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LastPrism;

		// vanilla Last Prism is an animated 项 with 5 frames of 动画. We 复制 that here.
		private const int NumAnimationFrames = 5;

		// This controls how m任何 individual beams are fired by the Prism.
		public const int NumBeams = 10;

		// This 值 controls how m任何 frames it takes 对于 Prism to reach "max charge". 60 frames = 1 second.
		public const float MaxCharge = 180f;

		// This 值 controls how m任何 frames it takes 对于 beams to begin dealing 伤害. Before then they can't hit 任何thing.
		public const float DamageStart = 30f;

		// This 值 controls how sluggish the Prism turns while being used. Vanilla Last Prism is 0.08f.
		// Higher values make the Prism turn faster.
		private const float AimResponsiveness = 0.08f;

		// This 值 controls how frequently the Prism emits 声音 once it's firing.
		private const int SoundInterval = 20;

		// These values place caps 在 魔力 consumption rate 的 Prism.
		// 当 first used, the Prism consumes 魔力 once 每个 MaxManaConsumptionDelay frames.
		// 每次 魔力 is consumed, the pace becomes one 帧 faster, meaning 魔力 consumption smoothly increases.
		// 当 capped out, the Prism consumes 魔力 once 每个 MinManaConsumptionDelay frames.
		private const float MaxManaConsumptionDelay = 15f;
		private const float MinManaConsumptionDelay = 5f;

		// This 属性 encloses the internal AI 变量 弹幕.ai[0]. It makes the code easier to read.
		private float FrameCounter {
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		// This 属性 encloses the internal AI 变量 弹幕.ai[1].
		private float NextManaFrame {
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		// This 属性 encloses the internal AI 变量 弹幕.localAI[0].
		// localAI is not automatically synced over the 网络, but that does not cause 任何 problems 在这种情况下.
		private float ManaConsumptionRate {
			get => Projectile.localAI[0];
			set => Projectile.localAI[0] = value;
		}

		public override void SetStaticDefaults() {
			Main.projFrames[Projectile.type] = NumAnimationFrames;

			// Signals to Terraria that this 弹幕 requires a unique 标识符 beyond its 索引 在 弹幕 数组.
			// 这防止 the issue 与 vanilla Last Prism where the beams are invisible in multiplayer.
			ProjectileID.Sets.NeedsUUID[Projectile.type] = true;

			// 防止s jitter when stepping up and down blocks and half blocks
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
		}

		public override void SetDefaults() {
			// 使用 CloneDefaults to clone all basic 弹幕 statistics 从 vanilla Last Prism.
			Projectile.CloneDefaults(ProjectileID.LastPrism);
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

			// 更新 the Prism's 伤害 每个 帧 以便 it is dynamically affected by 魔力 Sickness.
			UpdateDamageForManaSickness(player);

			// 更新 the 帧 计数器.
			FrameCounter += 1f;

			// 更新 弹幕 visuals and 声音.
			UpdateAnimation();
			PlaySounds();

			// 更新 the Prism's 位置 在 世界 and relevant variables 的 玩家 holding it.
			UpdatePlayerVisuals(player, rrp);

			// 更新 the Prism's behavior: project beams on 帧 1, consume 魔力, and despawn if out of 魔力.
			if (Projectile.owner == Main.myPlayer) {
				// Slightly re-aim the Prism 每个 帧 以便 it gradually sweeps to 点 towards the 鼠标.
				UpdateAim(rrp, player.HeldItem.shootSpeed);

				// 玩家.CheckMana returns 真 if the 魔力 成本 可以 paid. Since the second 参数 is 真, the 魔力 is actually consumed.
				// 如果 魔力 shouldn't consumed this 帧, the || operator short-circuits its evaluation 玩家.CheckMana never executes.
				bool manaIsAvailable = !ShouldConsumeMana() || player.CheckMana(player.HeldItem.mana, true, false);

				// Prism immediately stops functioning if the 玩家 is Cursed (玩家.noItems) or "Crowd Controlled", e.g. the Frozen 减益.
				// 玩家.通道 indicates whether the 玩家 is still holding down the 鼠标 按钮 to use the 项.
				bool stillInUse = player.channel && manaIsAvailable && !player.noItems && !player.CCed;

				// 生成 在 Prism's lasers 在 first 帧 if the 玩家 is capable of 使用 项.
				if (stillInUse && FrameCounter == 1f) {
					FireBeams();
				}

				// 如果 the Prism cannot 继续 to be used, then destroy it immediately.
				else if (!stillInUse) {
					Projectile.Kill();
				}
			}

			// 这确保 th在 Prism never times out while in use.
			Projectile.timeLeft = 2;
		}

		private void UpdateDamageForManaSickness(Player player) {
			Projectile.damage = (int)player.GetDamage(DamageClass.Magic).ApplyTo(player.HeldItem.damage);
		}

		private void UpdateAnimation() {
			Projectile.frameCounter++;

			// As the Prism charges up and focuses the beams, its 动画 plays faster.
			int framesPerAnimationUpdate = FrameCounter >= MaxCharge ? 2 : FrameCounter >= (MaxCharge * 0.66f) ? 3 : 4;

			// 如果 necessary, change which specific 帧 的 动画 is displayed.
			if (Projectile.frameCounter >= framesPerAnimationUpdate) {
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= NumAnimationFrames) {
					Projectile.frame = 0;
				}
			}
		}

		private void PlaySounds() {
			// Prism makes 声音 intermittently while in use, 使用 vanilla 弹幕 变量 soundDelay.
			if (Projectile.soundDelay <= 0) {
				Projectile.soundDelay = SoundInterval;

				// 在 very first 帧, the 声音 playing is skipped. This way it doesn't overlap the starting hiss 声音.
				if (FrameCounter > 1f) {
					SoundEngine.PlaySound(SoundID.Item15, Projectile.position);
				}
			}
		}

		private void UpdatePlayerVisuals(Player player, Vector2 playerHandPos) {
			// Place the Prism directly in到 玩家's hand at all times.
			Projectile.Center = playerHandPos;
			// beams emit 从 提示 的 Prism, not the side. As such, 旋转 the 精灵 by pi/2 (90 degrees).
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.spriteDirection = Projectile.direction;

			// Prism is a holdout 弹幕, so change the 玩家's variables to reflect that.
			// Constantly resetting 玩家.itemTime and 玩家.itemAnimation prevents the 玩家 from switching items or doing 任何thing else.
			player.ChangeDir(Projectile.direction);
			player.heldProj = Projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;

			// 如果 you do not multiply by 弹幕.方向, the 玩家's hand will 点 the wrong 方向 while facing 左.
			player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
		}

		private bool ShouldConsumeMana() {
			// 如果 the 魔力 consumption 计时器 hasn't been initialized yet, initialize it and consume 魔力 on 帧 1.
			if (ManaConsumptionRate == 0f) {
				NextManaFrame = ManaConsumptionRate = MaxManaConsumptionDelay;
				return true;
			}

			// Should 魔力 be consumed this 帧?
			bool consume = FrameCounter == NextManaFrame;

			// 如果 魔力 is being consumed this 帧, 更新 the rate of 魔力 consumption and write down the next 帧 魔力 将 consumed.
			if (consume) {
				// MathHelper.Clamp(X,A,B) guarantees that A <= X <= B. If X is outside the 范围, it 将 set to A or B 相应地.
				ManaConsumptionRate = MathHelper.Clamp(ManaConsumptionRate - 1f, MinManaConsumptionDelay, MaxManaConsumptionDelay);
				NextManaFrame += ManaConsumptionRate;
			}
			return consume;
		}

		private void UpdateAim(Vector2 source, float speed) {
			// 获取 the 玩家's current aiming 方向 as a normalized vector.
			Vector2 aim = Vector2.Normalize(Main.MouseWorld - source);
			if (aim.HasNaNs()) {
				aim = -Vector2.UnitY;
			}

			// 更改 a portion 的 Prism's current 速度 以便 it points 到 鼠标. This gives smooth movement over 时间.
			aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, AimResponsiveness));
			aim *= speed;

			if (aim != Projectile.velocity) {
				Projectile.netUpdate = true;
			}
			Projectile.velocity = aim;
		}

		private void FireBeams() {
			// 如果 for some reas在 beam 速度 can't be correctly normalized, set it to a default 值.
			Vector2 beamVelocity = Vector2.Normalize(Projectile.velocity);
			if (beamVelocity.HasNaNs()) {
				beamVelocity = -Vector2.UnitY;
			}

			// This 通用唯一标识符 将 the same between all players in multiplayer, ensuring th在 beams are properly anchored 在 Prism on 每个one's 屏幕.
			int uuid = Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI);

			int damage = Projectile.damage;
			float knockback = Projectile.knockBack;
			for (int b = 0; b < NumBeams; ++b) {
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, beamVelocity, ModContent.ProjectileType<ExampleLastPrismBeam>(), damage, knockback, Projectile.owner, b, uuid);
			}

			// 之后 creating the beams, mark the Prism as having an important 网络 事件. This will make Terraria 同步 its 数据 to other players ASAP.
			Projectile.netUpdate = true;
		}

		// Because the Prism is a holdout 弹幕 and stays glued to its 用户, it needs custom drawcode.
		public override bool PreDraw(ref Color lightColor) {
			SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int spriteSheetOffset = frameHeight * Projectile.frame;
			Vector2 sheetInsertPosition = (Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();

			// Prism is always at full brightness, regardless 的 surrounding light. This is equivalent to it being its own glowmask.
			// 它是 drawn in a non-white 颜色 to distinguish it 从 vanilla Last Prism.
			Color drawColor = ExampleLastPrism.OverrideColor;
			Main.EntitySpriteDraw(texture, sheetInsertPosition, new Rectangle?(new Rectangle(0, spriteSheetOffset, texture.Width, frameHeight)), drawColor, Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, effects, 0f);
			return false;
		}
	}
}