using ExampleMod.Common;
using ExampleMod.Common.Systems;
using ExampleMod.Content.Items.Placeable;
using ExampleMod.Content.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ObjectData;

namespace ExampleMod.Content.Tiles
{
	/// <summary>
	/// An example for creating a Pylon, identical to how they 函数 in Vanilla. Shows off <seealso cref="ModPylon"/>, an abstract
	/// 扩展名 of <seealso cref="ModTile"/> that has additional functionality for Pylon specific tiles.
	/// <br>
	/// If you are 将要 make 多个 pylons that all act the same (like in Vanilla), it is recommended you make a base 类
	/// with override functionality in 顺序 to 防止 writing boilerplate. (例如, making a "CrystalTexture" 属性 that you can
	/// override in 顺序 to streamline that 过程.)
	/// </br>
	/// </summary>
	public class ExamplePylonTile : ModPylon
	{
		public const int CrystalVerticalFrameCount = 8;

		public Asset<Texture2D> crystalTexture;
		public Asset<Texture2D> crystalHighlightTexture;
		public Asset<Texture2D> mapIcon;

		public override void Load() {
			// We'll need these textures 以后, it's best practice to 缓存 them on 加载 代替 continually requesting 每个 draw call.
			crystalTexture = ModContent.Request<Texture2D>(Texture + "_Crystal");
			crystalHighlightTexture = ModContent.Request<Texture2D>(Texture + "_CrystalHighlight");
			mapIcon = ModContent.Request<Texture2D>(Texture + "_MapIcon");
		}

		public override void SetStaticDefaults() {
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			// These definitions 允许 for vanilla's pylon TileEntities to be placed.
			// tModLoader has a built in 图格 Entity specifically for modded pylons, which we must extend (see SimplePylonTileEntity)
			TEModdedPylon moddedPylon = ModContent.GetInstance<SimplePylonTileEntity>();
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(moddedPylon.PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(moddedPylon.Hook_AfterPlacement, -1, 0, false);

			TileObjectData.addTile(Type);

			TileID.Sets.InteractibleByNPCs[Type] = true;
			TileID.Sets.PreventsSandfall[Type] = true;
			TileID.Sets.AvoidedByMeteorLanding[Type] = true;

			// 添加s functionality for proximity of pylons; if this is 真, then being near this 图格 will 计数 as being near a pylon 对于 teleportation 过程.
			AddToArray(ref TileID.Sets.CountsAsPylon);

			LocalizedText pylonName = CreateMapEntryName(); //名称 is 在 localization 文件
			AddMapEntry(Color.White, pylonName);
		}

		public override NPCShop.Entry GetNPCShopEntry() {
			// 在 this 方法 we can customize the 商店 entry 对于 pylon 项.
			// 默认 方法, base.GetNPCShopEntry(), generates a 商店 entry 对于 pylon 项 与 typical pylon conditions: 条件.HappyEnoughToSellPylons, 条件.AnotherTownNPCNearby, and 条件.NotInEvilBiome
			NPCShop.Entry shopEntry = base.GetNPCShopEntry();

			// 我们 will take that 商店 entry and add an additional 条件 to check 例如Biome, as this is typical for 生物群系 pylons
			// This does not affect the 传送 conditions, only the sale conditions
			shopEntry.AddCondition(ExampleConditions.InExampleBiome);

			// and finally we 返回 the 商店 entry
			return shopEntry;
		}

		public override void MouseOver(int i, int j) {
			// 显示 一点 pylon 图标 在 鼠标 indicating we are hovering over it.
			Main.LocalPlayer.cursorItemIconEnabled = true;
			Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<ExamplePylonItem>();
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			// 我们 需要 clean up after ourselves, since this is still a "unique" 图格, 分离 from Vanilla Pylons, so we must kill the TileEntity.
			ModContent.GetInstance<SimplePylonTileEntity>().Kill(i, j);
		}

		public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount) {
			// Let's say for fun sake that no NPCs 需要 be nearby in 顺序 for this pylon to 函数. If you want your pylon to 函数 just like vanilla,
			// you don't 需要 override this 方法 at all.
			return true;
		}

		public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) {
			// 右 before this hook is called, the sceneData 参数 exports its information 基于 wherever the destination pylon is,
			// and by 扩展名, it will call ALL ModSystems that use the TileCountsAvailable 方法. This means, that if you determine biomes
			// based off of 图格 计数, when this hook is called, you can simply check the 图格 阈值, like we do here. 在 context of ExampleMod,
			// something is considered with在 Example Surface/Underground 生物群系 if there are 40 或更多 example blocks at that 位置.

			return ModContent.GetInstance<ExampleBiomeTileCount>().exampleBlockCount >= 40;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
			// Pylons in vanilla light up, 即 just a simple functionality we add using ModTile's ModifyLight.
			// Let's just add a simple white light for our pylon:
			r = g = b = 0.75f;
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) {
			// 我们 想要 draw the pylon crystal the exact same way vanilla does, so we can use this built in 方法 in ModPylon for default crystal drawing:
			// 对于 the sake of example, lets make our pylon create a bit more dust by decreasing the dustConsequent 值 down to 1. If you want your dust spawning to be identical to vanilla, set dustConsequent to 4.
			// 我们 also multiply the pylonShadowColor in 顺序 to decrease its opacity, so it actually looks like a "shadow"
			DefaultDrawPylonCrystal(spriteBatch, i, j, crystalTexture, crystalHighlightTexture, new Vector2(0f, -12f), Color.White * 0.1f, Color.White, 1, CrystalVerticalFrameCount);
		}

		public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale) {
			// Just like in SpecialDraw, we want things to be handled the EXACT same way vanilla would 处理 it, which ModPylon also has built in methods for:
			bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f), drawColor, deselectedScale, selectedScale);
			DefaultMapClickHandle(mouseOver, pylonInfo, ModContent.GetInstance<ExamplePylonItem>().DisplayName.Key, ref mouseOverText);
		}
	}
}
