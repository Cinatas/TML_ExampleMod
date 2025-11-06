using ExampleMod.Common.Players;
using ExampleMod.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Armor
{
	// This and several other classes show off using EquipTextures to do a Merfolk or Werewolf effect.
	// Typically Armor items are automatically paired with an EquipTexture, but we can manually use EquipTextures to achieve more unique effects.
	// There is code for this effect in many places, look 在 following files 对于 full implementation:
	// NPCs.ExamplePerson drops this item when killed
	// Content.Items.Armor.ExampleCostume (below) is the accessory item that sets ExampleCostumePlayer values. Note that this item does not have EquipTypes set. This is a vital difference and key to our approach.
	// Content.Items.Armor.BlockyHead (below) is an EquipTexture class. It spawns dust when active.
	// 示例Costume.Load() shows calling AddEquipTexture 3 times with appropriate parameters. This is how we register EquipTexture manually instead 的 automatic pairing of ModItem and EquipTexture that other equipment uses.
	// Buffs.Blocky is the Buff 即 shown while in Blocky mode. The buff is responsible 对于 actual stat effects 的 costume. It also needs to remove itself when not near town npcs.
	// 示例CostumePlayer has 6 bools. They manage the visibility and other things related to this effect.
	// 示例CostumePlayer.ResetEffects resets those bool, except blockyAccessoryPrevious 即 special because 的 order of hooks.
	// 示例CostumePlayer.UpdateEquips is responsible for applying the Blocky buff 到 player if the conditions are met and the accessory is equipped.
	// 示例CostumePlayer.FrameEffects is most important. It overrides the drawn equipment slots and sets them to our Blocky EquipTextures.
	// 示例CostumePlayer.ModifyDrawInfo is for some fun effects for our costume.
	// 记住 th在 visuals and the effects of Costumes 必须 kept separate. Follow this example for best results.
	public class ExampleCostume : ModItem
	{
		public override void Load() {
			// code below runs only if we're not loading on a server
			if (Main.netMode == NetmodeID.Server)
				return;

			// 添加 equip textures
			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this, equipTexture: new BlockyHead());
			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);

			//Add a separate set of equip textures by providing a custom name reference instead of an item reference
			EquipLoader.AddEquipTexture(Mod, $"{Texture}Alt_{EquipType.Head}", EquipType.Head, name: "BlockyAlt", equipTexture: new BlockyHead());
			EquipLoader.AddEquipTexture(Mod, $"{Texture}Alt_{EquipType.Body}", EquipType.Body, name: "BlockyAlt");
			EquipLoader.AddEquipTexture(Mod, $"{Texture}Alt_{EquipType.Legs}", EquipType.Legs, name: "BlockyAlt");
		}

		// 调用ed in SetStaticDefaults
		private void SetupDrawing() {
			// Since the equipment textures weren't loaded 在 server, we can't have this code running server-side
			if (Main.netMode == NetmodeID.Server)
				return;

			int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
			int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
			int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

			int equipSlotHeadAlt = EquipLoader.GetEquipSlot(Mod, "BlockyAlt", EquipType.Head);
			int equipSlotBodyAlt = EquipLoader.GetEquipSlot(Mod, "BlockyAlt", EquipType.Body);
			int equipSlotLegsAlt = EquipLoader.GetEquipSlot(Mod, "BlockyAlt", EquipType.Legs);

			ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
			ArmorIDs.Head.Sets.DrawHead[equipSlotHeadAlt] = false;
			ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
			ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
			ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBodyAlt] = true;
			ArmorIDs.Body.Sets.HidesArms[equipSlotBodyAlt] = true;
			ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
			ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegsAlt] = true;
		}

		public override void SetStaticDefaults() {
			SetupDrawing();
		}

		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 28;
			Item.accessory = true;
			Item.value = Item.buyPrice(gold: 15);
			Item.rare = ItemRarityID.Pink;
			Item.hasVanityEffects = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			var p = player.GetModPlayer<ExampleCostumePlayer>();
			p.BlockyAccessory = true;
			p.BlockyHideVanity = hideVisual;
		}

		public override void UpdateVanity(Player player) {
			var p = player.GetModPlayer<ExampleCostumePlayer>();
			p.BlockyHideVanity = false;
			p.BlockyForceVanity = true;
		}
	}

	public class BlockyHead : EquipTexture
	{
		public override bool IsVanitySet(int head, int body, int legs) => true;

		public override void UpdateVanitySet(Player player) {
			if (Main.rand.NextBool(20)) {
				// 2 separate instances 的 BlockyHead class are used, we can differentiate them with Name 如果需要.
				if (Name == "ExampleCostume") {
					Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<Sparkle>());
				}
				else {
					// Name == "BlockyAlt"
					Dust.NewDust(player.position, player.width, player.height, DustID.BlueFlare);
				}
			}
		}
	}
}