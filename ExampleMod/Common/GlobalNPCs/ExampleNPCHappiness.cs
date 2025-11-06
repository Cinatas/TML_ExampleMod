using ExampleMod.Content.Biomes;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalNPCs
{
	public class ExampleNPCHappiness : GlobalNPC
	{
		public override void SetStaticDefaults() {
			int examplePersonType = ModContent.NPCType<Content.NPCs.ExamplePerson>(); // 获取 ExamplePerson's 类型
			var guideHappiness = NPCHappiness.Get(NPCID.Guide); // 获取 the 键 in到 Guide's happiness

			guideHappiness.SetNPCAffection(examplePersonType, AffectionLevel.Love); // 使 the Guide love ExamplePerson!

			guideHappiness.SetBiomeAffection<ExampleSurfaceBiome>(AffectionLevel.Love);  // 使 the Guide love ExampleSurfaceBiome!
		}
	}
}
