using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace ExampleMod.Common
{
	// ModMapLayers 用于在地图上绘制图标和其他内容。晶塔和出生点/床位图标是原版地图层的示例。此示例在地牢上添加一个图标。
	public class ExampleMapLayer : ModMapLayer
	{
		// 在 Draw 方法中，我们绘制所有内容。查阅源代码中的原版示例是正确使用此 Draw 方法的好资源。
		public override void Draw(ref MapOverlayDrawContext context, ref string text) {
			// 在这里，我们定义当图标被悬停和未被悬停时绘制图标的比例。
			const float scaleIfNotSelected = 1f;
			const float scaleIfSelected = scaleIfNotSelected * 2f;

			// 在这里，我们检索骷髅王 boss 头部的纹理以便我们可以绘制它。请记住，并非所有纹理默认都会加载，因此你可能需要在代码中执行类似 `Main.instance.LoadItem(ItemID.BoneKey);` 的操作以确保纹理已加载。
			var dungeonTexture = TextureAssets.NpcHeadBoss[19].Value;

			// 此处使用的 MapOverlayDrawContext.Draw 方法处理绘制图标的许多小细节，应尽可能使用。它将处理缩放、对齐、剔除、帧和地图缩放。手动处理这些是很多工作。
			// 请注意，`position` 参数期望以 Vector2 表示的图格坐标。不要通过乘以 16 将图格坐标缩放到世界坐标。
			// MapOverlayDrawContext.Draw 的返回值有一个字段，指示鼠标当前是否在我们的图标上。
			if (context.Draw(dungeonTexture, new Vector2(Main.dungeonX, Main.dungeonY), Color.White, new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected, Alignment.Center).IsMouseOver) {
				// 当图标被用户鼠标悬停时，我们将鼠标文本设置为"The Dungeon"的本地化文本
				text = Language.GetTextValue("Bestiary_Biomes.TheDungeon");
			}
		}
	}

	// The game doesn't send Main.dungeonX or Main.dungeonY to multiplayer clients.
	// This ModSystem will ensure that they are synced allowing ExampleMapLayer to work in multiplayer.
	public class ExampleMapLayerSystem : ModSystem
	{
		public override void NetSend(BinaryWriter writer) {
			writer.Write(Main.dungeonX);
			writer.Write(Main.dungeonY);
		}
		public override void NetReceive(BinaryReader reader) {
			Main.dungeonX = reader.ReadInt32();
			Main.dungeonY = reader.ReadInt32();
		}
	}
}