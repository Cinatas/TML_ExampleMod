using ExampleMod.Content.Items.Placeable;
using ExampleMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	// 这是 a 复制 的 Excalibur
	public class ExampleSwingingEnergySword : ModItem
	{
		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 20;
			Item.useTime = 20;
			Item.damage = 72;
			Item.knockBack = 4.5f;
			Item.width = 40;
			Item.height = 40;
			Item.scale = 1f;
			Item.UseSound = SoundID.Item1;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.buyPrice(gold: 23); // 出售 价格 is 5 times less than the 购买 价格.
			Item.DamageType = DamageClass.Melee;
			Item.shoot = ModContent.ProjectileType<ExampleSwingingEnergySwordProjectile>();
			Item.noMelee = true; // This is set the sword itself doesn't deal 伤害 (only the 弹幕 does).
			Item.shootsEveryUse = true; // This makes sure 玩家.ItemAnimationJustStarted is set when swinging.
			Item.autoReuse = true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			float adjustedItemScale = player.GetAdjustedItemScale(Item); // 获取 the melee 缩放 的 玩家 and 项.
			Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
			NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI); // 同步 the changes in multiplayer.

			return base.Shoot(player, source, position, velocity, type, damage, knockback);
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of 配方 creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleBar>(12)
				.AddTile(TileID.MythrilAnvil) // This includes both the Mythril and Orichalcum Anvils.
				.Register();
		}
	}
}
