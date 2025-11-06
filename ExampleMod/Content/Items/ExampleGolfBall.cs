using ExampleMod.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	public class ExampleGolfBall : ModItem
	{
		public override void SetDefaults() {
			// 默认ToGolfBall sets various properties common to golf balls. 悬停 over DefaultToGolfBall in Visual Studio to see the specific properties set.
			// ModContent.ProjectileType<ExampleGolfBallProjectile>() is the 弹幕 即 placed 在 golf tee.
			Item.DefaultToGolfBall(ModContent.ProjectileType<ExampleGolfBallProjectile>());
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Golf;
		}
	}
}
