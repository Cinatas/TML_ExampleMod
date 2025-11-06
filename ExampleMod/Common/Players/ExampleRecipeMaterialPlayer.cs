using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	// 此类展示如何使用玩家站立的箱子中的物品（如果存在）
	// 用于制作，即使玩家没有打开它
	// 其中一个用途是允许你的自定义银行中的物品用于制作
	public class ExampleRecipeMaterialPlayer : ModPlayer
	{
		private int _chestIndexNearby = -1;

		// Nearby 箱子 finding
		public override void PostUpdateMiscEffects() {
			if (Main.netMode == NetmodeID.Server) {
				// We don't 需要 do 任何 配方 stuff 在 服务器
				return;
			}

			int oldChestIndex = _chestIndexNearby;

			// 获取s leg 位置 in 图格 coord for further 箱子 searching
			var legPosition = Player.Bottom - new Vector2(0f, 20f);
			var legPositionInTile = legPosition.ToTileCoordinates();

			_chestIndexNearby = -1;
			// 查找 a possible 箱子 nearby
			for (int x = -1; x <= 1; x++) {
				var pos = new Point(legPositionInTile.X + x, legPositionInTile.Y);
				if (!WorldGen.InWorld(pos.X, pos.Y)) {
					continue;
				}

				var tile = Main.tile[pos];

				// Dressers are excluded to make 搜索 code simpler
				if (!tile.HasTile || !TileID.Sets.IsAContainer[tile.TileType] || tile.TileType is TileID.Dressers) {
					continue;
				}

				// 获取s the 左-顶部 位置 对于 箱子
				if (tile.TileFrameX % 36 != 0) {
					pos.X--;
				}

				if (tile.TileFrameY != 0) {
					pos.Y--;
				}

				int chestIndex = Chest.FindChest(pos.X, pos.Y);
				if (chestIndex > -1 && !Chest.IsLocked(pos.X, pos.Y)) {
					Chest chest = Main.chest[chestIndex];
					// Unopened chests in multiplayer have not initialized the items inside 的m, so we check for safety if the first 项 is not 空 (assuming that all others won't be 空 任一)
					// Ideally, we would 想要 write custom netcode to 请求 箱子 contents, see how a mod like 配方 Browser handles this: https://github.com/JavidPack/RecipeBrowser/blob/1.4/RecipeBrowser.cs, look for usage of packets
					if (chest.item[0] != null) {
						_chestIndexNearby = chestIndex;
						break;
					}
				}
			}

			// If the nearby 箱子 changed, call FindRecipes to 刷新 available recipes
			// Since FindRecipes takes a long 时间 to run, we should 尝试 avoid calling it frequently
			if (oldChestIndex != _chestIndexNearby) {
				Recipe.FindRecipes();
			}
		}

		// 使用 items 在 箱子 for 制作
		public override IEnumerable<Item> AddMaterialsForCrafting(out ItemConsumedCallback itemConsumedCallback) {
			// 确保 there is a 箱子 nearby 即 not opened by the 玩家, and wasn't destroyed last tick
			if (_chestIndexNearby is -1 || Player.chest == _chestIndexNearby || Main.chest[_chestIndexNearby] is not Chest chest)
				return base.AddMaterialsForCrafting(out itemConsumedCallback);

			// onUsedForCrafting invokes when the 项 is consumed, 可以 用于 send packets in multiplayer 模式
			// If there is no need for this, just set it to 空
			itemConsumedCallback = (_, index) => {
				if (Main.netMode is NetmodeID.MultiplayerClient) {
					// 同步 箱子 数据
					NetMessage.SendData(MessageID.SyncChestItem, number: _chestIndexNearby, number2: index);
				}
			};

			// 返回s the items 在 箱子 to use them for 制作
			// The returned 列表 should 不 a cloned 版本 of items 否则 items will 不 consumed
			return chest.item;
		}
	}
}