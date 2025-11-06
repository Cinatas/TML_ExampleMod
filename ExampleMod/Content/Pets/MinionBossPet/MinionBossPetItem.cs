using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Pets.MinionBossPet
{
	// 你 can 查找 a simple 宠物 example 在 ExampleMod\Content\Pets\ExamplePet\ 文件夹
	public class MinionBossPetItem : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToVanitypet(ModContent.ProjectileType<MinionBossPetProjectile>(), ModContent.BuffType<MinionBossPetBuff>()); // Vanilla has many useful methods like these, use them! It sets 稀有度 and 值 以及, so we have to overwrite those after

			Item.width = 28;
			Item.height = 20;
			Item.rare = ItemRarityID.Master;
			Item.master = true; // This makes sure that "Master" displays 在 工具提示, as the 稀有度 only changes the 项 名称 颜色
			Item.value = Item.sellPrice(0, 5);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			player.AddBuff(Item.buffType, 2); // The 项 applies the 增益, the 增益 spawns the 弹幕

			return false;
		}
	}
}
