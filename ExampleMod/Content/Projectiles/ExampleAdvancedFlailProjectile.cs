using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	// 示例 Advanced Flail is a complete adaption of Ball O' Hurt 弹幕. The code has been rewritten a bit to make it easier to follow. 比较 this code against the decompiled Terraria code for an example of adapting vanilla code. A few comments and extra code snippets show features from other vanilla flails 以及.
	// 示例 Advanced Flail shows a plethora of advanced AI and collision topics.
	// 参见 ExampleFlail for a simpler but less customizable flail 弹幕 example.
	public class ExampleAdvancedFlailProjectile : ModProjectile
	{
		private const string ChainTexturePath = "ExampleMod/Content/Projectiles/ExampleAdvancedFlailProjectileChain"; // The 文件夹 路径 到 flail chain 精灵
		private const string ChainTextureExtraPath = "ExampleMod/Content/Projectiles/ExampleAdvancedFlailProjectileChainExtra";  // This 纹理 and related code is optional and used for a unique 效果

		private static Asset<Texture2D> chainTexture;
		private static Asset<Texture2D> chainTextureExtra; // This 纹理 and related code is optional and used for a unique 效果

		private enum AIState
		{
			Spinning,
			LaunchingForward,
			Retracting,
			UnusedState,
			ForcedRetracting,
			Ricochet,
			Dropping
		}

		// These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
		private AIState CurrentAIState {
			get => (AIState)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}
		public ref float StateTimer => ref Projectile.ai[1];
		public ref float CollisionCounter => ref Projectile.localAI[0];
		public ref float SpinningStateTimer => ref Projectile.localAI[1];

		public override void Load() {
			chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);
			chainTextureExtra = ModContent.Request<Texture2D>(ChainTextureExtraPath);
		}

		public override void SetStaticDefaults() {
			// These lines facilitate the trail drawing
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.netImportant = true; // This ensures th在 弹幕 is synced when other players jo在 世界.
			Projectile.width = 24; // The 宽度 of your 弹幕
			Projectile.height = 24; // The 高度 of your 弹幕
			Projectile.friendly = true; // Deals 伤害 to enemies
			Projectile.penetrate = -1; // Infinite pierce
			Projectile.DamageType = DamageClass.Melee; // Deals melee 伤害
			Projectile.usesLocalNPCImmunity = true; // 使用d for hit cooldown changes 在 ai hook
			Projectile.localNPCHitCooldown = 10; // This facilitates custom hit cooldown logic

			// Vanilla flails all use aiStyle 15, but the code isn't customizable so an adaption of that aiStyle is used 在 AI 方法
		}

		// This AI code was adapted from vanilla code: Terraria.弹幕.AI_015_Flails() 
		public override void AI() {
			Player player = Main.player[Projectile.owner];
			// Kill the 弹幕 if the 玩家 dies or gets crowd controlled
			if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f) {
				Projectile.Kill();
				return;
			}
			if (Main.myPlayer == Projectile.owner && Main.mapFullscreen) {
				Projectile.Kill();
				return;
			}

			Vector2 mountedCenter = player.MountedCenter;
			bool doFastThrowDust = false;
			bool shouldOwnerHitCheck = false;
			int launchTimeLimit = 15;  // How much 时间 the 弹幕 can go before retracting (速度 and shootTimer will set the flail's 范围)
			float launchSpeed = 14f; // How fast the 弹幕 can 移动
			float maxLaunchLength = 800f; // How far the 弹幕's chain can stretch before being forced to retract when in launched 状态
			float retractAcceleration = 3f; // How quickly the 弹幕 will accelerate back towards the 玩家 while retracting
			float maxRetractSpeed = 10f; // The max 速度 the 弹幕 will have while retracting
			float forcedRetractAcceleration = 6f; // How quickly the 弹幕 will accelerate back towards the 玩家 while being forced to retract
			float maxForcedRetractSpeed = 15f; // The max 速度 the 弹幕 will have while being forced to retract
			float unusedRetractAcceleration = 1f;
			float unusedMaxRetractSpeed = 14f;
			int unusedChainLength = 60;
			int defaultHitCooldown = 10; // How often your flail hits when resting 在 ground, or retracting
			int spinHitCooldown = 20; // How often your flail hits when spinning
			int movingHitCooldown = 10; // How often your flail hits when moving
			int ricochetTimeLimit = launchTimeLimit + 5;

			// Scaling these speeds and accelerations by the players melee 速度 makes the 武器 more responsive if the 玩家 boosts it or general 武器 速度
			float meleeSpeedMultiplier = player.GetTotalAttackSpeed(DamageClass.Melee);
			launchSpeed *= meleeSpeedMultiplier;
			unusedRetractAcceleration *= meleeSpeedMultiplier;
			unusedMaxRetractSpeed *= meleeSpeedMultiplier;
			retractAcceleration *= meleeSpeedMultiplier;
			maxRetractSpeed *= meleeSpeedMultiplier;
			forcedRetractAcceleration *= meleeSpeedMultiplier;
			maxForcedRetractSpeed *= meleeSpeedMultiplier;
			float launchRange = launchSpeed * launchTimeLimit;
			float maxDroppedRange = launchRange + 160f;
			Projectile.localNPCHitCooldown = defaultHitCooldown;

			switch (CurrentAIState) {
				case AIState.Spinning: {
						shouldOwnerHitCheck = true;
						if (Projectile.owner == Main.myPlayer) {
							Vector2 unitVectorTowardsMouse = mountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX * player.direction);
							player.ChangeDir((unitVectorTowardsMouse.X > 0f).ToDirectionInt());
							if (!player.channel) // If the 玩家 releases then change to moving forward 模式
							{
								CurrentAIState = AIState.LaunchingForward;
								StateTimer = 0f;
								Projectile.velocity = unitVectorTowardsMouse * launchSpeed + player.velocity;
								Projectile.Center = mountedCenter;
								Projectile.netUpdate = true;
								Projectile.ResetLocalNPCHitImmunity();
								Projectile.localNPCHitCooldown = movingHitCooldown;
								break;
							}
						}
						SpinningStateTimer += 1f;
						// This line creates a unit vector 即 constantly rotated around the 玩家. 10f controls how fast the 弹幕 visually spins around the 玩家
						Vector2 offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 10f * (SpinningStateTimer / 60f) * player.direction);

						offsetFromPlayer.Y *= 0.8f;
						if (offsetFromPlayer.Y * player.gravDir > 0f) {
							offsetFromPlayer.Y *= 0.5f;
						}
						Projectile.Center = mountedCenter + offsetFromPlayer * 30f + new Vector2(0, player.gfxOffY);
						Projectile.velocity = Vector2.Zero;
						Projectile.localNPCHitCooldown = spinHitCooldown; // 设置 the hit 速度 到 spinning hit 速度
						break;
					}
				case AIState.LaunchingForward: {
						doFastThrowDust = true;
						bool shouldSwitchToRetracting = StateTimer++ >= launchTimeLimit;
						shouldSwitchToRetracting |= Projectile.Distance(mountedCenter) >= maxLaunchLength;
						if (player.controlUseItem) // If the 玩家 clicks, transition 到 Dropping 状态
						{
							CurrentAIState = AIState.Dropping;
							StateTimer = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.2f;
							// 这是 where Drippler Crippler spawns its 弹幕
							/*
							if (Main.myPlayer == Projectile.owner)
								Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Projectile.velocity, 928, Projectile.damage, Projectile.knockBack, Main.myPlayer);
							*/
							break;
						}
						if (shouldSwitchToRetracting) {
							CurrentAIState = AIState.Retracting;
							StateTimer = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.3f;
							// 这是 also where Drippler Crippler spawns its 弹幕, see above code.
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
						Projectile.localNPCHitCooldown = movingHitCooldown;
						break;
					}
				case AIState.Retracting: {
						Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
						if (Projectile.Distance(mountedCenter) <= maxRetractSpeed) {
							Projectile.Kill(); // Kill the 弹幕 once it is 关闭 enough 到 玩家
							return;
						}
						if (player.controlUseItem) // If the 玩家 clicks, transition 到 Dropping 状态
						{
							CurrentAIState = AIState.Dropping;
							StateTimer = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.2f;
						}
						else {
							Projectile.velocity *= 0.98f;
							Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxRetractSpeed, retractAcceleration);
							player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
						}
						break;
					}
				case AIState.UnusedState: // 弹幕.ai[0] == 3; This case is actually unused, but maybe a Terraria 更新 will add it back in, or maybe it is useless, so I 左 it here.
					{
						if (!player.controlUseItem) {
							CurrentAIState = AIState.ForcedRetracting; // 移动 to super retracting 模式 if the 玩家 taps
							StateTimer = 0f;
							Projectile.netUpdate = true;
							break;
						}
						float currentChainLength = Projectile.Distance(mountedCenter);
						Projectile.tileCollide = StateTimer == 1f;
						bool flag3 = currentChainLength <= launchRange;
						if (flag3 != Projectile.tileCollide) {
							Projectile.tileCollide = flag3;
							StateTimer = Projectile.tileCollide ? 1 : 0;
							Projectile.netUpdate = true;
						}
						if (currentChainLength > unusedChainLength) {

							if (currentChainLength >= launchRange) {
								Projectile.velocity *= 0.5f;
								Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedMaxRetractSpeed);
							}
							Projectile.velocity *= 0.98f;
							Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedRetractAcceleration);
						}
						else {
							if (Projectile.velocity.Length() < 6f) {
								Projectile.velocity.X *= 0.96f;
								Projectile.velocity.Y += 0.2f;
							}
							if (player.velocity.X == 0f) {
								Projectile.velocity.X *= 0.96f;
							}
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
						break;
					}
				case AIState.ForcedRetracting:
					{
						Projectile.tileCollide = false;
						Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
						if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed) {
							Projectile.Kill(); // Kill the 弹幕 once it is 关闭 enough 到 玩家
							return;
						}
						Projectile.velocity *= 0.98f;
						Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
						Vector2 target = Projectile.Center + Projectile.velocity;
						Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
						if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f) {
							Projectile.Kill(); // Kill 弹幕 if it will pass the 玩家
							return;
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
						break;
					}
				case AIState.Ricochet:
					if (StateTimer++ >= ricochetTimeLimit) {
						CurrentAIState = AIState.Dropping;
						StateTimer = 0f;
						Projectile.netUpdate = true;
					}
					else {
						Projectile.localNPCHitCooldown = movingHitCooldown;
						Projectile.velocity.Y += 0.6f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
					}
					break;
				case AIState.Dropping:
					if (!player.controlUseItem || Projectile.Distance(mountedCenter) > maxDroppedRange) {
						CurrentAIState = AIState.ForcedRetracting;
						StateTimer = 0f;
						Projectile.netUpdate = true;
					}
					else {
						Projectile.velocity.Y += 0.8f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
					}
					break;
			}

			// 这是 where Flower Pow launches projectiles. Decompile Terraria to 视图 that code.

			Projectile.direction = (Projectile.velocity.X > 0f).ToDirectionInt();
			Projectile.spriteDirection = Projectile.direction;
			Projectile.ownerHitCheck = shouldOwnerHitCheck; // This prevents attempting to 伤害 enemies without line of sight 到 玩家. The custom Colliding code for spinning makes this necessary.

			// This 旋转 code is unique to this flail, since the 精灵 isn't rotationally symmetric and has 提示.
			bool freeRotation = CurrentAIState == AIState.Ricochet || CurrentAIState == AIState.Dropping;
			if (freeRotation) {
				if (Projectile.velocity.Length() > 1f)
					Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
				else
					Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
			}
			else {
				Vector2 vectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
				Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;
			}

			// 如果 you have a ball shaped flail, you can use this simplified 旋转 code instead
			/*
			if (Projectile.velocity.Length() > 1f)
				Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
			else
				Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
			*/

			Projectile.timeLeft = 2; // 使 sure the flail doesn't die (good when the flail is resting 在 ground)
			player.heldProj = Projectile.whoAmI;
			player.SetDummyItemTime(2); //Add a 延迟 so the 玩家 can't 按钮 mash the flail
			player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
			if (Projectile.Center.X < mountedCenter.X) {
				player.itemRotation += (float)Math.PI;
			}
			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

			// 生成ing dust. We 生成 dust more often when 在 LaunchingForward 状态
			int dustRate = 15;
			if (doFastThrowDust)
				dustRate = 1;

			if (Main.rand.NextBool(dustRate))
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), 0f, 0f, 150, default(Color), 1.3f);
		}

		public override bool OnTileCollide(Vector2 oldVelocity) {
			int defaultLocalNPCHitCooldown = 10;
			int impactIntensity = 0;
			Vector2 velocity = Projectile.velocity;
			float bounceFactor = 0.2f;
			if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Ricochet) {
				bounceFactor = 0.4f;
			}

			if (CurrentAIState == AIState.Dropping) {
				bounceFactor = 0f;
			}

			if (oldVelocity.X != Projectile.velocity.X) {
				if (Math.Abs(oldVelocity.X) > 4f) {
					impactIntensity = 1;
				}

				Projectile.velocity.X = (0f - oldVelocity.X) * bounceFactor;
				CollisionCounter += 1f;
			}

			if (oldVelocity.Y != Projectile.velocity.Y) {
				if (Math.Abs(oldVelocity.Y) > 4f) {
					impactIntensity = 1;
				}

				Projectile.velocity.Y = (0f - oldVelocity.Y) * bounceFactor;
				CollisionCounter += 1f;
			}

			// 如果 在 Launched 状态, 生成 sparks
			if (CurrentAIState == AIState.LaunchingForward) {
				CurrentAIState = AIState.Ricochet;
				Projectile.localNPCHitCooldown = defaultLocalNPCHitCooldown;
				Projectile.netUpdate = true;
				Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
				Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
				impactIntensity = 2;
				Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);
				Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, velocity);
				Projectile.position -= velocity;
			}

			// 在这里 the tiles 生成 dust indicating they've been hit
			if (impactIntensity > 0) {
				Projectile.netUpdate = true;
				for (int i = 0; i < impactIntensity; i++) {
					Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
				}

				SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			}

			// Force retraction if stuck on tiles while retracting
			if (CurrentAIState != AIState.UnusedState && CurrentAIState != AIState.Spinning && CurrentAIState != AIState.Ricochet && CurrentAIState != AIState.Dropping && CollisionCounter >= 10f) {
				CurrentAIState = AIState.ForcedRetracting;
				Projectile.netUpdate = true;
			}

			// tModLoader currently does not provide the wetVelocity 参数, this code should make the flail bounce back faster when colliding with tiles underwater.
			//if (弹幕.wet)
			//	wetVelocity = 弹幕.速度;

			return false;
		}

		public override bool? CanDamage() {
			// Flails in spin 模式 won't 伤害 enemies with在 first 12 ticks. Visually this delays the first hit until the 玩家 swings the flail around for a full spin before damaging anything.
			if (CurrentAIState == AIState.Spinning && SpinningStateTimer <= 12f) {
				return false;
			}
			return base.CanDamage();
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			// Flails do special collision logic that serves to hit anything within an ellipse centered 在 玩家 when the flail is spinning around the 玩家. 例如, the 弹幕 rotating around the 玩家 won't actually hit a bee if it is directly 在 玩家 usually, but this code ensures th在 bee is hit. This code makes hitting enemies while spinning more consistent and not reliant 的 actual 位置 的 flail 弹幕.
			if (CurrentAIState == AIState.Spinning) {
				Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
				Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
				shortestVectorFromPlayerToTarget.Y /= 0.8f; // 使 the hit 区域 an ellipse. Vertical hit 距离 is smaller due to this math.
				float hitRadius = 55f; // The 长度 的 semi-major radius 的 ellipse (the long 结束)
				return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
			}
			// Regular collision logic happens otherwise.
			return base.Colliding(projHitbox, targetHitbox);
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
			// Flails do a few custom things, you'll want to keep these to have the same feel as vanilla flails.

			// Flails do 20% more 伤害 while spinning
			if (CurrentAIState == AIState.Spinning) {
				modifiers.SourceDamage *= 1.2f;
			}
			// Flails do 100% more 伤害 while launched or retracting. This is the 伤害 the 项 工具提示 for flails aim to 匹配, as this is the most common 模式 of 攻击. This is why the 项 has ItemID.Sets.ToolTipDamageMultiplier[类型] = 2f;
			else if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Retracting) {
				modifiers.SourceDamage *= 2f;
			}

			// hitDirection is always set to hit away 从 玩家, even if the flail damages the npc while returning
			modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();

			// Knockback is only 25% as powerful when in spin 模式
			if (CurrentAIState == AIState.Spinning) {
				modifiers.Knockback *= 0.25f;
			}
			// Knockback is only 50% as powerful when in 放下 down 模式
			else if (CurrentAIState == AIState.Dropping) {
				modifiers.Knockback *= 0.5f;
			}
		}

		// PreDraw is used to draw a chain and trail before the 弹幕 is drawn normally.
		public override bool PreDraw(ref Color lightColor) {
			Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

			// This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This 应该 removed once the vanilla bug is fixed.
			playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

			Rectangle? chainSourceRectangle = null;
			// Drippler Crippler customizes sourceRectangle to 循环 through 精灵 frames: sourceRectangle = asset.帧(1, 6);
			float chainHeightAdjustment = 0f; // 使用 this to adjust the chain overlap. 

			Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
			Vector2 chainDrawPosition = Projectile.Center;
			Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
			Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
			float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
			if (chainSegmentLength == 0) {
				chainSegmentLength = 10; // When the chain 纹理 is being loaded, the 高度 is 0 which would cause infinite loops.
			}
			float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
			int chainCount = 0;
			float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

			// This while 循环 draws the chain 纹理 从 弹幕 到 玩家, looping to draw the chain 纹理 along the 路径
			while (chainLengthRemainingToDraw > 0f) {
				// This code gets the lighting 在 current 图格 coordinates
				Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

				// Flaming Mace and Drippler Crippler use code here to draw custom 精灵 frames with custom lighting.
				// Cycling through frames: sourceRectangle = asset.帧(1, 6, 0, chainCount % 6);
				// 此示例 shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw 颜色 at different values

				var chainTextureToDraw = chainTexture;
				if (chainCount >= 4) {
					// 使用 normal chainTexture and lighting, no changes
				}
				else if (chainCount >= 2) {
					// Near 到 ball, we draw a custom chain 纹理 and slightly make it glow if unlit.
					chainTextureToDraw = chainTextureExtra;
					byte minValue = 140;
					if (chainDrawColor.R < minValue)
						chainDrawColor.R = minValue;

					if (chainDrawColor.G < minValue)
						chainDrawColor.G = minValue;

					if (chainDrawColor.B < minValue)
						chainDrawColor.B = minValue;
				}
				else {
					// 关闭 到 ball, we draw a custom chain 纹理 and draw it at full brightness glow.
					chainTextureToDraw = chainTextureExtra;
					chainDrawColor = Color.White;
				}

				// Here, we draw the chain 纹理 在 coordinates
				Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

				// chainDrawPosition is advanced along the vector back 到 玩家 by the chainSegmentLength
				chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
				chainCount++;
				chainLengthRemainingToDraw -= chainSegmentLength;
			}

			// 添加 a motion trail when moving forward, like most flails do (don't add trail if already hit a 图格)
			if (CurrentAIState == AIState.LaunchingForward) {
				Texture2D projectileTexture = TextureAssets.Projectile[Type].Value;
				Vector2 drawOrigin = new Vector2(projectileTexture.Width * 0.5f, Projectile.height * 0.5f);
				SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
				for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++) {
					Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
					Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
				}
			}
			return true;
		}
	}
}
