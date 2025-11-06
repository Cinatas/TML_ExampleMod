using ExampleMod.Content.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ExampleMod.Common
{
	public class ExamplePlayerDrawLayer : PlayerDrawLayer
	{
		// 在此属性中返回 真 可使此层出现在小地图玩家头部图标上。
		public override bool IsHeadLayer => true;

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			// 仅当玩家手持 ExampleItem 时，该层才可见。或者如果另一个模组作者强制使此层可见。
			return drawInfo.drawPlayer.HeldItem?.type == ModContent.ItemType<ExampleItem>();

			// 如果你想引用另一个 PlayerDrawLayer 的可见性，
			// 你可以通过 ModContent.GetInstance<OtherDrawLayer>() 获取其实例，并在其上调用 GetDefaultVisibility
		}

		// 此层将是头部层的子层，并在其之前（下方）绘制。
		// 如果隐藏头部层，此层也将被隐藏。
		// 如果移动头部层，此层将随之移动。
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);
		// 如果要创建不是另一个层的子层的层，请使用 `new Between(Layer1, Layer2)` 指定位置。
		// 如果要创建可根据 drawInfo 在不同位置渲染的移动层，请使用 `Multiple` 位置。

		protected override void Draw(ref PlayerDrawSet drawInfo) {
			// 以下代码在玩家头部后面绘制 ExampleItem 的纹理。
			var exampleItemTexture = TextureAssets.Item[ModContent.ItemType<ExampleItem>()];

			var position = drawInfo.Center + new Vector2(0f, -20f) - Main.screenPosition;
			position = new Vector2((int)position.X, (int)position.Y); // 你有时会想这样做，以避免抖动。

			// 将精灵的绘制排队。不要在绘制层中使用 SpriteBatch！
			drawInfo.DrawDataCache.Add(new DrawData(
				exampleItemTexture.Value, // 要渲染的纹理。
				position, // 渲染位置。
				null, // 源矩形。
				Color.White, // 颜色。
				0f, // 旋转。
				exampleItemTexture.Size() * 0.5f, // 原点. Uses the 纹理's 中心.
				1f, // 缩放。
				SpriteEffects.None, // 精灵效果。
				0 // 层。在 Terraria 中始终为 0。
			));
		}
	}
}
