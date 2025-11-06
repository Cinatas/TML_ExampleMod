using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalItems
{
	// 这是 ExampleShiftClickSlotPlayer.cs 的另一部分，为凝胶添加工具提示行
	public class GelGlobalItem : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Gel;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
			// 在这里我们为凝胶添加工具提示，让玩家知道会发生什么
			tooltips.Add(new(Mod, "SpecialShiftClick", "Shift-click on this item from your inventory to get a random color and rarity!"));
		}
	}
}
