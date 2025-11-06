using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Consumables
{
	public class ExampleFoodItem : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 5;

			// 这是 to show the correct 帧 在 库存
			// MaxValue 参数 is 对于 动画 速度, we want it to be stuck on 帧 1
			// 设置ting it to max 值 will cause it to take 414 days to reach the next 帧
			// No one is going to have game 打开 that long so this is fine
			// second 参数 is the 数字 of frames, 即 3
			// first 帧 is the 库存 纹理, the second 帧 is the holding 纹理,
			// and the third 帧 is the placed 纹理
			Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

			// This allows you to change the 颜色 的 crumbs that are created when you eat.
			// numbers are RGB (Red, Green, and Blue) values which 范围 from 0 to 255.
			// Most foods have 3 crumb colors, but you can use more or less if you desire.
			// Depending on if you are making solid or liquid food switch out FoodParticleColors
			// with DrinkParticleColors. The difference is that food particles fly outwards
			// whereas drink particles fall straight down and are slightly transparent
			ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
				new Color(249, 230, 136),
				new Color(152, 93, 95),
				new Color(174, 192, 192)
			};

			ItemID.Sets.IsFood[Type] = true; //This allows it to be placed on a plate and held correctly
		}

		public override void SetDefaults() {
			// This code matches the ApplePie code.

			// 默认ToFood sets all 的 food related 项 defaults 例如 the 增益 类型, 增益 持续时间, use 声音, and 动画 时间.
			Item.DefaultToFood(22, 22, BuffID.WellFed3, 57600); // 57600 is 16 minutes: 16 * 60 * 60
			Item.value = Item.buyPrice(0, 3);
			Item.rare = ItemRarityID.Blue;
		}

		// 如果 you want multiple buffs, you can apply the remainder of buffs with this 方法.
		// 确保 the primary 增益 is set in SetDefaults so th在 QuickBuff hotkey can work properly.
		public override void OnConsumeItem(Player player) {
			player.AddBuff(BuffID.SugarRush, 3600);
		}

		//Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddIngredient(ItemID.Apple, 3)
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}