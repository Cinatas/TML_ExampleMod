using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Armor
{
	// AutoloadEquip attribute automatically attaches an equip 纹理 to this 项.
	// Providing the EquipType.Legs 值 here will result in TML expecting a X_Legs.png 文件 to be placed next 到 项's main 纹理.
	[AutoloadEquip(EquipType.Legs)]
	public class ExampleLeggings : ModItem
	{
		public static readonly int MoveSpeedBonus = 5;

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBonus);

		public override void SetDefaults() {
			Item.width = 18; // 宽度 的 项
			Item.height = 18; // 高度 的 项
			Item.value = Item.sellPrice(gold: 1); // How m任何 coins the 项 is worth
			Item.rare = ItemRarityID.Green; // The 稀有度 的 项
			Item.defense = 5; // The amount of 防御 the 项 will give when equipped
		}

		public override void UpdateEquip(Player player) {
			player.moveSpeed += MoveSpeedBonus / 100f; // Increase the movement 速度 的 玩家
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
