using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.Items.ExampleDamageClass
{
	public class ExampleResourceStaff : ExampleDamageItem
	{
		// 这是一个 staff that uses the example 伤害 类 stuff you've set up before, but uses exampleResource 代替 魔力.
		// 这是一个 very simple way of doing it, and if you plan on 多个 items using exampleResource then I'd suggest making a new abstract ModItem 类 that inherits ExampleDamageItem,
		// and doing the CanUseItem and UseItem in a more generalized way there, so you can just define the 资源 usage in SetDefaults and it'll do it automatically for you.
		public override void SetStaticDefaults() {
			Item.staff[item.type] = true;
		}

		public override void SafeSetDefaults() {
			item.CloneDefaults(ItemID.AmethystStaff);
			item.Size = new Vector2(28, 36);
			item.damage = 32;
			item.knockBack = 3;
			item.rare = ItemRarityID.Red;
			item.mana = 0; // 使 sure to nullify the 魔力 usage 的 staff here, as it still copies the setdefaults 的 amethyst staff.
			item.useStyle = ItemUseStyleID.HoldingOut;

			// 示例ResourceCost is a 字段 在 base 类 ExampleDamageItem. This 项 consumes 10 Example 资源 to use.
			exampleResourceCost = 10;
		}
	}
}
