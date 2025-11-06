using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	public class ExampleQuestFish : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 2;
			ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true; // All vanilla fish 可以 placed in a 武器 rack.
		}

		public override void SetDefaults() {
			// 默认ToQuestFish sets 任务 fish properties.
			// Of note, it sets rare to ItemRarityID.任务, 即 the special 稀有度 for 任务 items.
			// It also sets uniqueStack to 真, which prevents players from picking up a 2nd 复制 的 项 in到ir 库存.
			Item.DefaultToQuestFish();
		}

		public override bool IsQuestFish() => true; // 使 the 项 a 任务 fish

		public override bool IsAnglerQuestAvailable() => Main.hardMode; // 使 the 任务 only appear in hard 模式. Adding a '!' before Main.hardMode makes it ONLY available in pre-hardmode.

		public override void AnglerQuestChat(ref string description, ref string catchLocation) {
			// How the angler describes the fish 到 玩家.
			description = "I've heard stories of a fish that swims upside-down. Supposedly you have the stand upside-down yourself to even find one. One of those would go great on my ceiling. Go fetch!";
			// What it says 在 底部 的 angler's 文本 box of how to catch the fish.
			catchLocation = "Caught anywhere while standing upside-down.";
		}
	}
}
