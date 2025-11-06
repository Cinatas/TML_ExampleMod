using Terraria.ModLoader.Default;

namespace ExampleMod.Content.TileEntities
{
	/// <summary>
	/// This is an empty child 类 that acts exactly like the default implementation 的 abstract <seealso cref="TEModdedPylon"/>
	/// 类, which itself acts nearly identical to vanilla pylon TEs. This inheritance only exists so that modded pylon entities
	/// will properly have their "Mod" 属性 set, for I/O purposes. Has the sealed 修饰符 since this TE acts identical to its parent.
	/// </summary>
	public sealed class SimplePylonTileEntity : TEModdedPylon { }
}
