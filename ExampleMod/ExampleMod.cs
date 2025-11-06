using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace ExampleMod
{
	// 这是一个部分类，意味着它的一些部分被拆分到其他文件中。请查看 ExampleMod.*.cs 以了解其他部分。
	// 该类是部分类，用于将相似的代码组织在一起，以便更清楚地了解相关内容。
	public partial class ExampleMod : Mod
	{
		public const string AssetPath = $"{nameof(ExampleMod)}/Assets/";

		public static int ExampleCustomCurrencyId;

		public override void Load() {
			// 注册一个新的自定义货币
			ExampleCustomCurrencyId = CustomCurrencyManager.RegisterCurrency(new Content.Currencies.ExampleCustomCurrency(ModContent.ItemType<Content.Items.ExampleItem>(), 999L, "Mods.ExampleMod.Currencies.ExampleCustomCurrency"));
		}

		public override void Unload() {
			// Unload() 方法可用于卸载/释放/清理特殊对象、取消订阅事件或撤销一些模组的操作。
			// 当某些模组对象可能仍然保留在原版程序集内时，请务必始终编写卸载代码。
			// 最常见的原因是使用了事件，而不是 On.* 和 IL.* 代码注入命名空间。
			// 如果你订阅了一个事件，请务必最终取消订阅它。

			// 注意：在编写卸载代码时，请务必使用“防御性编程”。换句话说，你应该始终假设你正在卸载的模组中的所有内容可能尚未初始化。
			// 注意：通常不需要将静态字段的值置为 空，因为 TML 旨在模组重载之间完全释放模组程序集。
		}
	}
}
