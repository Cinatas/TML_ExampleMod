using ExampleMod.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Tools
{
	// 示例Drill closely mimics Titanium Drill, except where noted.
	// Of note, this example showcases 项.tileBoost and teaches the basic concepts of a held 弹幕.
	public class ExampleDrill : ModItem
	{
		public override void SetStaticDefaults() {
			// As mentioned 在 documentation, IsDrill and IsChainsaw automatically reduce useTime and useAnimation to 60% of what is set in SetDefaults and decrease tileBoost by 1, but only for vanilla items.
			// 我们 set it here despite it doing nothing because it is likely to be used by other mods to provide special effects to drill or chainsaw items globally.
			ItemID.Sets.IsDrill[Type] = true;
		}

		public override void SetDefaults() {
			Item.damage = 27;
			Item.DamageType = DamageClass.MeleeNoSpeed; // ignores melee 速度 bonuses. There's no need for drill animations to play faster, nor drills to dig faster with melee 速度.
			Item.width = 20;
			Item.height = 12;
			// IsDrill/IsChainsaw effects 必须 applied manually, so 60% or 0.6 times the 时间 的 corresponding pickaxe. In this case, 60% of 7 is 4 and 60% of 25 is 15.
			// 如果 you decide to 复制 values from vanilla drills or chainsaws, you should multiply each one by 0.6 to get the expected behavior.
			Item.useTime = 4;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0.5f;
			Item.value = Item.buyPrice(gold: 12, silver: 60);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item23;
			Item.shoot = ModContent.ProjectileType<ExampleDrillProjectile>(); // 创建 the drill 弹幕
			Item.shootSpeed = 32f; // Adjusts how far away 从 玩家 to hold the 弹幕
			Item.noMelee = true; // Turns off 伤害 从 项 itself, as we have a 弹幕
			Item.noUseGraphic = true; // Stops the 项 from drawing in your hands, 对于 aforementioned reason
			Item.channel = true; // 重要 as the 弹幕 checks if the 玩家 channels

			// tileBoost changes the 范围 of tiles th在 项 can reach.
			// 要 匹配 Titanium Drill, we should set this to -1, but we'll set it to 10 blocks of extra 范围 对于 sake of an example.
			Item.tileBoost = 10;

			Item.pick = 190; // How strong the drill is, see https://terraria.wiki.gg/wiki/Pickaxe_power for a 列表 of common values
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
