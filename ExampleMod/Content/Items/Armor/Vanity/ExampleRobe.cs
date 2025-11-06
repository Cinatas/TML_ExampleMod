using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Armor.Vanity
{
	// 参见 also: ExampleCostume
	[AutoloadEquip(EquipType.Body)]
	public class ExampleRobe : ModItem
	{
		public override void Load() {
			// code below runs only if we're not loading on a 服务器
			if (Main.netMode == NetmodeID.Server) {
				return;
			}

			// By passing this (the ModItem) in到 项 参数 we can 引用 it later in GetEquipSlot with just the 项's 名称
			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
		}

		public override void SetStaticDefaults() {
			// 隐藏sHands defaults to 真 which we don't want.
			ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = false;
		}

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 14;
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}

		public override void SetMatch(bool male, ref int equipSlot, ref bool robes) {
			// By changing the equipSlot 到 leg equip 纹理 槽位, the leg 纹理 will now be drawn 在 玩家
			// We're changing the leg 槽位 so we set this to 真
			robes = true;
			// 在这里 we can get the equip 槽位 by 名称 since we referenced the 项 when adding the 纹理
			// 你 can also 缓存 the equip 槽位 in a 变量 when you add it so this way you don't have to call GetEquipSlot
			equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
		}
	}
}
