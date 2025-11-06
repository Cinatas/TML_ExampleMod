using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Tiles
{
	public class ExampleTreeLeaf : ModGore
	{
		public override string Texture => "ExampleMod/Content/Tiles/Plants/ExampleTree_Leaf";

		public override void SetStaticDefaults() {
			ChildSafety.SafeGore[Type] = true; // Leaf gore should appear regardless 的 "Blood and Gore" setting
			GoreID.Sets.SpecialAI[Type] = 3; // Falling leaf behavior
			GoreID.Sets.PaintedFallingLeaf[Type] = true; // This is used for all vanilla tree leaves, related 到 bigger spritesheet for tile paints
		}
	}
}
