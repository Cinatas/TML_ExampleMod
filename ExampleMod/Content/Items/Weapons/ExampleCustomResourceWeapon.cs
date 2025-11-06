using ExampleMod.Common.Players;
using ExampleMod.Content.Tiles.Furniture;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	// Holding this 项 will cause the ExampleResourceBar 用户界面 to show, displaying the 玩家's custom 资源 amounts tracked in ExampleResourcePlayer.
	public class ExampleCustomResourceWeapon : ModItem
	{
		private int exampleResourceCost; // 添加 our custom 资源 成本

		public static LocalizedText UsesXExampleResourceText { get; private set; }

		public override void SetStaticDefaults() {
			UsesXExampleResourceText = this.GetLocalization("UsesXExampleResource");
		}

		public override void SetDefaults() {
			Item.damage = 130;
			Item.DamageType = DamageClass.Magic;
			Item.width = 38;
			Item.height = 38;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(gold: 15);
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.Item71;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.VortexBeaterRocket;
			Item.shootSpeed = 7;
			Item.crit = 32;
			exampleResourceCost = 5; // 设置 our custom 资源 成本 to 5
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			tooltips.Add(new TooltipLine(Mod, "ExampleResourceCost", UsesXExampleResourceText.Format(exampleResourceCost)));
		}

		// 确保 you can't use the 项 if you don't have enough 资源
		public override bool CanUseItem(Player player) {
			var exampleResourcePlayer = player.GetModPlayer<ExampleResourcePlayer>();

			return exampleResourcePlayer.exampleResourceCurrent >= exampleResourceCost;
		}

		// Reduce 资源 on use
		public override bool? UseItem(Player player) {
			var exampleResourcePlayer = player.GetModPlayer<ExampleResourcePlayer>();

			exampleResourcePlayer.exampleResourceCurrent -= exampleResourceCost;

			return true;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>(10)
				.AddTile<ExampleWorkbench>()
				.Register();
		}
	}
}
