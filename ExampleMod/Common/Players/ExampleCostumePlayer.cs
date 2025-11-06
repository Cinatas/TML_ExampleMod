using ExampleMod.Content.Biomes;
using ExampleMod.Content.Buffs;
using ExampleMod.Content.Items.Armor;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	public class ExampleCostumePlayer : ModPlayer
	{
		// 这 6 个与 ExampleCostume 相关。
		public bool BlockyAccessoryPrevious;
		public bool BlockyAccessory;             // If 真, an 饰品 granting potential effects is equipped
		public bool BlockyHideVanity;            // If 真, the 项 is in a hidden 饰品 槽位
		public bool BlockyForceVanity;           //	If 真, the vanity is forced because the 项 is in a vanity 槽位, not the stats.
		public bool BlockyPower;                 // If 真, the stats boosts are applied
		public bool BlockyVanityEffects => BlockyForceVanity || (BlockyPower && !BlockyHideVanity); // This helper 属性 controls if the 音频 and visual effects 的 vanity 应该 applied.

		public override void ResetEffects() {
			BlockyAccessoryPrevious = BlockyAccessory;
			BlockyAccessory = BlockyHideVanity = BlockyForceVanity = BlockyPower = false;
		}

		public override void UpdateEquips() {
			// 确保此条件与增益中删除自身的条件相同。我们在这里执行此操作而不是在 ModItem.UpdateAccessory 中，以防我们希望将来升级的物品设置 blockyAccessory
			if (Player.townNPCs >= 1 && BlockyAccessory) {
				Player.AddBuff(ModContent.BuffType<Blocky>(), 60);
			}
		}

		public override void FrameEffects() {
			// TODO：需要新钩子，FrameEffects 在暂停时不运行。
			if (BlockyVanityEffects) {
				var exampleCostume = ModContent.GetInstance<ExampleCostume>();
				Player.head = EquipLoader.GetEquipSlot(Mod, exampleCostume.Name, EquipType.Head);
				Player.body = EquipLoader.GetEquipSlot(Mod, exampleCostume.Name, EquipType.Body);
				Player.legs = EquipLoader.GetEquipSlot(Mod, exampleCostume.Name, EquipType.Legs);

				// 使用 the alternative equipment textures by calling them through their internal 名称.
				if (Player.wet) {
					Player.head = EquipLoader.GetEquipSlot(Mod, "BlockyAlt", EquipType.Head);
					Player.body = EquipLoader.GetEquipSlot(Mod, "BlockyAlt", EquipType.Body);
					Player.legs = EquipLoader.GetEquipSlot(Mod, "BlockyAlt", EquipType.Legs);
				}
			}
		}

		public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo) {
			if (BlockyVanityEffects) {
				Player.headRotation = Player.velocity.Y * Player.direction * 0.1f;
				Player.headRotation = Utils.Clamp(Player.headRotation, -0.3f, 0.3f);
				if (Player.InModBiome<ExampleSurfaceBiome>()) {
					Player.headRotation = (float)Main.time * 0.1f * Player.direction;
				}
			}
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers) {
			if (BlockyVanityEffects) {
				modifiers.DisableSound();
			}
		}

		public override void OnHurt(Player.HurtInfo info) {
			if (BlockyVanityEffects) {
				// SoundID.Frog is actually SoundType.Ambient, so we 需要 change it to play 在 correct SoundType.声音 master 音量.
				SoundEngine.PlaySound(SoundID.Frog with { Type = SoundType.Sound }, Player.position);
			}
		}
	}
}