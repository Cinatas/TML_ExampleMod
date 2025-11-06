using ExampleMod.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ExampleMod.Common.UI.ResourceOverlay
{
	public class VanillaManaOverlay : ModResourceOverlay
	{
		// 此字段用于缓存此文件下方 CompareAssets 辅助方法中使用的原版资产
		private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

		// 这些字段用于缓存 ModContent.请求<Texture2D>() 的结果
		private Asset<Texture2D> starTexture, fancyPanelTexture, barsFillingTexture, barsPanelTexture;

		// 与 VanillaLifeOverlay 不同，此钩子绘制每个星星。
		public override void PostDrawResource(ResourceOverlayDrawContext context) {
			Asset<Texture2D> asset = context.texture;

			string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
			string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

			if (Main.LocalPlayer.GetModPlayer<ExampleStatIncreasePlayer>().exampleManaCrystals <= 0)
				return;

			// 注意：CompareAssets 在此方法主体下方定义
			if (asset == TextureAssets.Mana) {
				// 在经典星星上绘制
				DrawClassicFancyOverlay(context);
			}
			else if (CompareAssets(asset, fancyFolder + "Star_Fill")) {
				// 绘制 over the Fancy stars
				DrawClassicFancyOverlay(context);
			}
			else if (CompareAssets(asset, barsFolder + "MP_Fill")) {
				// 绘制 over the Bars 魔力 bars
				DrawBarsOverlay(context);
			}
			else if (CompareAssets(asset, fancyFolder + "Star_A") || CompareAssets(asset, fancyFolder + "Star_B") || CompareAssets(asset, fancyFolder + "Star_C") || CompareAssets(asset, fancyFolder + "Star_Single")) {
				// 绘制 over the Fancy star panels
				DrawFancyPanelOverlay(context);
			}
			else if (CompareAssets(asset, barsFolder + "MP_Panel_Middle")) {
				// 绘制 over the Bars middle 魔力 panels
				DrawBarsPanelOverlay(context);
			}
		}

		private bool CompareAssets(Asset<Texture2D> existingAsset, string compareAssetPath) {
			// 这是一个 helper 方法 for checking if a certain vanilla asset was drawn
			if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
				asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

			return existingAsset == asset;
		}

		private void DrawClassicFancyOverlay(ResourceOverlayDrawContext context) {
			// 绘制 over the Classic / 魔力 stars
			// "context" contains information 用于 draw the 资源
			// If you 想要 draw directly on 顶部 的 vanilla stars, just 替换 the 纹理 and have the context draw the new 纹理
			context.texture = starTexture ??= ModContent.Request<Texture2D>("ExampleMod/Common/UI/ResourceOverlay/ClassicManaOverlay");
			context.Draw();
		}

		// 绘制ing over the 面板 backgrounds is not required.
		// 此示例 just showcases changing the "inner" part 的 star panels to more closely resemble the example life fruit.
		private void DrawFancyPanelOverlay(ResourceOverlayDrawContext context) {
			// 绘制 over the Fancy star panels
			string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";

			// The original 位置 refers 到 entire 面板 slice.
			// 然而, since this overlay only modifies the "inner" portion 的 slice (aka the part behind the star),
			// the 位置 应该 modified to compensate 对于 精灵 大小 difference
			Vector2 positionOffset;

			if (context.resourceNumber == context.snapshot.AmountOfManaStars - 1) {
				// Final 面板 在 列.  Determine whether it has panels above it
				if (CompareAssets(context.texture, fancyFolder + "Star_Single")) {
					// 首先 and only 面板
					positionOffset = new Vector2(4, 4);
				}
				else {
					// Other panels existed above this 面板
					// Vanilla 纹理 is "Star_C"
					positionOffset = new Vector2(4, 0);
				}
			}
			else if (CompareAssets(context.texture, fancyFolder + "Star_A")) {
				// 首先 面板 在 列
				positionOffset = new Vector2(4, 4);
			}
			else {
				// Any 面板 that has a 面板 above AND below it
				// Vanilla 纹理 is "Star_B"
				positionOffset = new Vector2(4, 0);
			}

			// "context" contains information 用于 draw the 资源
			// If you 想要 draw directly on 顶部 的 vanilla stars, just 替换 the 纹理 and have the context draw the new 纹理
			context.texture = fancyPanelTexture ??= ModContent.Request<Texture2D>("ExampleMod/Common/UI/ResourceOverlay/FancyManaOverlay_Panel");
			// Due 到 replacement 纹理 and the vanilla 纹理 having different dimensions, the source needs to also be modified
			context.source = context.texture.Frame();
			context.position += positionOffset;
			context.Draw();
		}

		private void DrawBarsOverlay(ResourceOverlayDrawContext context) {
			// 绘制 over the Bars 魔力 bars
			// "context" contains information 用于 draw the 资源
			// If you 想要 draw directly on 顶部 的 vanilla bars, just 替换 the 纹理 and have the context draw the new 纹理
			context.texture = barsFillingTexture ??= ModContent.Request<Texture2D>("ExampleMod/Common/UI/ResourceOverlay/BarsManaOverlay_Fill");
			context.Draw();
		}

		// 绘制ing over the 面板 backgrounds is not required.
		// 此示例 just showcases changing the "inner" part 的 条 panels to more closely resemble the example life fruit.
		private void DrawBarsPanelOverlay(ResourceOverlayDrawContext context) {
			// 绘制 over the Bars middle life panels
			// "context" contains information 用于 draw the 资源
			// If you 想要 draw directly on 顶部 的 vanilla 条 panels, just 替换 the 纹理 and have the context draw the new 纹理
			context.texture = barsPanelTexture ??= ModContent.Request<Texture2D>("ExampleMod/Common/UI/ResourceOverlay/BarsManaOverlay_Panel");
			// Due 到 replacement 纹理 and the vanilla 纹理 having different heights, the source needs to also be modified
			context.source = context.texture.Frame();
			// The original 位置 refers 到 entire 面板 slice.
			// 然而, since this overlay only modifies the "inner" portion 的 slice (aka the part behind the 条 filling),
			// the 位置 应该 modified to compensate 对于 精灵 大小 difference
			context.position.Y += 6;
			context.Draw();
		}
	}
}
