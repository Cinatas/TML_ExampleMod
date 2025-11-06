using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Tools
{
	internal class ExampleHookItem : ModItem
	{
		public override void SetDefaults() {
			// 复制 values 从 Amethyst Hook
			Item.CloneDefaults(ItemID.AmethystHook);
			Item.shootSpeed = 18f; // This defines how quickly the hook is shot.
			Item.shoot = ModContent.ProjectileType<ExampleHookProjectile>(); // 使 the 项 shoot the hook's 弹幕 when used.

			// 如果 you do not use 项.CloneDefaults(), 你必须 set the following values 对于 hook to work properly:
			// 项.useStyle = ItemUseStyleID.None;
			// 项.useTime = 0;
			// 项.useAnimation = 0;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}

	internal class ExampleHookProjectile : ModProjectile
	{
		private static Asset<Texture2D> chainTexture;

		public override void Load() { // This is called once on mod (re)加载 when this piece of content is being loaded.
			// 这是 the 路径 到 纹理 that we'll use 对于 hook's chain. 确保 to 更新 it.
			chainTexture = ModContent.Request<Texture2D>("ExampleMod/Content/Items/Tools/ExampleHookChain");
		}

		/*
		public override void SetStaticDefaults() {
			// 如果 you wish for your hook 弹幕 to have ONE 复制 of it PER 玩家, uncomment this section.
			ProjectileID.Sets.SingleGrappleHook[Type] = true;
		}
		*/

		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.GemHookAmethyst); // Copies the attributes 的 Amethyst hook's 弹幕.
		}

		// 使用 this hook for hooks that can have 多个 hooks mid-flight: Dual Hook, Web Slinger, Fish Hook, Static Hook, Lunar Hook.
		public override bool? CanUseGrapple(Player player) {
			int hooksOut = 0;
			foreach (var projectile in Main.ActiveProjectiles) {
				if (projectile.owner == Main.myPlayer && projectile.type == Projectile.type) {
					hooksOut++;
				}
			}

			return hooksOut <= 2;
		}

		// 使用 this to kill oldest hook. For hooks that kill the oldest when shot, not when the newest latches on: Like SkeletronHand
		// 你 can also change the 弹幕 like: Dual Hook, Lunar Hook
		// public override void UseGrapple(玩家 玩家, ref int 类型) {
		//	int hooksOut = 0;
		//	int oldestHookIndex = -1;
		//	int oldestHookTimeLeft = 100000;
		//	foreach (var otherProjectile in Main.ActiveProjectiles) {
		//		if (otherProjectile.所有者 == 玩家.whoAmI && otherProjectile.类型 == 类型) {
		//			hooksOut++;
		//			if (otherProjectile.timeLeft < oldestHookTimeLeft) {
		//				oldestHookIndex = otherProjectile.whoAmI;
		//				oldestHookTimeLeft = otherProjectile.timeLeft;
		//			}
		//		}
		//	}
		//	if (hooksOut > 1) {
		//		Main.弹幕[oldestHookIndex].Kill();
		//	}
		// }

		// Amethyst Hook is 300, Static Hook is 600.
		public override float GrappleRange() {
			return 500f;
		}

		public override void NumGrappleHooks(Player player, ref int numHooks) {
			numHooks = 2; // The amount of hooks that 可以 shot out
		}

		// 默认 is 11, Lunar is 24
		public override void GrappleRetreatSpeed(Player player, ref float speed) {
			speed = 18f; // How fast the grapple returns to you after meeting its max shoot 距离
		}

		public override void GrapplePullSpeed(Player player, ref float speed) {
			speed = 10; // How fast you get pulled 到 grappling hook 弹幕's landing 位置
		}

		// Adjusts the 位置 th在 玩家 将 pulled towards. This will make them hang 50 pixels away 从 图格 being grappled.
		public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY) {
			Vector2 dirToPlayer = Projectile.DirectionTo(player.Center);
			float hangDist = 50f;
			grappleX += dirToPlayer.X * hangDist;
			grappleY += dirToPlayer.Y * hangDist;
		}

		// Can customize what tiles this hook can latch onto, or force/防止 latching altogether, like Squirrel Hook also latching to trees
		public override bool? GrappleCanLatchOnTo(Player player, int x, int y) {
			// 默认情况下, the hook returns 空 to apply the vanilla conditions 对于 given 图格 位置 (this 图格 位置 可能 air or an actuated 图格!)
			// 如果 you 想要 返回 真 here, 确保 to check for Main.图格[x, y].HasUnactuatedTile (and Main.tileSolid[Main.图格[x, y].TileType] and/or Main.图格[x, y].HasTile 如果需要)

			// 我们 make this hook latch onto trees just like Squirrel Hook

			// Tree trunks can不 actuated so we don't 需要 check for that here
			Tile tile = Main.tile[x, y];
			if (TileID.Sets.IsATreeTrunk[tile.TileType] || tile.TileType == TileID.PalmTree) {
				return true;
			}

			// 在 任何 other case, behave like a normal hook
			return null;
		}

		// 绘制s the grappling hook's chain.
		public override bool PreDrawExtras() {
			Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
			Vector2 center = Projectile.Center;
			Vector2 directionToPlayer = playerCenter - Projectile.Center;
			float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
			float distanceToPlayer = directionToPlayer.Length();

			while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer)) {
				directionToPlayer /= distanceToPlayer; // 获取 unit vector
				directionToPlayer *= chainTexture.Height(); // multiply by chain link 长度

				center += directionToPlayer; // 更新 draw 位置
				directionToPlayer = playerCenter - center; // 更新 距离
				distanceToPlayer = directionToPlayer.Length();

				Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

				// 绘制 chain
				Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, chainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
			// 停止 vanilla from drawing the default chain.
			return false;
		}
	}
}
