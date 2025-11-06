using ExampleMod.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Hairs
{
	// 基于 Player_Hair_88 and Player_Hair_98
	// 注意 that internal hair ids are 1 less than the texture filename
	public class ExampleHair : ModHair
	{
		// This determines what gender the character 将 when randomizing during character creation.
		// 请记住 that hairstyles 可以 used with any gender; this property just allows for defining behavior during randomization.
		// 如果 not set by us like here, Gender.Unspecified 将 used instead, which randomizes the gender independent 的 hairstyle.
		// This 可能 particularly useful if your hairstyle doesn't lean either way.
		public override Gender RandomizedCharacterCreationGender => Gender.Female;

		// This determines whether the hairstyle will appear 在 character creation UI.
		// 你 may run into cases where you want special cases for showing hairstyles here,
		// or to outright disable showing it like in our case where we want it exclusively in-game.
		public override bool AvailableDuringCharacterCreation => false;

		public override void SetStaticDefaults() {
			HairID.Sets.DrawBackHair[Type] = true;
		}

		// These conditions determine whether our hair is available in-game 在 Stylist UI.
		// These conditions are not used to determine if the hairstyle is available during character creation.
		// 参见 AvailableDuringCharacterCreation for that.
		public override IEnumerable<Condition> GetUnlockConditions() {
			yield return ExampleConditions.InExampleBiome;
		}
	}
}
