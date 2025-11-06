using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Tiles
{
	// 此类 shows off a 数字 of less common ModTile methods. These methods 帮助 our 陷阱 图格 behave like vanilla traps. 
	// 在 particular, hammer behavior is particularly tricky. The logic here is setup for 多个 styles 以及.
	public class ExampleTrap : ModTile
	{
		public override void SetStaticDefaults() {
			TileID.Sets.DrawsWalls[Type] = true;
			TileID.Sets.DontDrawTileSliced[Type] = true;
			TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;
			TileID.Sets.IsAMechanism[Type] = true;

			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileFrameImportant[Type] = true;

			// These 2 AddMapEntry and GetMapOption show off 多个 地图 Entries per 图格. 删除 GetMapOption and all but 1 的se for your own ModTile if you don't actually need it.
			AddMapEntry(new Color(21, 179, 192), Language.GetText("MapObject.Trap")); // localized 文本 for "陷阱"
			AddMapEntry(new Color(0, 141, 63), Language.GetText("MapObject.Trap"));
		}

		// Read the comments above on AddMapEntry.
		public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameY / 18);

		public override bool IsTileDangerous(int i, int j, Player player) => true;

		// Because this 图格 does not use a TileObjectData, and 因此 does not have "real" 图格 styles, the correct 图格 style 值 can't be determined automatically. 这意味着 th在 correct 项 won't automatically 放下, so we must use GetItemDrops to calculate the 图格 style to determine the 项 放下. 
		public override IEnumerable<Item> GetItemDrops(int i, int j) {
			Tile t = Main.tile[i, j];
			int style = t.TileFrameY / 18;
			// It 可以 useful to share a single 图格 with 多个 styles.
			yield return new Item(Mod.Find<ModItem>(Items.Placeable.ExampleTrap.GetInternalNameFromStyle(style)).Type);

			// 在这里 is an alternate approach:
			// int dropItem = TileLoader.GetItemDropFromTypeAndStyle(类型, style);
			// yield 返回 new 项(dropItem);
		}

		public override bool CreateDust(int i, int j, ref int type) {
			int style = Main.tile[i, j].TileFrameY / 18;
			if (style == 0) {
				type = DustID.Glass; // A blue dust to 匹配 the 图格
			}
			if (style == 1) {
				type = DustID.JungleGrass; // A green dust 对于 2nd style.
			}
			return true;
		}

		// PlaceInWorld is needed to facilitate styles and alternates since this 图格 doesn't use a TileObjectData. Placing 左 and 右 基于 玩家 方向 is usually done 在 TileObjectData, but the specifics of that don't work for how we want this 图格 to work. 
		public override void PlaceInWorld(int i, int j, Item item) {
			int style = Main.LocalPlayer.HeldItem.placeStyle;
			Tile tile = Main.tile[i, j];
			tile.TileFrameY = (short)(style * 18);
			if (Main.LocalPlayer.direction == 1) {
				tile.TileFrameX += 18;
			}
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
			}
		}

		// This progression matches vanilla tiles, you don't 必须 follow it if you don't want. Some vanilla traps don't have 6 states, only 4. This 可以 implemented with different logic in Slope. Making 8 directions is also easily done in a similar manner.
		private static int[] frameXCycle = { 2, 3, 4, 5, 1, 0 };
		// 我们 can use the Slope 方法 to override what happens when this 图格 is hammered.
		public override bool Slope(int i, int j) {
			Tile tile = Main.tile[i, j];
			int nextFrameX = frameXCycle[tile.TileFrameX / 18];
			tile.TileFrameX = (short)(nextFrameX * 18);
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
			}
			return false;
		}

		public override void HitWire(int i, int j) {
			Tile tile = Main.tile[i, j];
			int style = tile.TileFrameY / 18;
			Vector2 spawnPosition;
			// This logic here corresponds 到 orientation 的 sprites 在 spritesheet, change it if your 图格 is different in design.
			int horizontalDirection = (tile.TileFrameX == 0) ? -1 : ((tile.TileFrameX == 18) ? 1 : 0);
			int verticalDirection = (tile.TileFrameX < 36) ? 0 : ((tile.TileFrameX < 72) ? -1 : 1);
			// Each 陷阱 style within this 图格 shoots different projectiles.
			if (style == 0) {
				// Wiring.CheckMech checks if the wiring cooldown has been reached. Put a longer 数字 here for less frequent 弹幕 spawns. 200 is the dart/flame cooldown. Spear is 90, spiky ball is 300
				if (Wiring.CheckMech(i, j, 60)) {
					spawnPosition = new Vector2(i * 16 + 8 + 0 * horizontalDirection, j * 16 + 9 + 0 * verticalDirection); // The extra numbers here 帮助 中心 the 弹幕 生成 位置 if you 需要.

					// 在 a real mod you 应该 spawning projectiles that are 两者 hostile and friendly to do 伤害 to 两者 players and NPC, as Terraria traps do.
					// 确保 to change 速度, 弹幕, 伤害, and knockback.
					Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), spawnPosition, new Vector2(horizontalDirection, verticalDirection) * 6f, ProjectileID.IchorBullet, 20, 2f, Main.myPlayer);
				}
			}
			else if (style == 1) {
				// 一个 longer cooldown for ChlorophyteBullet 陷阱.
				if (Wiring.CheckMech(i, j, 200)) {
					spawnPosition = new Vector2(i * 16 + 8 + 0 * horizontalDirection, j * 16 + 9 + 0 * verticalDirection); // The extra numbers here 帮助 中心 the 弹幕 生成 位置.
					Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), spawnPosition, new Vector2(horizontalDirection, verticalDirection) * 8f, ProjectileID.ChlorophyteBullet, 40, 2f, Main.myPlayer);
				}
			}
		}
	}
}