using ExampleMod.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	/// <summary>
	/// This the 类 that clones the vanilla Meowmere 弹幕 using CloneDefaults().
	/// Make sure to check out <see cref="ExampleCloneWeapon" />, which fires this 弹幕; it itself is a cloned 版本 的 Meowmere.
	/// </summary>
	public class ExampleCloneProjectile : ModProjectile
	{
		public override void SetDefaults() {
			// 此方法 右 here is the backbone of what we're doing here; by using this 方法, we 复制 all of
			// the Meowmere 弹幕's SetDefault stats (例如 弹幕.friendly and 弹幕.penetrate) on to our 弹幕,
			// so we don't have to go in到 source and 复制 the stats ourselves. It saves a lot of 时间 and looks much cleaner;
			// if you're going to 复制 the stats of a 弹幕, use CloneDefaults().

			Projectile.CloneDefaults(ProjectileID.Meowmere);

			// 要 further the Cloning 过程, we can also 复制 the ai of any given 弹幕 using AIType, since we want
			// the 弹幕 to essentially behave the same way as the vanilla 弹幕.
			AIType = ProjectileID.Meowmere;

			// 之后 CloneDefaults has been called, we can now modify the stats to our wishes, or keep them as they are.
			// 对于 the sake of example, lets make our 弹幕 penetrate enemies a few more times than the vanilla 弹幕.
			// This 可以 done by modifying 弹幕.penetrate
			Projectile.penetrate += 3;
		}

		// While there are several different ways to change how our 弹幕 可能have differently, lets make it so
		// when our 弹幕 finally dies, it will explode into 4 regular Meowmere projectiles.
		public override void OnKill(int timeLeft) {
			Vector2 launchVelocity = new Vector2(-4, 0); // 创建 a 速度 moving the 左.
			for (int i = 0; i < 4; i++) {
				// Every 迭代, 旋转 the newly spawned 弹幕 by the equivalent 1/4th of a circle (MathHelper.PiOver4)
				// (Remember that all 旋转 in Terraria is based on Radians, NOT Degrees!)
				launchVelocity = launchVelocity.RotatedBy(MathHelper.PiOver4);

				// 生成 a new 弹幕 与 newly rotated 速度, belonging 到 original 弹幕 所有者. The new 弹幕 will inherit the spawning source of this 弹幕.
				Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVelocity, ProjectileID.Meowmere, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
			}
		}

		// Now, using CloneDefaults() and aiType doesn't 复制 EVERY aspect 的 弹幕. In Vanilla, several other methods
		// are used to generate different effects that aren't included in AI. 对于 case 的 Meowmere 弹幕, since the
		// ricochet 声音 is not included 在 AI, we must add it ourselves:
		public override bool OnTileCollide(Vector2 oldVelocity) {
			// Since there are two ricochet sounds 对于 Meowmere, we can randomly choose between them like this:

			SoundEngine.PlaySound(Main.rand.NextBool() ? SoundID.Item57 : SoundID.Item58, Projectile.position);

			// Essentially, using ? and : is a glorified and shortened 方法 of creating a simple if statement in
			// a single line. If Main.rand.NextBool() returns 真, it plays SoundID.Item57. If it returns 假, then it
			// will play SoundID.Item58. The 条件 goes before the ? and the two possibilities follow, separated by a :

			// This line calls the base (empty) implementation of this hook 方法 to 返回 its default 值, which in its case is always '真'.
			// 悬停 在 方法 below in VS to see its summary.
			return base.OnTileCollide(oldVelocity);
		}
	}
}
