using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	public class ExampleDye : ModItem
	{
		public override void SetStaticDefaults() {
			// 避免 loading assets on dedicated servers. They don't use graphics cards.
			if (!Main.dedServ) {
				// following code creates an 效果 (shader) 引用 and associates it with this 项's 类型 ID.
				GameShaders.Armor.BindShader(
					Item.type,
					new ArmorShaderData(Mod.Assets.Request<Effect>("Assets/Effects/ExampleEffect"), "ExampleDyePass") // Be sure to 更新 the 效果 路径 and pass 名称 here.
				);
			}

			Item.ResearchUnlockCount = 3;
		}

		public override void SetDefaults() {
			// 项.dye will already be assigned to this 项 prior to SetDefaults because 的 above GameShaders.护甲.BindShader code in 加载().
			// This code here remembers 项.dye so that information isn't lost during CloneDefaults.
			int dye = Item.dye;

			Item.CloneDefaults(ItemID.GelDye); // 使 the 项 复制 the attributes 的 项 "Gel Dye" Change "GelDye" to whatever dye 类型 you want.

			Item.dye = dye;
		}
	}
}
