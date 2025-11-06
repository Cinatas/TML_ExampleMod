using ExampleMod.Content.Projectiles;
using ExampleMod.Content.Rarities;
using ExampleMod.Content.Tiles.Furniture;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleYoyo : ModItem
	{
		public override void SetStaticDefaults() {
			// These are all related to gamepad controls and don't seem to affect anything else
			ItemID.Sets.Yoyo[Item.type] = true; // 使用d to increase the gamepad 范围 when using Strings.
			ItemID.Sets.GamepadExtraRange[Item.type] = 15; // Increases the gamepad 范围. Some vanilla values: 4 (Wood), 10 (Valor), 13 (Yelets), 18 (The Eye of Cthulhu), 21 (Terrarian).
			ItemID.Sets.GamepadSmartQuickReach[Item.type] = true; // Unused, but weapons that require aiming 在 屏幕 are in this set.
		}

		public override void SetDefaults() {
			Item.width = 24; // The 宽度 的 项's hitbox.
			Item.height = 24; // The 高度 的 项's hitbox.

			Item.useStyle = ItemUseStyleID.Shoot; // The way the 项 is used (e.g. swinging, throwing, etc.)
			Item.useTime = 25; // All vanilla yoyos have a useTime of 25.
			Item.useAnimation = 25; // All vanilla yoyos have a useAnimation of 25.
			Item.noMelee = true; // This makes it so the 项 doesn't do 伤害 to enemies (the 弹幕 does that).
			Item.noUseGraphic = true; // 使 the 项 invisible while using it (the 弹幕 is the visible part).
			Item.UseSound = SoundID.Item1; // The 声音 that will play when the 项 is used.

			Item.damage = 40; // The amount of 伤害 the 项 does to an 敌人 or 玩家.
			Item.DamageType = DamageClass.MeleeNoSpeed; // The 类型 of 伤害 the 武器 does. MeleeNoSpeed means the 项 will not 缩放 with 攻击 速度.
			Item.knockBack = 2.5f; // The amount of knockback the 项 inflicts.
			Item.crit = 8; // The percent 概率 对于 武器 to deal a critical strike. Defaults to 4.
			Item.channel = true; // 设置 to 真 for items that require the 攻击 按钮 to be held out (e.g. yoyos and magic missile weapons)
			Item.rare = ModContent.RarityType<ExampleModRarity>(); // The 项's 稀有度. This changes the 颜色 的 项's 名称.
			Item.value = Item.buyPrice(gold: 1); // The amount of money th在 项 is 可以 bought for.

			Item.shoot = ModContent.ProjectileType<ExampleYoyoProjectile>(); // Which 弹幕 this 项 will shoot. We set this to our corresponding 弹幕.
			Item.shootSpeed = 16f; // The 速度 的 shot 弹幕.			
		}

		// 在这里 is an example of blacklisting certain modifiers. 删除 this section for standard vanilla behavior.
		// 在 this example, we are blacklisting the ones that reduce 伤害 of a melee 武器.
		// 确保 that your 项 can even receive these prefixes (check the vanilla wiki on prefixes).
		private static readonly int[] unwantedPrefixes = new int[] { PrefixID.Terrible, PrefixID.Dull, PrefixID.Shameful, PrefixID.Annoying, PrefixID.Broken, PrefixID.Damaged, PrefixID.Shoddy };

		public override bool AllowPrefix(int pre) {
			// 返回 假 to make the game reroll the 前缀.

			// DON'T DO THIS BY ITSELF:
			// 返回 假;
			// This will get the game stuck because it will try to reroll 每次. Instead, make it have a 概率 to 返回 真.

			if (Array.IndexOf(unwantedPrefixes, pre) > -1) {
				// IndexOf returns a positive 索引 的 元素 you 搜索 for. If not found, it's less than 0.
				// 在这里 we check if the selected 前缀 is positive (it was found).
				// 如果 so, we found a 前缀 that we don't want. Reroll.
				return false;
			}

			// 不要 reroll
			return true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<ExampleWorkbench>()
				.Register();
		}
	}
}
