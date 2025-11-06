using ExampleMod.Content.Biomes;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalNPCs
{
	public class ExampleNPCHappiness : GlobalNPC
	{
		public override void SetStaticDefaults() {
			int examplePersonType = ModContent.NPCType<Content.NPCs.ExamplePerson>(); // 获取 ExamplePerson's type
			var guideHappiness = NPCHappiness.Get(NPCID.Guide); // 获取 the key into The Guide's happiness

			guideHappiness.SetNPCAffection(examplePersonType, AffectionLevel.Love); // Make the Guide love ExamplePerson!

			guideHappiness.SetBiomeAffection<ExampleSurfaceBiome>(AffectionLevel.Love);  // Make the Guide love ExampleSurfaceBiome!
		}
	}
}
