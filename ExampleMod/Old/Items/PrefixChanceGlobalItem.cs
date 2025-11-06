using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ExampleMod.Items
{
	/// <summary>
	/// This 类 demonstrates how to manipulate the chances of prefixes given to items.
	/// For other 前缀 related hooks and their usage, 另请参阅:
	/// <seealso cref="Accessories.ManaHeart"/>
	/// <seealso cref="Weapons.ExampleYoyo"/>
	/// </summary>
	public class PrefixChanceGlobalItem : GlobalItem
	{
		public override bool? PrefixChance(Item item, int pre, UnifiedRandom rand) {
			// pre: The 前缀 being applied 到 项, or the roll 模式
			// -1 is when an 项 is naturally generated in a 箱子, crafted, purchased from an NPC, looted from a grab bag (excluding presents), or dropped by a slain 敌人
			// -2 is when an 项 is rolled 在 tinkerer
			// -3 determines if an 项 可以 placed 在 tinkerer 槽位

			// To 防止 putting an 项 在 tinkerer 槽位, 返回 假 when pre is -3
			if (pre == -3 && item.type == ItemID.LaserRifle) {
				// 这将 make the Laser Rifle 不 reforgeable at all (useful if you want your 项 to preserve its custom 名称 颜色)
				return false;
			}

			// To make an 项 重置 its 前缀 when reforging
			if (pre == -2) {
				if (Main.LocalPlayer.HasBuff(BuffID.Cursed)) {
					// If the 玩家 is cursed, make it 删除 the 前缀
					return false;
				}
			}

			// To 防止 rolling of a 前缀 on 生成, 返回 假 when pre is -1
			if (pre == -1) {
				if (item.melee && item.modItem?.mod == mod) {
					// All melee weapons from ExampleMod won't have a 前缀 when they are crafted, bought, taken from a generated 箱子, opened, or dropped by an 敌人
					return false;
				}
			}

			// 对于 following code, this is useful to know (从 terraria wiki):
			// Nearly all weapons and accessories have a 75% 概率 of receiving a 随机 修饰符 up在 项's creation
			// (naturally generated in a 箱子, crafted, purchased from an NPC, looted from a grab bag (excluding presents), or dropped by a slain 敌人).

			// To change the 概率 of a 前缀 being rolled or not, 返回 真 or 假 取决于 some 条件
			if (pre == -1 && item.type == ItemID.Shackle) {
				// Force rolling
				// 返回 真;

				// When using 随机 numbers, 确保 to use the rand 对象 passed into this 方法, and not Main.rand.
				// 这将 make it consistent with worldgen should this 项 be spawned in a 箱子
				if (rand.NextFloat() < 0.5f) {
					// Increase the 概率 of not receiving 任何 前缀 on 生成 by 50%
					return false;
				}
				// Keep in mind that if the code arrives here, there is still a 25% 概率 that it won't get a 修饰符.
				// If you want a more controlled approach, 返回 真 in an else 方块
			}

			return null;
		}
	}
}
