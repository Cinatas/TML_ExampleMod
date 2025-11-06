using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalNPCs
{
	// 展示各种减益免疫更改。显示了单个减益免疫更改和继承的免疫更改。

	// 为了模组兼容性，将调整减益的代码放在正确的方法中很重要：
	// 在 ModNPC/GlobalNPC.SetStaticDefaults 中调整 BuffID.Sets.GrantImmunityWith 和 NPCID.Sets.SpecificDebuffImmunity。
	// 如果需要更改其他模组内容的免疫并且 GrantImmunityWith 不足，则可以在 PostSetupContent 中调整 SpecificDebuffImmunity。
	// 仅在 ModNPC/GlobalNPC.SetDefaults 中直接调整 NPC.buffImmune[] 以实现复杂或世界状态特定逻辑。
	// 可以在 AI 或 SetDefaults 方法期间调整 NPC.buffImmune[]，但请注意减益继承不适用于手动更改。
	// 例如：当 Boss 旋转时，设置 NPC.buffImmune[OnFire] = true。
	// 使用 NPC.BecomeImmuneTo 和 NPC.ClearImmuneToBuffs 方法而不是直接调整 NPC.buffImmune[] 将允许减益继承。MinionBossBody 展示了这种方法。
	public class BuffImmuneGlobalNPC : GlobalNPC
	{
		public override void SetStaticDefaults() {
			// 不要应用于 NPCID.Sets.SpecificDebuffImmunity，这不是重点。

			// 此示例执行 2 件事。
			// 首先，它将现有减益与模组减益链接在一起。这些链接确保任何对现有减益免疫的 NPC 将自动对模组减益免疫。有关此概念的另一个示例，请参阅 ExampleJavelinDebuff.cs。
			// 其次，它改变了一些现有的 NPC 免疫。

			// 此代码使任何对灵液免疫的 NPC 自动对双足飞龙诅咒免疫。
			// 在 ExampleDefenseDebuff 中调用 SetDefenseDebuffStaticDefaults 以对该增益执行相同的操作。
			// 原本幽灵、沙漠精灵和远古视界都对双足飞龙诅咒和灵液免疫，
			// 而灵液黏附怪和圣骑士仅对灵液免疫。当此模组处于活动状态时，这些将改变。
			SetDefenseDebuffStaticDefaults(BuffID.BetsysCurse);

			// 模组作者必须使用 GrantImmunityWith 来"继承"免疫，而不是迭代 NPCID.Sets.SpecificDebuffImmunity。GrantImmunityWith 有助于保留所有加载的模组的意图。未能使用 GrantImmunityWith 将导致 NPC 具有不合逻辑的免疫，这是由于模组代码手动逐个更改免疫造成的。
			// NPCID.Sets.SpecificDebuffImmunity 应仅修改以指示显式免疫，而不是继承的免疫。

			// NPC 增益免疫更改：

			// IchorSticker 已经对灵液免疫，但对其他没有免疫。由于 GrantImmunityWith，它将自动对双足飞龙诅咒和 ExampleDefenseDebuff 免疫。

			// DesertGhoulCrimson 施加灵液，因此此模组将其更改为对灵液免疫，这反过来也将使其对双足飞龙诅咒和 ExampleDefenseDebuff 免疫。
			NPCID.Sets.SpecificDebuffImmunity[NPCID.DesertGhoulCrimson][BuffID.Ichor] = true;

			// 圣骑士特别对灵液免疫，但在此处将其设置为 false 将使其不免疫。这是此模组的假设设计决策。任何其他模组正确使用 GrantImmunityWith 为其自己的灵液变体继承灵液增益免疫不会导致圣骑士对该增益免疫，如预期的那样。
			NPCID.Sets.SpecificDebuffImmunity[NPCID.Paladin][BuffID.Ichor] = false;
		}

		public static void SetDefenseDebuffStaticDefaults(int buffType) {
			// 此辅助方法使任何对灵液免疫的 NPC 自动对提供的增益免疫。
			BuffID.Sets.GrantImmunityWith[buffType].Add(BuffID.Ichor);
		}

		public override void SetDefaults(NPC entity) {
			// 如果需要，任何最终的自定义全局增益免疫逻辑都可以放在 GlobalNPC.SetDefaults 中。（这在 ModNPC.SetDefaults 之后运行） 
			// 尽量坚持提供的功能以获得最大兼容性，而不是手动执行操作。
			// 例如，如果 NPC 对任何指定的增益免疫，则以下增益继承将适用：
			// BuffID.Sets.GrantImmunityWith[ModContent.BuffType<PoisonFire>()].AddRange(new int[] { BuffID.OnFire, BuffID.Poisoned });
			// 如果意图是仅当对着火和中毒都免疫时才对 PoisonFire 免疫，则可以在此处完成该效果。
		}
	}
}
