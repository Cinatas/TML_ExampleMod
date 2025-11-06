using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod
{
	// This file shows the very basics of using ModPlayer classes since ExamplePlayer 可以 a bit overwhelming.
	// ModPlayer classes provide a way to attach data to Players and act on that data. 
	// This example will hopefully provide you with an understanding 的 basic building blocks of how ModPlayer works. 
	// This example will teach the most commonly sought after effect: "How to do X if the player has Y?"
	// X in this example 将 "Apply a debuff to enemies."
	// Y in this example 将 "Wearing an accessory."
	// 之后 studying this example, you can change X to other effects by changing the "hook" you use or the code with在 hook you use. 例如, you could use OnHitByNPC and call Projectile.NewProjectile within that hook to change X to "When the player is hit by NPC, spawn Projectiles".
	// We can change Y to other conditions 以及. 例如, you could give the player the effect by having a "potion" ModItem give a ModBuff that sets the ModPlayer variable in ModBuff.Update
	// Another example 将 an armor set effect. Simply use the ModItem.UpdateArmorSet hook 

	// Below you will see the ModPlayer class, and below that 将 another class called SimpleAccessory 对于 accessory both 在 same file for your reading convenience. This accessory will give our effect to our ModPlayer. 

	// This is the ModPlayer class. Make note 的 classname, 即 SimpleModPlayer, since we 将 using this 在 accessory item below.
	public class SimpleModPlayer : ModPlayer
	{
		// Here we declare the frostBurnSummon variable which will represent whether this player has the effect or not.
		public bool FrostBurnSummon;

		// 重置Effects is used to reset effects back 到ir default value. Terraria resets all effects every frame back to defaults so we will follow this design. (You might think to set a variable when an item is equipped and unassign the value when the item in unequipped, but Terraria is not designed that way.)
		public override void ResetEffects() {
			FrostBurnSummon = false;
		}

		// Here we use a "hook" to actually let our frostBurnSummon status take effect. This hook is called anytime a player owned projectile hits an enemy. 
		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit) {
			// frostBurnSummon, as its name suggests, applies frostBurn to enemy NPC but only for Summon projectiles.
			// In this if statement we check several conditions. We first check to make sure the projectile that hit the NPC is either a minion projectile or a projectile that minions shoot.
			// We then check that frostBurnSummon is set to true. The last check for not noEnchantments is because some projectiles don't allow enchantments and we want to honor that restriction.
			if ((proj.minion || ProjectileID.Sets.MinionShot[proj.type]) && FrostBurnSummon && !proj.noEnchantments) {
				// If all those checks pass, we apply FrostBurn for some random duration.
				target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(5, 15), false);
			}
		}

		// As a recap. Make a class variable, reset that variable in ResetEffects, and use that variable 在 logic of whatever hooks you use.
	}

	// Below is SimpleAccessory, the ModItem that gives the player the frostBurnSummon effect when worn as an accessory.

	// Note that since this namespace is nested with在 outer namespace of "ExampleMod", the full namespace is ExampleMod.Items.Armor. This is important because textures are loaded 从 namespace and classname. Even though this class is in a .cs file 在 root folder 的 mod, the namespace decides where to find item and animation textures.
	namespace Items.Armor
	{
		// Assigning multiple EquipType/Animation textures is easily done.
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
				// To assign the player the frostBurnSummon effect, we can't do player.frostBurnSummon = true because Player doesn't have frostBurnSummon. Be sure to remember to call the GetModPlayer method to retrieve the ModPlayer instance attached 到 specified Player.
				player.GetModPlayer<SimpleModPlayer>().FrostBurnSummon = true;
			}
		}
	}
}
