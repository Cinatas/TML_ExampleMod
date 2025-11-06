using ExampleMod.Content.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleSpear : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.SkipsInitialUseSound[Item.type] = true; // This skips use 动画-tied 声音 playback, 以便 we're 能够 make it be tied to use 时间 instead 在 UseItem() hook.
			ItemID.Sets.Spears[Item.type] = true; // 这允许 游戏 to recognize our new 项 as a spear.
		}

		public override void SetDefaults() {
			// 常见 Properties
			Item.rare = ItemRarityID.Pink; // Assign this 项 a 稀有度 级别 of Pink
			Item.value = Item.sellPrice(silver: 10); // The 数字 and 类型 of coins 项 可以 sold for to an NPC

			// 使用 Properties
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the 项 (swinging, holding out, etc.)
			Item.useAnimation = 12; // The 长度 的 项's use 动画 in ticks (60 ticks == 1 second.)
			Item.useTime = 18; // The 长度 的 项's use 时间 in ticks (60 ticks == 1 second.)
			Item.UseSound = SoundID.Item71; // The 声音 that this 项 plays when used.
			Item.autoReuse = true; // 允许s the 玩家 to hold 点击 to automatically use the 项 again. Most spears don't autoReuse, but it's possible when used in conjunction with CanUseItem()

			// 武器 Properties
			Item.damage = 25;
			Item.knockBack = 6.5f;
			Item.noUseGraphic = true; // When 真, the 项's 精灵 will 不 visible while the 项 is in use. This is 真 because the spear 弹幕 is what's shown so we do not 想要 show the spear 精灵 以及.
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true; // 允许s the 项's 动画 to do 伤害. This is important because the spear is actually a 弹幕 代替 an 项. 这防止 the melee hitbox of this 项.

			// 弹幕 Properties
			Item.shootSpeed = 3.7f; // The 速度 的 弹幕 measured in pixels per 帧.
			Item.shoot = ModContent.ProjectileType<ExampleSpearProjectile>(); // The 弹幕 即 fired from this 武器
		}

		public override bool CanUseItem(Player player) {
			// 确保s no 超过 one spear 可以 thrown out, use this when using autoReuse
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}

		public override bool? UseItem(Player player) {
			// Because we're skipping 声音 playback on use 动画 开始, we 必须 play it ourselves whenever the 项 is actually used.
			if (!Main.dedServ && Item.UseSound.HasValue) {
				SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
			}

			return null;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
