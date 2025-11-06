using ExampleMod.Content.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	// 此文件 showcases the concept of piercing.
	// code 的 项 that spawns it is located 在 底部.

	// NPC.immune determines if an npc 可以 hit by a 项 or 弹幕 owned by a particular 玩家 (it is an 数组, each 槽位 corresponds to different players (whoAmI))
	// NPC.immune is decremented towards 0 每个 更新
	// Melee items set NPC.immune to 玩家.itemAnimation, which starts at 项.useAnimation and decrements towards 0
	// Projectiles, however, provide mechanisms for custom immunity.
	// 1. penetrate == 1: A 弹幕 with penetrate set to 1 in SetDefaults will hit regardless 的 NPC's immunity counters (The penetrate from SetDefaults is remembered in maxPenetrate)
	//	Ex: Wooden 箭.
	// 2. No code and penetrate > 1, penetrate == -1, or (appliesImmunityTimeOnSingleHits && penetrate == 1): npc.immune[所有者] 将 set to 10.
	// 	The NPC 将 hit 如果不是 immune and 将come immune to all 伤害 for 10 ticks
	// 	Ex: Unholy 箭
	// 3. Override OnHitNPC: 如果不是 immune, when it hits it manually set an immune other than 10
	// 	Ex: Arkhalis: Sets it to 5
	// 	Ex: Sharknado 仆从: Sets to 20
	// 	Video: https://media-1.discordapp.net/attachments/242228770855976960/1150275205017636995/Projectile_Immunity_Sharknado_Arkhalis_Example.mp4 Notice how Sharknado 仆从 hits 防止 Arkhalis hits for a brief moment.
	// 4. 弹幕.usesIDStaticNPCImmunity and 弹幕.idStaticNPCHitCooldown: Specifies that a 类型 of 弹幕 has a shared immunity 计时器 for each npc.
	// 	Use this if you want other projectiles a 概率 to 伤害, but don't want the same 弹幕 类型 to hit an npc rapidly.
	// 	Ex: Ghastly Glaive is the only one who uses this.
	// 5. 弹幕.usesLocalNPCImmunity and 弹幕.localNPCHitCooldown: Specifies the 弹幕 manages it's own immunity timers for each npc
	// 	Use this if you want the 多个 projectiles 的 same 类型 to have a 概率 to 攻击 rapidly, but don't want a single 弹幕 to hit rapidly. A -1 值 prevents the same 弹幕 from ever hitting the npc again.
	// 	Ex: Lightning Aura sentries use this. (localNPCHitCooldown = 3, but other code controls how fast the 弹幕 itself hits)
	// 		Overlapping Auras all have a 概率 to hit after each other 尽管 they share the same ID.
	// Try the above by uncommenting out the respective bits of code 在 弹幕 below.


	public class ExamplePiercingProjectile : ModProjectile
	{
		public override void SetDefaults() {
			Projectile.width = 12; // The 宽度 of 弹幕 hitbox
			Projectile.height = 12; // The 高度 of 弹幕 hitbox

			// 复制 the ai of 任何 given 弹幕 using AIType, since we want
			// the 弹幕 to essentially behave the same way as the vanilla 弹幕.
			AIType = ProjectileID.Bullet;

			Projectile.friendly = true; // Can the 弹幕 deal 伤害 to enemies?
			Projectile.DamageType = DamageClass.Melee; // Is the 弹幕 shoot by a ranged 武器?
			Projectile.ignoreWater = true; // Does the 弹幕's 速度 be influenced by water?
			Projectile.tileCollide = false; // Can the 弹幕 collide with tiles?
			Projectile.timeLeft = 60; // Each 更新 timeLeft is decreased by 1. Once timeLeft hits 0, the 弹幕 will naturally despawn. (60 ticks = 1 second)

			Projectile.penetrate = -1;
			// 1: 弹幕.penetrate = 1; // Will hit 即使 npc is currently immune to 玩家
			// 2a: 弹幕.penetrate = -1; // Will hit and unless 3 is use, set 10 ticks of immunity
			// 2b: 弹幕.penetrate = 3; // Same, but max 3 hits before dying
			// 5: 弹幕.usesLocalNPCImmunity = 真;
			// 5a: 弹幕.localNPCHitCooldown = -1; // 1 hit per npc max
			// 5b: 弹幕.localNPCHitCooldown = 20; // 20 ticks before the same npc 可以 hit again
		}

		// 参见 comments 在 beginning 的 类
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			// 3a: 目标.immune[弹幕.所有者] = 20;
			// 3b: 目标.immune[弹幕.所有者] = 5;
		}
	}

	// 这是 a simple 项 即 based 在 FlintlockPistol and shoots ExamplePiercingProjectile to showcase it.
	internal class ExamplePiercingProjectileItem : ModItem
	{
		public override string Texture => $"Terraria/Images/Item_{ItemID.FlintlockPistol}";

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.FlintlockPistol);
			Item.useAmmo = AmmoID.None;
			Item.shoot = ModContent.ProjectileType<ExamplePiercingProjectile>();
		}
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
