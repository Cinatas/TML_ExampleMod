using ExampleMod.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Tools
{
	// 示例Drill closely mimics Titanium Drill, except where noted.
	// Of note, this example showcases Item.tileBoost and teaches the basic concepts of a held projectile.
	public class ExampleDrill : ModItem
	{
		public override void SetStaticDefaults() {
			// As mentioned 在 documentation, IsDrill and IsChainsaw automatically reduce useTime and useAnimation to 60% of what is set in SetDefaults and decrease tileBoost by 1, but only for vanilla items.
			// 我们 set it here despite it doing nothing because it is likely to be used by other mods to provide special effects to drill or chainsaw items globally.
			ItemID.Sets.IsDrill[Type] = true;
		}

		public override void SetDefaults() {
			Item.damage = 27;
			Item.DamageType = DamageClass.MeleeNoSpeed; // ignores melee speed bonuses. There's no need for drill animations to play faster, nor drills to dig faster with melee speed.
			Item.width = 20;
			Item.height = 12;
			// IsDrill/IsChainsaw effects 必须 applied manually, so 60% or 0.6 times the time 的 corresponding pickaxe. In this case, 60% of 7 is 4 and 60% of 25 is 15.
			// 如果 you decide to copy values from vanilla drills or chainsaws, you should multiply each one by 0.6 to get the expected behavior.
			Item.useTime = 4;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0.5f;
			Item.value = Item.buyPrice(gold: 12, silver: 60);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item23;
			Item.shoot = ModContent.ProjectileType<ExampleDrillProjectile>(); // 创建 the drill projectile
			Item.shootSpeed = 32f; // Adjusts how far away 从 player to hold the projectile
			Item.noMelee = true; // Turns off damage 从 item itself, as we have a projectile
			Item.noUseGraphic = true; // Stops the item from drawing in your hands, 对于 aforementioned reason
			Item.channel = true; // 重要 as the projectile checks if the player channels

			// tileBoost changes the range of tiles th在 item can reach.
			// 要 match Titanium Drill, we should set this to -1, but we'll set it to 10 blocks of extra range 对于 sake of an example.
			Item.tileBoost = 10;

			Item.pick = 190; // How strong the drill is, see https://terraria.wiki.gg/wiki/Pickaxe_power for a list of common values
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
