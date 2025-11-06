using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Back)]
	public class WaspNest : ModItem
	{
		// 仅 gets run once per 类型
		public override void Load() {
			IL_Player.beeType += HookBeeType;
		}

		// This IL editing (Intermediate Language editing) example is walked through 在 guide: https://github.com/tModLoader/tModLoader/wiki/Expert-IL-Editing#example---hive-pack-升级
		private static void HookBeeType(ILContext il) {
			try {
				ILCursor c = new ILCursor(il);

				// Try to 查找 where 566 is placed on到 堆叠
				c.GotoNext(i => i.MatchLdcI4(566));

				// 移动 the cursor after 566 and on到 ret op.
				c.Index++;
				// Push the 玩家 实例 on到 堆叠
				c.Emit(OpCodes.Ldarg_0);
				// 调用 a delegate using the int and 玩家 从 堆叠.
				c.EmitDelegate<Func<int, Player, int>>((returnValue, player) => {
					// Regular c# code
					if (player.GetModPlayer<WaspNestPlayer>().strongBeesUpgrade && Main.rand.NextBool(10) && Main.ProjectileUpdateLoopIndex == -1) {
						return ProjectileID.Beenade;
					}

					return returnValue;
				});
			}
			catch (Exception e) {
				// 如果 there are any failures 与 IL editing, this 方法 will dump the IL to Logs/ILDumps/{Mod 名称}/{方法 名称}.txt
				MonoModHooks.DumpIL(ModContent.GetInstance<ExampleMod>(), il);

				// 如果 the mod cannot run without the IL hook, throw an exception instead. The exception will call DumpIL internally
				// throw new ILPatchFailureException(ModContent.GetInstance<ExampleMod>(), il, e);
			}
		}

		public override void SetDefaults() {
			int realBackSlot = Item.backSlot;
			Item.CloneDefaults(ItemID.HiveBackpack);
			Item.value = Item.sellPrice(0, 5);
			// CloneDefaults will 清除 out the autoloaded Back 槽位, so we need to preserve it this way.
			Item.backSlot = realBackSlot;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			// original Hive Pack sets strongBees.
			player.strongBees = true;
			// 在这里 we add an additional 效果
			player.GetModPlayer<WaspNestPlayer>().strongBeesUpgrade = true;
		}

		public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player) {
			// 不要 允许 Hive Pack and Wasp Nest to be equipped 在 same 时间.
			return incomingItem.type != ItemID.HiveBackpack;
		}
	}

	public class WaspNestPlayer : ModPlayer
	{
		public bool strongBeesUpgrade;

		public override void ResetEffects() {
			strongBeesUpgrade = false;
		}
	}
}