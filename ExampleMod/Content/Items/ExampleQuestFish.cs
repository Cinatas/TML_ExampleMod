using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	public class ExampleQuestFish : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 2;
			ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true; // All vanilla fish 可以 placed in a weapon rack.
		}

		public override void SetDefaults() {
			// 默认ToQuestFish sets quest fish properties.
			// Of note, it sets rare to ItemRarityID.Quest, 即 the special rarity for quest items.
			// It also sets uniqueStack to true, which prevents players from picking up a 2nd copy 的 item in到ir inventory.
			Item.DefaultToQuestFish();
		}

		public override bool IsQuestFish() => true; // 使 the item a quest fish

		public override bool IsAnglerQuestAvailable() => Main.hardMode; // 使 the quest only appear in hard mode. Adding a '!' before Main.hardMode makes it ONLY available in pre-hardmode.

		public override void AnglerQuestChat(ref string description, ref string catchLocation) {
			// How the angler describes the fish 到 player.
			description = "I've heard stories of a fish that swims upside-down. Supposedly you have the stand upside-down yourself to even find one. One of those would go great on my ceiling. Go fetch!";
			// What it says 在 bottom 的 angler's text box of how to catch the fish.
			catchLocation = "Caught anywhere while standing upside-down.";
		}
	}
}
