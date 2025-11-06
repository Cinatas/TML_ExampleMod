using ExampleMod.Content.Items.Placeable;
using ExampleMod.Content.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Content.Tiles
{
	/// <summary>
	/// This is a more advanced variation 的 <seealso cref="ExamplePylonTile"/> implementation
	/// in tandem with <seealso cref="AdvancedPylonTileEntity"/>, which shows off what advanced techniques you can apply with ModPylons.
	/// If you want to use ModPylons with your own 图格 Entities or with multi-tiles that do not conform to vanilla's standards, then
	/// this is the example for you. If you just want normal pylons that act like the ones in vanilla do, check out <seealso cref="ExamplePylonTile"/>.
	/// </summary>
	/// <remarks>
	/// Note that since this is an advanced example, things that were already explained in <seealso cref="ExamplePylonTile"/> will not
	/// be as thoroughly explained. They will still be explained 如果需要 in context.
	/// </remarks>
	public class ExamplePylonTileAdvanced : ModPylon
	{
		public const int CrystalVerticalFrameCount = 8;

		public Asset<Texture2D> crystalTexture;
		public Asset<Texture2D> crystalHighlightTexture;
		public Asset<Texture2D> mapIcon;

		public override void Load() {
			// We'll still use the other Example Pylon's sprites, but we need to adjust the 纹理 values first to do so.
			crystalTexture = ModContent.Request<Texture2D>(Texture.Replace("Advanced", "") + "_Crystal");
			crystalHighlightTexture = ModContent.Request<Texture2D>(Texture.Replace("Advanced", "") + "_CrystalHighlight");
			mapIcon = ModContent.Request<Texture2D>(Texture.Replace("Advanced", "") + "_MapIcon");
		}

		public override void SetStaticDefaults() {
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;

			// This 时间 around, we'll have a 图格 即 2x3 instead of 3x4.
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 2);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			// Since we are going to need more in-depth functionality, we can't use vanilla's Pylon TE's OnPlace or CanPlace:
			AdvancedPylonTileEntity advancedEntity = ModContent.GetInstance<AdvancedPylonTileEntity>();
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(advancedEntity.PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(advancedEntity.Hook_AfterPlacement, -1, 0, false);

			TileObjectData.addTile(Type);

			TileID.Sets.InteractibleByNPCs[Type] = true;
			TileID.Sets.PreventsSandfall[Type] = true;

			AddToArray(ref TileID.Sets.CountsAsPylon);

			LocalizedText pylonName = CreateMapEntryName();
			AddMapEntry(Color.Black, pylonName);
		}

		public override NPCShop.Entry GetNPCShopEntry() {
			// Let's say that our pylon is for sale no matter what for any NPC under all circumstances.
			return new NPCShop.Entry(ModContent.ItemType<ExamplePylonItemAdvanced>());
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
			return true;
		}

		public override bool RightClick(int i, int j) {
			Main.mapFullscreen = true;
			SoundEngine.PlaySound(SoundID.MenuOpen);
			return true;
		}

		public override void MouseOver(int i, int j) {
			Main.LocalPlayer.cursorItemIconEnabled = true;
			Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<ExamplePylonItemAdvanced>();
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			ModContent.GetInstance<AdvancedPylonTileEntity>().Kill(i, j);
		}

		// 对于 the sake of example, we will 允许 this pylon to always be teleported to as long as it is on, so we make sure these two checks 返回 真.
		public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount) {
			return true;
		}

		public override bool ValidTeleportCheck_AnyDanger(TeleportPylonInfo pylonInfo) {
			return true;
		}

		// These two steps below are simply determining whether or not either side 的 硬币 is valid, 即 to say:
		// Is the destination pylon (the pylon clicked 在 地图) a valid pylon, and is the pyl在 玩家 standing near (the nearby pylon)
		// a valid pylon? If either one 的se checks fail, a errorKey wil be set to a custom localization 键 and a 消息 will go 到 玩家 with
		// said 文本 (after its been localized, of course).
		public override void ValidTeleportCheck_DestinationPostCheck(TeleportPylonInfo destinationPylonInfo, ref bool destinationPylonValid, ref string errorKey) {
			// 如果 you are unfamiliar with pattern matching notation, all this is asking is:
			// 1) The 图格 Entity 在 given 位置 is an AdvancedPylonTileEntity (AKA not 空 or something else)
			// 2) The 图格 Entity's isActive 值 is 假
			if (TileEntity.ByPosition[destinationPylonInfo.PositionInTiles] is AdvancedPylonTileEntity { isActive: false }) {
				//Given that both 的se things are 真, set the 错误 键 to our own special 消息 (check the localization 文件), and make the destination 值 invalid (假)
				destinationPylonValid = false;
				errorKey = "Mods.ExampleMod.MessageInfo.UnstablePylonIsOff";
			}
		}

		public override void ValidTeleportCheck_NearbyPostCheck(TeleportPylonInfo nearbyPylonInfo, ref bool destinationPylonValid, ref bool anyNearbyValidPylon, ref string errorKey) {
			// next check is determining whether or not the nearby pylon is potentially unstable, and if so, if it's not active, we also 防止 teleportation.
			if (TileEntity.ByPosition[nearbyPylonInfo.PositionInTiles] is AdvancedPylonTileEntity { isActive: false }) {
				destinationPylonValid = false;
				errorKey = "Mods.ExampleMod.MessageInfo.NearbyUnstablePylonIsOff";
			}
		}

		public override void ModifyTeleportationPosition(TeleportPylonInfo destinationPylonInfo, ref Vector2 teleportationPosition) {
			// Now, 对于 fun of it and 对于 showcase of this hook, let's put a 玩家 a bit in到 air above the pylon when they 传送.
			teleportationPosition = destinationPylonInfo.PositionInTiles.ToWorldCoordinates(8f, -32f);
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
			// Same as the basic example, but our light 将 the disco 颜色 like the crystal
			r = Main.DiscoColor.R / 255f * 0.75f;
			g = Main.DiscoColor.G / 255f * 0.75f;
			b = Main.DiscoColor.B / 255f * 0.75f;
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
			// This 时间, we'll ONLY draw the crystal if the pylon is active
			// 我们 need to check the framing here in 顺序 to guarantee we that we are trying to grab the TE ONLY when 在 顶部 左 corner, where it is
			// located. If we don't do this check, we 将 attempting to grab the TE in 位置 where it doesn't exist, throwing errors and causing
			// 加载 of visual bugs.
			if (drawData.tileFrameX % 36 == 0 && drawData.tileFrameY == 0 && TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is AdvancedPylonTileEntity { isActive: true }) {
				Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
			}
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) {
			// This code is essentially identical to how it is 在 basic example, but this 时间 the crystal 颜色 is the disco (rainbow) 颜色 instead
			// 另外, since we want the pylon crystal to be drawn 在 same 高度 as vanilla (since our 图格 is one 图格 smaller), we have to 移动 up the crystal accordingly 与 crystalOffset 参数
			DefaultDrawPylonCrystal(spriteBatch, i, j, crystalTexture, crystalHighlightTexture, new Vector2(0f, -18f), Main.DiscoColor * 0.1f, Main.DiscoColor, 1, CrystalVerticalFrameCount);
		}

		public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale) {
			if (!TileEntity.ByPosition.TryGetValue(pylonInfo.PositionInTiles, out var te) || te is not AdvancedPylonTileEntity entity) {
				// 如果 for some reason we don't 查找 the 图格 entity, we won't draw anything.
				return;
			}

			// Depending 在 whether or not the pylon is active, the 颜色 的 图标 will change;
			// otherwise, it acts as normal.
			drawColor = !entity.isActive ? Color.Gray * 0.5f : drawColor;
			bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1, 1.5f), drawColor, deselectedScale, selectedScale);
			DefaultMapClickHandle(mouseOver, pylonInfo, ModContent.GetInstance<ExamplePylonItemAdvanced>().DisplayName.Key, ref mouseOverText);
		}
	}
}
