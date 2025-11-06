using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ID;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Chat;

namespace ExampleMod.Content.Tiles
{
	public class ExampleSign : ModTile
	{
		public static LocalizedText DefaultSignText { get; private set; }

		public override void SetStaticDefaults() {
			// These are all used by TileID.Signs, 悬停 over them to read their documentation and see the purpose of each
			Main.tileSign[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.FramesOnKillWall[Type] = true;
			TileID.Sets.AvoidedByNPCs[Type] = true;
			TileID.Sets.TileInteractRead[Type] = true;
			TileID.Sets.InteractibleByNPCs[Type] = true;

			// TileObjectData assignment
			// TileID.Signs TileObjectData doesn't set StyleMultiplier to 5, so we will 不 copying from it 在这种情况下
			// 使用 Style2x2 as a base, we will create a TileObjectData with 5 alternate placements, each 锚定 to a different anchor.
			// 我们 also adjust the 原点 对于 alternates to 匹配 vanilla. Style2x2 starts with a 原点 at 0, 1 and a AnchorBottom, these will 两者 be adjusted 在 alternates.
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleMultiplier = 5; // Since each style has 5 placement styles, we set this to 5.
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty; // 清除 out existing 底部 anchor inherited from Style2x2 temporarily 以便 we don't 必须 set it to empty in each 的 alternates. 

			// 要 reduce code repetition, we'll use the same AnchorData 值 多个 times. This works because the 图格 is as tall as it is wide.
			AnchorData SolidOrSolidSideAnchor2TilesLong = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);

			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = Point16.Zero;
			TileObjectData.newAlternate.AnchorTop = SolidOrSolidSideAnchor2TilesLong;
			TileObjectData.addAlternate(1);

			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = Point16.Zero;
			TileObjectData.newAlternate.AnchorLeft = SolidOrSolidSideAnchor2TilesLong;
			TileObjectData.addAlternate(2);

			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 0);
			TileObjectData.newAlternate.AnchorRight = SolidOrSolidSideAnchor2TilesLong;
			TileObjectData.addAlternate(3);

			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = Point16.Zero;
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.addAlternate(4);

			// 最后, we restore the default AnchorBottom, the extra AnchorTypes here 允许 placing on tables, platforms, and other tiles.
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, 2, 0);
			TileObjectData.addTile(Type);

			// 地图 entry and extra localization
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), name);

			DefaultSignText = this.GetLocalization("DefaultSignText");
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			// This code sets a default 值 对于 sign, this is not typical and 可以 removed from normal sign tiles.
			int signId = Sign.ReadSign(i, j, true);
			if (signId != -1) {
				Sign.TextSign(signId, DefaultSignText.Value);
			}
		}

		public override bool RightClick(int i, int j) {
			// Normal sign 右 点击 behavior happens automatically 因为 Main.tileSign, this code just shows how to retrieve the 文本 的 sign and 应该 removed from normal sign tiles.
			int signId = Sign.ReadSign(i, j);
			if (signId != -1) {
				string signText = Main.sign[signId].text;
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(signText), Color.White);
			}
			return true;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			// Destroy the associated Sign 数据.
			Sign.KillSign(i, j);
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
			return true;
		}
	}
}
