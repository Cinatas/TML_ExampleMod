using ExampleMod.Common.Configs;
using ExampleMod.Content.NPCs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

//与 GlobalProjectile 相关：ProjectileWithGrowingDamage
namespace ExampleMod.Common.GlobalItems
{
	public class WeaponWithGrowingDamage : GlobalItem
	{
		public int experience;
		public static int experiencePerLevel = 100;
		private int bonusValuePerItem;
		public int level => experience / experiencePerLevel;

		public override bool InstancePerEntity => true;

		public override bool IsLoadingEnabled(Mod mod) {
			// 要试验此示例，你需要在配置中启用它。
			return ModContent.GetInstance<ExampleModConfig>().WeaponWithGrowingDamageToggle;
		}

		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
			//应用于武器
			return lateInstantiation && entity.damage > 0;
		}
		public override void LoadData(Item item, TagCompound tag) {
			experience = 0;
			GainExperience(item, tag.Get<int>("experience"));//加载经验标签
		}

		public override void SaveData(Item item, TagCompound tag) {
			tag["experience"] = experience;//保存经验标签
		}

		public override void NetSend(Item item, BinaryWriter writer) {
			writer.Write(experience);
		}

		public override void NetReceive(Item item, BinaryReader reader) {
			experience = 0;
			GainExperience(item, reader.ReadInt32());
		}

		public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone) {
			OnHitNPCGeneral(player, target, hit, item);
		}

		public void OnHitNPCGeneral(Player player, NPC target, NPC.HitInfo hit, Item item = null, Projectile projectile = null) {
			//武器在击中 NPC 时获得经验。
			int xp = hit.Damage;
			if (projectile != null) {
				xp /= 2;
			}

			GainExperience(item, xp);
		}

		public void GainExperience(Item item, int xp) {
			experience += xp;

			UpdateValue(item);
		}

		public void UpdateValue(Item item, int stackChange = 0) {
			if (item == null) {
				return;
			}

			item.value -= bonusValuePerItem;
			int stack = item.stack + stackChange;
			if (stack == 0) {
				bonusValuePerItem = 0;
			}
			else {
				bonusValuePerItem = experience * 5 / stack;
			}

			item.value += bonusValuePerItem;
		}

		public override void UpdateInventory(Item item, Player player) {
			UpdateValue(item);
		}

		public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage) {
			//武器每升一级获得 1% 的乘法伤害。
			damage *= 1f + (float)level / 100f;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
			if (experience > 0) {
				tooltips.Add(new TooltipLine(Mod, "level", $"Level: {level}") { OverrideColor = Color.LightGreen });
				string levelString = $" ({(level + 1) * experiencePerLevel - experience} to next level)";
				tooltips.Add(new TooltipLine(Mod, "experience", $"Experience: {experience}{levelString}") { OverrideColor = Color.White });
			}
		}

		public override void OnCreated(Item item, ItemCreationContext context) {
			if (item.type == ItemID.Snowball) {
				GainExperience(item, item.stack); // 雪球带有 1 经验值，用于测试 :)
			}

			if (context is RecipeItemCreationContext rContext) {
				foreach (Item ingredient in rContext.ConsumedItems) {
					if (ingredient.TryGetGlobalItem(out WeaponWithGrowingDamage ingredientGlobal)) {
						//将消耗物品的所有经验转移到制作的物品。
						GainExperience(item, ingredientGlobal.experience);
					}
				}
			}
		}

		public override void OnStack(Item destination, Item source, int numToTransfer) {
			if (!source.TryGetGlobalItem(out WeaponWithGrowingDamage weapon2)) {
				return;
			}

			TransferExperience(destination, source, weapon2, numToTransfer);
		}

		public override void SplitStack(Item destination, Item source, int numToTransfer) {
			if (!source.TryGetGlobalItem(out WeaponWithGrowingDamage weapon2)) {
				return;
			}

			//防止在新物品上复制经验，increase 是 decrease 的克隆。经验不应该被克隆，所以将其设置为 0。
			experience = 0;

			TransferExperience(destination, source, weapon2, numToTransfer);
		}

		private void TransferExperience(Item destination, Item source, WeaponWithGrowingDamage weapon2, int numToTransfer) {
			//将经验和价值转移到 increase。
			experience += weapon2.experience;
			UpdateValue(destination, numToTransfer);

			if (source.stack > numToTransfer) {
				//如果 decrease 仍然存在，则通过清除其经验来防止复制经验。
				weapon2.experience = 0;
				weapon2.UpdateValue(source, -numToTransfer);
			}
		}
	}

	public class DoubleXPSnowBallInExamplePersonShop : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) {
			// 要试验此示例，你需要在配置中启用它。
			return ModContent.GetInstance<ExampleModConfig>().WeaponWithGrowingDamageToggle;
		}

		public override void ModifyShop(NPCShop shop) {
			if (shop.NpcType != ModContent.NPCType<ExamplePerson>()) {
				return;
			}

			var snowball = new Item(ItemID.Snowball);
			if (snowball.TryGetGlobalItem(out WeaponWithGrowingDamage weapon)) {
				weapon.GainExperience(snowball, 2);
			}
			shop.Add(snowball);
		}
	}
}
