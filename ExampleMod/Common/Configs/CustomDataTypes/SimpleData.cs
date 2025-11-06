using System.ComponentModel;
using Terraria.ModLoader.Config;

// 此文件定义包含各种简单数据类型的自定义数据类型
// 并且可以在 ModConfig 类中使用。
namespace ExampleMod.Common.Configs.CustomDataTypes
{
	[BackgroundColor(255, 7, 7)]
	public class SimpleData
	{
		[Header("FirstHeader")]
		public int boost;
		public float percent;

		[Header("SecondHeader")]
		public bool enabled;

		[DrawTicks]
		[OptionStrings(new string[] { "Pikachu", "Charmander", "Bulbasaur", "Squirtle" })]
		[DefaultValue("Bulbasaur")]
		public string FavoritePokemon;

		public SimpleData() {
			FavoritePokemon = "Bulbasaur";
		}

		public override bool Equals(object obj) {
			if (obj is SimpleData other)
				return boost == other.boost && percent == other.percent && enabled == other.enabled && FavoritePokemon == other.FavoritePokemon;
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return new { boost, percent, enabled, FavoritePokemon }.GetHashCode();
		}
	}
}
