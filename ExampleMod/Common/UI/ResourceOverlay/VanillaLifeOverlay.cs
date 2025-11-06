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
	public class VanillaLifeOverlay : ModResourceOverlay
	{
		// 此字段用于缓存此文件下方 CompareAssets 辅助方法中使用的原版资产
		private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

		// 这些字段用于缓存 ModContent.请求<Texture2D>() 的结果
		private Asset<Texture2D> heartTexture, fancyPanelTexture, barsFillingTexture, barsPanelTexture;

		public override void PostDrawResource(ResourceOverlayDrawContext context) {
			Asset<Texture2D> asset = context.texture;

			string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
			string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

			bool drawingBarsPanels = CompareAssets(asset, barsFolder + "HP_Panel_Middle");

			int exampleFruits = Main.LocalPlayer.GetModPlayer<ExampleStatIncreasePlayer>().exampleLifeFruits;

			// 生命资源以两个为一组绘制
			if (context.resourceNumber >= 2 * exampleFruits)
				return;

			// 注意：CompareAssets 在此方法主体下方定义
			if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2) {
				// 在经典心上绘制
				DrawClassicFancyOverlay(context);
			}
			else if (CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B")) {
				// 绘制 over the Fancy hearts
				DrawClassicFancyOverlay(context);
			}
			else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey")) {
				// 绘制 over the Bars life bars
				DrawBarsOverlay(context);
			}
			else if (CompareAssets(asset, fancyFolder + "Heart_Left") || CompareAssets(asset, fancyFolder + "Heart_Middle") || CompareAssets(asset, fancyFolder + "Heart_Right") || CompareAssets(asset, fancyFolder + "Heart_Right_Fancy") || CompareAssets(asset, fancyFolder + "Heart_Single_Fancy")) {
				// 绘制 over the Fancy heart panels
				DrawFancyPanelOverlay(context);
			}
			else if (drawingBarsPanels) {
				// 绘制 over the Bars middle life panels
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
			// 绘制 over the Classic / Fancy hearts
			// "context" contains information 用于 draw the 资源
			// If you 想要 draw directly on 顶部 的 vanilla hearts, just 替换 the 纹理 and have the context draw the new 纹理
			context.texture = heartTexture ??= ModContent.Request<Texture2D>("ExampleMod/Common/UI/ResourceOverlay/ClassicLifeOverlay");
			context.Draw();
		}

		// 绘制ing over the 面板 backgrounds is not required.
		// 此示例 just showcases changing the "inner" part 的 heart panels to more closely resemble the example life fruit.
		private void DrawFancyPanelOverlay(ResourceOverlayDrawContext context) {
			// 绘制 over the Fancy heart panels
			string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";

			// The original 位置 refers 到 entire 面板 slice.
			// 然而, since this overlay only modifies the "inner" portion 的 slice (aka the part behind the heart),
			// the 位置 应该 modified to compensate 对于 精灵 大小 difference
			Vector2 positionOffset;

			if (context.resourceNumber == context.snapshot.AmountOfLifeHearts - 1) {
				// Final 面板 to draw has a special "Fancy" variant.  Determine whether it has panels 到 左 of it
				if (CompareAssets(context.texture, fancyFolder + "Heart_Single_Fancy")) {
					// 首先 and only 面板 in this 面板's 行
					positionOffset = new Vector2(8, 8);
				}
				else {
					// Other panels existed in this 面板's 行
					// Vanilla 纹理 is "Heart_Right_Fancy"
					positionOffset = new Vector2(8, 8);
				}
			}
			else if (CompareAssets(context.texture, fancyFolder + "Heart_Left")) {
				// 首先 面板 in this 行
				positionOffset = new Vector2(4, 4);
			}
			else if (CompareAssets(context.texture, fancyFolder + "Heart_Middle")) {
				// Any 面板 that has a 面板 to its 左 AND 右
				positionOffset = new Vector2(0, 4);
			}
			else {
				// Final 面板 在 first 行
				// Vanilla 纹理 is "Heart_Right"
				positionOffset = new Vector2(0, 4);
			}

			// "context" contains information 用于 draw the 资源
			// If you 想要 draw directly on 顶部 的 vanilla hearts, just 替换 the 纹理 and have the context draw the new 纹理
			context.texture = fancyPanelTexture ??= ModContent.Request<Texture2D>("ExampleMod/Common/UI/ResourceOverlay/FancyLifeOverlay_Panel");
			// Due 到 replacement 纹理 and the vanilla 纹理 having different dimensions, the source needs to also be modified
			context.source = context.texture.Frame();
			context.position += positionOffset;
			context.Draw();
		}

		private void DrawBarsOverlay(ResourceOverlayDrawContext context) {
			// 绘制 over the Bars life bars
			// "context" contains information 用于 draw the 资源
			// If you 想要 draw directly on 顶部 的 vanilla bars, just 替换 the 纹理 and have the context draw the new 纹理
			context.texture = barsFillingTexture ??= ModContent.Request<Texture2D>("ExampleMod/Common/UI/ResourceOverlay/BarsLifeOverlay_Fill");
			context.Draw();
		}

		// 绘制ing over the 面板 backgrounds is not required.
		// 此示例 just showcases changing the "inner" part 的 条 panels to more closely resemble the example life fruit.
		private void DrawBarsPanelOverlay(ResourceOverlayDrawContext context) {
			// 绘制 over the Bars middle life panels
			// "context" contains information 用于 draw the 资源
			// If you 想要 draw directly on 顶部 的 vanilla 条 panels, just 替换 the 纹理 and have the context draw the new 纹理
			context.texture = barsPanelTexture ??= ModContent.Request<Texture2D>("ExampleMod/Common/UI/ResourceOverlay/BarsLifeOverlay_Panel");
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
