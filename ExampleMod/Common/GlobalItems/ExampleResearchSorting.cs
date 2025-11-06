using ExampleMod.Content.DamageClasses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalItems
{
	// 此文件展示了如何在旅程模式的复制菜单中创建和设置自己的分类组，以及如何更改现有物品的分类组。
	// 如果你的模组有特定的自定义物品类型，或者原版的分类方法没有为你的物品分配正确的组，创建自己的研究分类组会很有用。
	// 虽然你可以在 ModItem 中执行此操作，但使用 GlobalItem 批量将所有模组物品添加到分类组中有好处，如此处所示。
	public class ExampleResearchSorting : GlobalItem
	{
		// 在这里，我们将使用 ExampleDamageClass 伤害类的每个武器添加到单个自定义分类组中。我们还将现有物品铜短剑添加到原版分类组中。
		// 这些可以互换，模组物品可以放入原版分类组中，反之亦然。
		public override void ModifyResearchSorting(Item item, ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
			if (item.DamageType.CountsAsClass<ExampleDamageClass>()) {
				itemGroup = (ContentSamples.CreativeHelper.ItemGroup)531; // 此数字是物品排序相对于原版或模组添加的任何其他排序的位置；此处设置的 531 位于 MeleeWeapon 和 RangedWeapon 排序之间。要了解自定义组与原版排序数字的关系，请参考原版 ItemGroup 类，如果使用 Visual Studio，可以通过按 F12 轻松访问。
			}

			if (item.type == ItemID.CopperShortsword) {
				itemGroup = ContentSamples.CreativeHelper.ItemGroup.EventItem; // 将铜短剑的默认分类更改为与事件物品一起，而不是近战武器。
				// 原版已经有许多默认的研究分类组，你可以将物品添加到其中。通常会自动完成，但有一些例外。有关例外的示例，请参考 ExampleFishingCrate 文件。
			}

			// 想要快速检查其模组中物品的当前物品分类组值的模组作者可以使用以下代码片段，然后检查 client.log。
			/*
			if (item.ModItem?.Mod == Mod) {
				Mod.Logger.Info($"{item.ModItem.Name}: {itemGroup}");
			}
			*/
		}
	}
}
