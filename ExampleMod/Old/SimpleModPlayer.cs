using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod
{
	// This 文件 shows the very basics of using ModPlayer classes since ExamplePlayer 可以 a bit overwhelming.
	// ModPlayer classes provide a way to attach 数据 to Players and act on that 数据. 
	// 此示例 will hopefully provide you with an understanding 的 basic building blocks of how ModPlayer works. 
	// 此示例 will teach the most commonly sought after 效果: "How to do X if the 玩家 has Y?"
	// X in this example 将 "Apply a 减益 to enemies."
	// Y in this example 将 "Wearing an 饰品."
	// 之后 studying this example, you can change X to other effects by changing the "hook" you use or the code with在 hook you use. 例如, you could use OnHitByNPC and call 弹幕.NewProjectile within that hook to change X to "When the 玩家 is hit by NPC, 生成 Projectiles".
	// 我们可以 change Y to other conditions 以及. 例如, you could give the 玩家 the 效果 by having a "药水" ModItem give a ModBuff that sets the ModPlayer 变量 in ModBuff.更新
	// Another example 将 an 护甲 set 效果. Simply use the ModItem.UpdateArmorSet hook 

	// Below you will see the ModPlayer 类, and below that 将 another 类 called SimpleAccessory 对于 饰品 两者 在 same 文件 for your reading convenience. This 饰品 will give our 效果 to our ModPlayer. 

	// 这是 ModPlayer 类. Make note 的 classname, 即 SimpleModPlayer, since we 将 using this 在 饰品 项 below.
	public class SimpleModPlayer : ModPlayer
	{
		// Here we declare the frostBurnSummon 变量 which will represent whether this 玩家 has the 效果 or not.
		public bool FrostBurnSummon;

		// 重置Effects is 用于 重置 effects back 到ir default 值. Terraria resets all effects 每个 帧 back to defaults so we will follow this design. (You might think to set a 变量 when an 项 is equipped and unassign the 值 when the 项 in unequipped, but Terraria is not designed that way.)
		public override void ResetEffects() {
			FrostBurnSummon = false;
		}

		// Here we use a "hook" to actually let our frostBurnSummon status take 效果. This hook is called 任何time a 玩家 owned 弹幕 hits an 敌人. 
		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit) {
			// frostBurnSummon, as its 名称 suggests, applies frostBurn to 敌人 NPC but only for Summon projectiles.
			// In this if statement we check 几个 conditions. We first check to make sure the 弹幕 that hit the NPC is 任一 a 仆从 弹幕 or a 弹幕 that minions shoot.
			// We then check that frostBurnSummon is set to 真. The last check for not noEnchantments is because some projectiles don't 允许 enchantments and we 想要 honor that restriction.
			if ((proj.minion || ProjectileID.Sets.MinionShot[proj.type]) && FrostBurnSummon && !proj.noEnchantments) {
				// If all those checks pass, we apply FrostBurn for some 随机 持续时间.
				target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(5, 15), false);
			}
		}

		// As a recap. Make a 类 变量, 重置 that 变量 in ResetEffects, and use that 变量 在 logic of whatever hooks you use.
	}

	// Below is SimpleAccessory, the ModItem that gives the 玩家 the frostBurnSummon 效果 when worn as an 饰品.

	// Note that since this namespace is nested with在 outer namespace of "ExampleMod", the full namespace is ExampleMod.Items.护甲. This is important because textures are loaded 从 namespace and classname. Even though this 类 is in a .cs 文件 在 root 文件夹 的 mod, the namespace decides where to 查找 项 and 动画 textures.
	namespace Items.Armor
	{
		// Assigning 多个 EquipType/动画 textures is easily done.
		[AutoloadEquip(EquipType.Neck, EquipType.Balloon)]
		internal class SimpleAccessory : ModItem
		{
			public override void SetDefaults() {
				item.width = 34;
				item.height = 34;
				item.accessory = true;
				item.value = 150000;
				item.rare = ItemRarityID.Pink;
			}

			public override void UpdateAccessory(Player player, bool hideVisual) {
				// To assign the 玩家 the frostBurnSummon 效果, we can't do 玩家.frostBurnSummon = 真 because 玩家 doesn't have frostBurnSummon. Be sure to remember to call the GetModPlayer 方法 to retrieve the ModPlayer 实例 attached 到 specified 玩家.
				player.GetModPlayer<SimpleModPlayer>().FrostBurnSummon = true;
			}
		}
	}
}
