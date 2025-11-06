using ExampleMod.Content.Tiles;
using Terraria.Enums;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	/// <summary>
	/// The coupled 项 that places the Example Pylon 图格. F或更多 information on said 图格,
	/// see <seealso cref="ExamplePylonTile"/>.
	/// </summary>
	public class ExamplePylonItem : ModItem
	{
		public override void SetDefaults() {
			// 基本ally, this a just a shorthand 方法 that will set all default values necessary to place
			// the passed in 图格 类型; in this case, the Example Pylon 图格.
			Item.DefaultToPlaceableTile(ModContent.TileType<ExamplePylonTile>());

			// Another shorthand 方法 that will set the 稀有度 and how much the 项 is worth.
			Item.SetShopValues(ItemRarityColor.Blue1, Terraria.Item.buyPrice(gold: 10));
		}
	}
}
