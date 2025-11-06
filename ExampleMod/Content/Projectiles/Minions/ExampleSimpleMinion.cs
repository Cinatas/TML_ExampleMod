using ExampleMod.Content.Items;
using ExampleMod.Content.Tiles.Furniture;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles.Minions
{
	// 此文件 contains all the code necessary for a 仆从
	// - ModItem - the 武器 which you use to summ在 仆从 with
	// - ModBuff - the 图标 you can 点击 on to despawn the 仆从
	// - ModProjectile - the 仆从 itself

	// 它是 not recommended to put all these classes 在 same 文件. For demonstrations sake they are all compacted together so you get a better overview.
	// 要 get a better understanding of how 每个thing works together, and how to code 仆从 AI, read the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-仆从-Guide
	// 这是 NOT an in-depth guide to advanced 仆从 AI

	public class ExampleSimpleMinionBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true; // This 增益 won't 保存 when you 退出 the 世界
			Main.buffNoTimeDisplay[Type] = true; // The 时间 remaining won't 显示 on this 增益
		}

		public override void Update(Player player, ref int buffIndex) {
			// 如果 the minions exist 重置 the 增益 时间, 否则 删除 the 增益 从 玩家
			if (player.ownedProjectileCounts[ModContent.ProjectileType<ExampleSimpleMinion>()] > 0) {
				player.buffTime[buffIndex] = 18000;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	public class ExampleSimpleMinionItem : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the 玩家 目标 任何where 在 whole 屏幕 while using a controller
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

			ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f; // 默认的 值 is 1, but other values are supported. See the docs f或更多 guidance. 
		}

		public override void SetDefaults() {
			Item.damage = 30;
			Item.knockBack = 3f;
			Item.mana = 10; // 魔力 成本
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing; // how the 玩家's arm moves when 使用 项
			Item.value = Item.sellPrice(gold: 30);
			Item.rare = ItemRarityID.Cyan;
			Item.UseSound = SoundID.Item44; // What 声音 should play when 使用 项

			// These below are needed for a 仆从 武器
			Item.noMelee = true; // this 项 doesn't do 任何 melee 伤害
			Item.DamageType = DamageClass.Summon; // 使 the 伤害 register as summon. If your 项 does not have 任何 伤害 类型, it becomes 真 伤害 (which means that 伤害 scalars will not affect it). Be sure to have a 伤害 类型
			Item.buffType = ModContent.BuffType<ExampleSimpleMinionBuff>();
			// No buffTime because 否则 the 项 工具提示 would say something like "1 minute 持续时间"
			Item.shoot = ModContent.ProjectileType<ExampleSimpleMinion>(); // This 项 creates the 仆从 弹幕
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// 在这里 you can change where the 仆从 is spawned. Most vanilla minions 生成 在 cursor 位置
			position = Main.MouseWorld;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// 这是 needed so the 增益 that keeps your 仆从 alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			// Minions 必须 be spawned manually, then have originalDamage assigned 到 伤害 的 summon 项
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			// Since we spawned the 弹幕 manually already, we do not need 游戏 to 生成 it for ourselves 任何more, so 返回 假
			return false;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<ExampleItem>())
				.AddTile(ModContent.TileType<ExampleWorkbench>())
				.Register();
		}
	}

	// This 仆从 shows 一些 mandatory things that make it behave properly.
	// Its 攻击 pattern is simple: If an 敌人 is in 范围 of 43 tiles, it will fly to it and deal contact 伤害
	// 如果 the 玩家 targets a certain NPC with 右-点击, it will fly through tiles to it
	// 如果 it isn't attacking, it will float near the 玩家 with minimal movement
	public class ExampleSimpleMinion : ModProjectile
	{
		public override void SetStaticDefaults() {
			// 设置s the amount of frames this 仆从 has on its spritesheet
			Main.projFrames[Projectile.type] = 4;
			// 这是 necessary for 右-点击 targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			Main.projPet[Projectile.type] = true; // Denotes that this 弹幕 is a 宠物 or 仆从

			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your 仆从 can properly 生成 when summoned and replaced when other minions are summoned
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // 使 the cultist resistant to this 弹幕, as it's resistant to all homing projectiles.
		}

		public sealed override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 28;
			Projectile.tileCollide = false; // 使 the 仆从 go through tiles freely

			// These below are needed for a 仆从 武器
			Projectile.friendly = true; // 仅 controls if it deals 伤害 to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a 仆从 (has m任何 effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the 伤害 类型 (needed for it to deal 伤害)
			Projectile.minionSlots = 1f; // Amount of slots this 仆从 occupies 从 total 仆从 slots available 到 玩家 (more on that later)
			Projectile.penetrate = -1; // Needed so the 仆从 doesn't despawn on collision with enemies or tiles
		}

		// 在这里 you can decide if your 仆从 breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		// 这是 mandatory if your 仆从 deals contact 伤害 (further related stuff in AI() 在 Movement 区域)
		public override bool MinionContactDamage() {
			return true;
		}

		// AI of this 仆从 is 拆分 into 多个 methods to avoid bloat. This 方法 just passes values between calls actual parts 的 AI.
		public override void AI() {
			Player owner = Main.player[Projectile.owner];

			if (!CheckActive(owner)) {
				return;
			}

			GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
			Visuals();
		}

		// 这是 the "active check", makes sure the 仆从 is alive while the 玩家 is alive, and despawns 如果不是
		private bool CheckActive(Player owner) {
			if (owner.dead || !owner.active) {
				owner.ClearBuff(ModContent.BuffType<ExampleSimpleMinionBuff>());

				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<ExampleSimpleMinionBuff>())) {
				Projectile.timeLeft = 2;
			}

			return true;
		}

		private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition) {
			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles 从 中心 的 玩家)

			// 如果 your 仆从 doesn't aimlessly 移动 around when it's idle, you 需要 "put" it in到 line of other summoned minions
			// 索引 is 弹幕.minionPos
			float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
			idlePosition.X += minionPositionOffsetX; // Go behind the 玩家

			// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

			// 传送 to 玩家 if 距离 is too big
			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f) {
				// Whenever you deal with non-regular events that change the behavior or 位置 drastically, 确保 to only run the code 在 所有者 的 弹幕,
				// 然后 set netUpdate to 真
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}

			// 如果 your 仆从 is flying, you 想要 do this independently of 任何 conditions
			float overlapVelocity = 0.04f;

			// Fix overlap with other minions
			foreach (var other in Main.ActiveProjectiles) {
				if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width) {
					if (Projectile.position.X < other.position.X) {
						Projectile.velocity.X -= overlapVelocity;
					}
					else {
						Projectile.velocity.X += overlapVelocity;
					}

					if (Projectile.position.Y < other.position.Y) {
						Projectile.velocity.Y -= overlapVelocity;
					}
					else {
						Projectile.velocity.Y += overlapVelocity;
					}
				}
			}
		}

		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) {
			// Starting 搜索 距离
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;

			// This code is required if your 仆从 武器 has the targeting feature
			if (owner.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);

				// Reasonable 距离 away so it doesn't 目标 across 多个 screens
				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget) {
				// This code is required 任一 way, used for finding a 目标
				foreach (var npc in Main.ActiveNPCs) {
					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						// 添加itional check for this specific 仆从 behavior, 否则 it will 停止 attacking once it dashed through an 敌人 while flying though tiles afterwards
						// 数字 depends on 各种 parameters seen 在 movement code below. 测试 different ones out until it works alright
						bool closeThroughWall = between < 100f;

						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}

			// friendly needs to be set to 真 so the 仆从 can deal contact 伤害
			// friendly needs to be set to 假 so it doesn't 伤害 things like 目标 dummies while idling
			// Both things depend on if it has a 目标 or not, so it's just one assignment here
			// 你 don't need this assignment if your 仆从 is shooting things 代替 dealing contact 伤害
			Projectile.friendly = foundTarget;
		}

		private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition) {
			// 默认 movement parameters (here for attacking)
			float speed = 8f;
			float inertia = 20f;

			if (foundTarget) {
				// 仆从 has a 目标: 攻击 (here, fly towards the 敌人)
				if (distanceFromTarget > 40f) {
					// immediate 范围 around the 目标 (so it doesn't latch onto it when 关闭)
					Vector2 direction = targetCenter - Projectile.Center;
					direction.Normalize();
					direction *= speed;

					Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
				}
			}
			else {
				// 仆从 doesn't have a 目标: 返回 to 玩家 and idle
				if (distanceToIdlePosition > 600f) {
					// 速度 up the 仆从 if it's away 从 玩家
					speed = 12f;
					inertia = 60f;
				}
				else {
					// Slow down the 仆从 if closer 到 玩家
					speed = 4f;
					inertia = 80f;
				}

				if (distanceToIdlePosition > 20f) {
					// immediate 范围 around the 玩家 (when it passively floats about)

					// 这是 a simple movement 公式 使用 two parameters and its desired 方向 to create a "homing" movement
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
				}
				else if (Projectile.velocity == Vector2.Zero) {
					// 如果 there is a case where it's not moving at all, give it 一点 "poke"
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
			}
		}

		private void Visuals() {
			// So it will lean slightly towards the 方向 it's moving
			Projectile.rotation = Projectile.velocity.X * 0.05f;

			// 这是 a simple "循环 through all frames from 顶部 to 底部" 动画
			int frameSpeed = 5;

			Projectile.frameCounter++;

			if (Projectile.frameCounter >= frameSpeed) {
				Projectile.frameCounter = 0;
				Projectile.frame++;

				if (Projectile.frame >= Main.projFrames[Projectile.type]) {
					Projectile.frame = 0;
				}
			}

			// Some visuals here
			Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
		}
	}
}
