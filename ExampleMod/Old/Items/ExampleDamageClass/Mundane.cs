using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.Items.ExampleDamageClass
{
	public class Mundane : ExampleDamageItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.HellwingBow;

		// 调用ed when the mod loads, so our changes are added 到 game
		public static void AddHacks() {
			// 设置 ourselves to be ranged temporarily to benefit from ranged bonuses
			// This is needed because terraria changes the variables before calling tML's 方法
			// 基于 if the 项 was set to be ranged. Ours isn't, but we still want our custom bow
			// to benefit from ranged bonuses, despite being an Example 伤害 武器.
			// This is how to do it.
			On.Terraria.Player.GetWeaponDamage += PlayerOnGetWeaponDamage;
			On.Terraria.Player.GetWeaponKnockback += PlayerOnGetWeaponKnockback;
		}

		private static float PlayerOnGetWeaponKnockback(On.Terraria.Player.orig_GetWeaponKnockback orig, Player self, Item sitem, float knockback) {
			bool isMundane = sitem.type == ItemType<Mundane>();
			if (isMundane) sitem.ranged = true;

			float kb = orig(self, sitem, knockback);
			if (isMundane) sitem.ranged = false;
			return kb;
		}

		private static int PlayerOnGetWeaponDamage(On.Terraria.Player.orig_GetWeaponDamage orig, Player self, Item sitem) {
			bool isMundane = sitem.type == ItemType<Mundane>();
			if (isMundane) sitem.ranged = true;

			int dmg = orig(self, sitem);
			if (isMundane) sitem.ranged = false;
			return dmg;
		}

		// Our ExampleDamageItem abstract 类 handles all code 与...相关 our custom 伤害 类
		public override void SafeSetDefaults() {
			item.CloneDefaults(ItemID.WoodenBow);
			item.Size = new Vector2(18, 46);
			item.damage = 20;
			item.crit = 20;
			item.knockBack = 2;
			item.rare = ItemRarityID.Red;
		}

		public override void GetWeaponCrit(Player player, ref int crit) {
			// 它是 hard to hook into 每个 place checking 项's crit and fake 项.ranged = 真
			// 代替, 我们可以 mimick regular ranged crit assignment
			crit = Main.LocalPlayer.rangedCrit - Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem].crit + Main.HoverItem.crit;
			base.GetWeaponCrit(player, ref crit);
		}
	}
}