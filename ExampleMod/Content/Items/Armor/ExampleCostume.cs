using ExampleMod.Common.Players;
using ExampleMod.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Armor
{
	// This and 几个 other classes show off using EquipTextures to do a Merfolk or Werewolf 效果.
	// Typically 护甲 items are automatically paired with an EquipTexture, but 我们可以 manually use EquipTextures to achieve more unique effects.
	// 有 code for this 效果 in m任何 places, look 在 following files 对于 full implementation:
	// NPCs.ExamplePerson drops this 项 when killed
	// Content.Items.护甲.ExampleCostume (below) is the 饰品 项 that sets ExampleCostumePlayer values. 注意 this 项 does not have EquipTypes set. 这是一个 vital difference and 键 to our approach.
	// Content.Items.护甲.BlockyHead (below) is an EquipTexture 类. It spawns dust when active.
	// 示例Costume.加载() shows calling AddEquipTexture 3 times with appropriate parameters. This is how we register EquipTexture manually instead 的 automatic pairing of ModItem and EquipTexture that other equipment uses.
	// Buffs.Blocky is the 增益 即 shown while in Blocky 模式. The 增益 is responsible 对于 actual stat effects 的 costume. It also needs to 删除 itself when not near town npcs.
	// 示例CostumePlayer has 6 bools. They 管理 the visibility and other things 与...相关 this 效果.
	// 示例CostumePlayer.ResetEffects resets those bool, except blockyAccessoryPrevious 即 special because 的 顺序 of hooks.
	// 示例CostumePlayer.UpdateEquips is responsible for applying the Blocky 增益 到 玩家 if the conditions are met and the 饰品 is equipped.
	// 示例CostumePlayer.FrameEffects is most important. It overrides the drawn equipment slots and sets them to our Blocky EquipTextures.
	// 示例CostumePlayer.ModifyDrawInfo is for some fun effects for our costume.
	// 记住 th在 visuals and the effects of Costumes 必须 kept 分离. Follow this example for best results.
	public class ExampleCostume : ModItem
	{
		public override void Load() {
			// code below runs only if we're not loading on a 服务器
			if (Main.netMode == NetmodeID.Server)
				return;

			// 添加 equip textures
			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this, equipTexture: new BlockyHead());
			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);

			//Add a 分离 set of equip textures by providing a custom 名称 引用 代替 an 项 引用
			EquipLoader.AddEquipTexture(Mod, $"{Texture}Alt_{EquipType.Head}", EquipType.Head, name: "BlockyAlt", equipTexture: new BlockyHead());
			EquipLoader.AddEquipTexture(Mod, $"{Texture}Alt_{EquipType.Body}", EquipType.Body, name: "BlockyAlt");
			EquipLoader.AddEquipTexture(Mod, $"{Texture}Alt_{EquipType.Legs}", EquipType.Legs, name: "BlockyAlt");
		}

		// 调用ed in SetStaticDefaults
		private void SetupDrawing() {
			// Since the equipment textures weren't loaded 在 服务器, 我们可以't have this code running 服务器-side
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
				// 2 分离 instances 的 BlockyHead 类 are used, 我们可以 differentiate them with 名称 如果需要.
				if (Name == "ExampleCostume") {
					Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<Sparkle>());
				}
				else {
					// 名称 == "BlockyAlt"
					Dust.NewDust(player.position, player.width, player.height, DustID.BlueFlare);
				}
			}
		}
	}
}