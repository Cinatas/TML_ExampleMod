using ExampleMod.Content.Items.Mounts;
using ExampleMod.Content.Mounts;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	//TODO 1.4.5: review MinecartLeft/Right if it still exists
	public class ExampleMinecartBuff : ModBuff
	{
		// 使用 the vanilla DisplayName ("Minecart")
		//public override LocalizedText DisplayName => Language.GetText("BuffName.MinecartLeft");
		// But 对于 sake of example, we want to reuse the item name
		public override LocalizedText DisplayName => ModContent.GetInstance<ExampleMinecart>().DisplayName;

		// 使用 the vanilla Description
		public override LocalizedText Description => Language.GetText("BuffDescription.MinecartLeft");

		public override void SetStaticDefaults() {
			// 处理s automatically mounting the player within Update, and setting Main.buffNoTimeDisplay/buffNoSave (no need to write yourself like in ExampleMountBuff)
			BuffID.Sets.BasicMountData[Type] = new BuffID.Sets.BuffMountData() {
				mountID = ModContent.MountType<ExampleMinecartMount>()
			};
		}
	}
}
