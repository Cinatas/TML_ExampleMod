using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

// 注意：此点以下是自定义配置 UI 元素。
// 请注意，使用自定义配置元素的模组将在接下来的几个 tModLoader 更新中中断，直到它们的设计最终确定。
// 如果你使用这些，你需要非常积极地更新你的模组，因为它们可能在任何更新中中断。

// 此文件定义基于 Corner 枚举的自定义 ConfigElement
// with custom drawing implemented that 可以 used in ModConfig classes.
namespace ExampleMod.Common.Configs.CustomUI
{
	// 此自定义配置 UI 元素展示了一个完全自定义的配置元素，除了自定义绘制外，还处理设置和获取值。
	[JsonConverter(typeof(StringEnumConverter))]
	[CustomModConfigItem(typeof(CornerElement))]
	public enum Corner
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	class CornerElement : ConfigElement
	{
		Texture2D circleTexture;
		string[] valueStrings;

		public override void OnBind() {
			base.OnBind();
			circleTexture = Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			valueStrings = Enum.GetNames(MemberInfo.Type);
			TextDisplayFunction = () => Label + ": " + GetStringValue();
		}

		void SetValue(Corner value) => SetObject(value);

		Corner GetValue() => (Corner)GetObject();

		string GetStringValue() {
			return valueStrings[(int)GetValue()];
		}

		public override void LeftClick(UIMouseEvent evt) {
			base.LeftClick(evt);
			SetValue(GetValue().NextEnum());
		}

		public override void RightClick(UIMouseEvent evt) {
			base.RightClick(evt);
			SetValue(GetValue().PreviousEnum());
		}

		public override void Draw(SpriteBatch spriteBatch) {
			base.Draw(spriteBatch);
			CalculatedStyle dimensions = GetDimensions();
			var circleSourceRectangle = new Rectangle(0, 0, (circleTexture.Width - 2) / 2, circleTexture.Height);
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(dimensions.X + dimensions.Width - 25), (int)(dimensions.Y + 4), 22, 22), Color.LightGreen);
			Corner corner = GetValue();
			var circlePositionOffset = new Vector2((int)corner % 2 * 8, (int)corner / 2 * 8);
			spriteBatch.Draw(circleTexture, new Vector2(dimensions.X + dimensions.Width - 25, dimensions.Y + 4) + circlePositionOffset, circleSourceRectangle, Color.White);
		}
	}
}
