using ExampleMod.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Hairs
{
	// 基于 Player_Hair_88 and Player_Hair_98
	// 注意 that internal hair ids are 1 less than the 纹理 filename
	public class ExampleHair : ModHair
	{
		// This determines what gender the character 将 when randomizing during character creation.
		// 请记住 that hairstyles 可以 used with 任何 gender; this 属性 just allows for defining behavior during randomization.
		// 如果 not set by us like here, Gender.Unspecified 将 used instead, which randomizes the gender independent 的 hairstyle.
		// This 可能 particularly useful if your hairstyle doesn't lean 任一 way.
		public override Gender RandomizedCharacterCreationGender => Gender.Female;

		// This determines whether the hairstyle will appear 在 character creation 用户界面.
		// 你 may run into cases where you want special cases for showing hairstyles here,
		// or to outright 禁用 showing it like in our case where we want it exclusively in-game.
		public override bool AvailableDuringCharacterCreation => false;

		public override void SetStaticDefaults() {
			HairID.Sets.DrawBackHair[Type] = true;
		}

		// These conditions determine whether our hair is available in-game 在 Stylist 用户界面.
		// These conditions are not 用于 determine if the hairstyle is available during character creation.
		// 参见 AvailableDuringCharacterCreation for that.
		public override IEnumerable<Condition> GetUnlockConditions() {
			yield return ExampleConditions.InExampleBiome;
		}
	}
}
