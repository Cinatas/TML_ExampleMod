using ExampleMod.Common.Configs.CustomDataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using Terraria.GameContent;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

// 注意：此点以下是自定义配置 用户界面 元素。
// 请注意，使用自定义配置元素的模组将在接下来的几个 tModLoader 更新中中断，直到它们的设计最终确定。
// 如果你使用这些，你需要非常积极地更新你的模组，因为它们可能在任何更新中中断。

// 此文件定义基于渐变数据类型的自定义 ConfigElement
// 实现了可在 ModConfig 类中使用的自定义绘制。
namespace ExampleMod.Common.Configs.CustomUI
{
	// 此自定义配置 用户界面 元素使用原版配置元素与自定义绘制配对。
	class GradientElement : ConfigElement
	{
		public override void OnBind() {
			base.OnBind();

			object subitem = MemberInfo.GetValue(Item);

			if (subitem == null) {
				subitem = Activator.CreateInstance(MemberInfo.Type);
				JsonConvert.PopulateObject("{}", subitem, ConfigManager.serializerSettings);
				MemberInfo.SetValue(Item, subitem);
			}

			// 项 是所有者对象实例，MemberInfo 是 项 中此字段的信息

			int height = 30;
			int order = 0;

			foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(subitem)) {
				var wrapped = ConfigManager.WrapIt(this, ref height, variable, subitem, order++);

				if (List != null) {
					wrapped.Item1.Left.Pixels -= 20;
					wrapped.Item1.Width.Pixels += 20;
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch) {
			base.Draw(spriteBatch);
			var hitbox = GetInnerDimensions().ToRectangle();
			if (MemberInfo.GetValue(Item) is Gradient g) {
				int left = (hitbox.Left + hitbox.Right) / 2;
				int right = hitbox.Right;
				int steps = right - left;
				for (int i = 0; i < steps; i += 1) {
					float percent = (float)i / steps;
					spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, 30), Color.Lerp(g.start, g.end, percent));
				}

				//Main.spriteBatch.Draw(TextureAssets.MagicPixel.值, new Rectangle(hitbox.X + hitbox.宽度 / 2, hitbox.Y, hitbox.宽度 / 4, 30), g.开始);
				//Main.spriteBatch.Draw(TextureAssets.MagicPixel.值, new Rectangle(hitbox.X + 3 * hitbox.宽度 / 4, hitbox.Y, hitbox.宽度 / 4, 30), g.结束);
			}
		}
	}
}
