using ExampleMod.Content.Tiles;
using Terraria.Enums;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	/// <summary>
	/// The coupled 项 that places the Advanced Example Pylon 图格. F或更多 information on said 图格,
	/// see <seealso cref="ExamplePylonTileAdvanced"/>.
	/// </summary>
	public class ExamplePylonItemAdvanced : ModItem
	{
		public override void SetDefaults() {
			// 基本ally, this a just a shorthand 方法 that will set all default values necessary to place
			// the passed in 图格 类型; in this case, the Advanced Example Pylon 图格.
			Item.DefaultToPlaceableTile(ModContent.TileType<ExamplePylonTileAdvanced>());

			// Another shorthand 方法 that will set the 稀有度 and how much the 项 is worth.
			Item.SetShopValues(ItemRarityColor.LightRed4, Terraria.Item.buyPrice(gold: 20));
		}
	}
}
