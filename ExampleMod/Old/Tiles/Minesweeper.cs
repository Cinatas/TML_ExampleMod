using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.Tiles
{
	// This 类 replicates the behavior 的 game Minesweeper within a ModTile.
	// This contrived example serves to teach modders about what TileFrame is capable of. Usually ModTiles are "framed" 根据 vanilla patterns. We override this behavior as a teaching example.
	public class Minesweeper : ModTile
	{
		public override void SetDefaults() {
			// Most 1x1 tiles without a TileObjectData don't set tileFrameImportant because FrameTile will reconstruct the 帧 automatically. 
			// This 图格 is special because 我们需要 it to preserve the hidden 地雷 tiles.
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = true; // 待办事项： tModLoader hook for allowing non solid tiles to be hammer-able.
			drop = ItemType<MinesweeperItem>();
		}

		public override bool Dangersense(int i, int j, Player player) => IsMine(i, j);

		public override void PlaceInWorld(int i, int j, Item item) {
			Tile tile = Main.tile[i, j];
			if (Main.rand.NextBool(4)) // 1 in 4 placed Tiles 将 a 地雷
			{
				tile.frameX = 18;
				TileFrame8Neighbors(i, j);
				if (Main.netMode == NetmodeID.MultiplayerClient) // If we are a multiplayer 客户端, we 需要 inform the 服务器 的 changes we've made 到 图格.
					NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
			}
		}

		// When a 图格 is hammered, we 需要 reveal it and possibly 更新 nearby tiles. 
		public override bool Slope(int i, int j) {
			Tile tile = Main.tile[i, j];
			bool IsBomb = (tile.frameX == 18 || tile.frameX == 5 * 18) && tile.frameY == 0;

			if (IsBomb) {
				// 生成ing a 手榴弹 弹幕 that dies quickly is the simplest way to get this 效果
				int projectile = Projectile.NewProjectile(i * 16 + 8, j * 16 + 8, 0, 0, ProjectileID.Grenade, 30, 1, Main.myPlayer);
				Main.projectile[projectile].timeLeft = 2;
				Main.projectile[projectile].netUpdate = true;
				tile.frameX = 5 * 18;

				if (Main.netMode == NetmodeID.MultiplayerClient) // Slope is called on Clients, so we 需要 inform the 服务器 of changes.
					NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
			}
			else {
				short mineCount = NearbyMines(i, j);
				if (mineCount == 0)
					RevealNeighbors(i, j);
				tile.frameX = 0; // TileFrame will take care of 设置 this correctly. 
				tile.frameY = 18;

				WorldGen.TileFrame(i, j);
				TileFrame8Neighbors(i, j);
			}
			// By returning 假, we tell Terraria to 跳过 the default sloping behavior
			return false;
		}

		// By using ModTile.TileFrame, 我们可以 have tiles adapt to nearby tiles however we like.
		// TileFrame is called to correct the frameX and frameY values of this 图格. Usually this happens when a 图格 is placed nearby or when the 世界 is first loaded.
		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
			Tile tile = Main.tile[i, j];
			bool changed = false;
			// frameX and frameY correspond 到 顶部 左 corner 的 精灵 在 图格 spritesheet.
			bool revealed = !((tile.frameX == 18 || tile.frameX == 0) && tile.frameY == 0);
			bool revealedBomb = tile.frameX == 5 * 18 && tile.frameY == 0;
			if (revealed && !revealedBomb) {
				short mineCount = NearbyMines(i, j);
				if (tile.frameX != (mineCount + 1) * 18 || tile.frameY != 18)
					changed = true;
				tile.frameX = (short)((mineCount + 1) * 18);
				tile.frameY = 18;
			}
			if (changed) {
				if (Main.netMode == NetmodeID.MultiplayerClient)
					NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);

				// Since this 图格 changed, we will change other nearby tiles. This isn't typical but is suitable for minesweeper. 
				TileFrame8Neighbors(i, j);
			}
			return false;
		}

		// A recursive 方法 that visits nearby Minesweeper tiles and reveals them, continuing to reveal if there are no nearby mines.
		void RevealNeighbors(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			if (tile.active() && tile.type == Type && (tile.frameY != 18 /*|| (tile.frameX == 0 && tile.frameY == 18)*/)) {
				// revealed, not 右 数字, TileFrame will fix
				tile.frameX = 0;
				tile.frameY = 18;

				if (NearbyMines(i, j) == 0) {
					RevealNeighbors(i + 1, j);
					RevealNeighbors(i - 1, j);
					RevealNeighbors(i, j - 1);
					RevealNeighbors(i, j + 1);
				}
			}
		}

		bool IsMine(int i, int j) => IsMine(Main.tile[i, j]);

		bool IsMine(Tile tile) => tile.type == Type && tile.frameX != 0 && tile.frameY == 0;

		short NearbyMines(int i, int j) => (short)(new bool[] { IsMine(i - 1, j - 1), IsMine(i - 1, j), IsMine(i - 1, j + 1), IsMine(i, j - 1), IsMine(i, j + 1), IsMine(i + 1, j - 1), IsMine(i + 1, j), IsMine(i + 1, j + 1), }.Count(b => b));

		private void TileFrame8Neighbors(int i, int j) {
			WorldGen.TileFrame(i + 1, j);
			WorldGen.TileFrame(i - 1, j);
			WorldGen.TileFrame(i, j + 1);
			WorldGen.TileFrame(i, j - 1);
			WorldGen.TileFrame(i + 1, j + 1);
			WorldGen.TileFrame(i - 1, j - 1);
			WorldGen.TileFrame(i - 1, j + 1);
			WorldGen.TileFrame(i + 1, j - 1);
		}
	}

	public class MinesweeperItem : ModItem
	{
		public override string Texture => "ExampleMod/Items/ExampleItem";

		public override void SetDefaults() {
			item.CloneDefaults(ItemID.DirtBlock);
			item.createTile = TileType<Minesweeper>();
		}
	}
}