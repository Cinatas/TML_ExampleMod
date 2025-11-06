using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ExampleMod.Common.Configs
{
	public class ExampleModConfig : ModConfig
	{
		// ConfigScope.ClientSide 应该 used for client side, usually visual or audio tweaks.
		// ConfigScope.ServerSide 应该 used for basically everything else, including disabling items or changing NPC behaviors
		public override ConfigScope Mode => ConfigScope.ServerSide;

		// The things in brackets are known as "Attributes".

		[Header("Items")] // Headers are like titles in a config. You only need to declare a header 在 item it should appear over, not every item 在 category. 
		// [Label("$Some.Key")] // A label is the text displayed next 到 option. This should usually be a short description of what it does. 默认情况下 all ModConfig fields and properties have an automatic label translation key, but modders can specify a specific translation key.
		// [Tooltip("$Some.Key")] // A tooltip is a description showed when you hover your mouse over the option. It 可以 used as a more in-depth explanation 的 option. Like with Label, a specific key 可以 provided.
		[DefaultValue(true)] // This sets the configs default value.
		[ReloadRequired] // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It 应该 used for things like item toggles, which only take effect during mod loading
		public bool ExampleWingsToggle; // To see the implementation of this option, see ExampleWings.cs

		[ReloadRequired]
		public bool WeaponWithGrowingDamageToggle;
	}
}
