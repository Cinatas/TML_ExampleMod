using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	// 示例Bobber is a fishing bobber spawned by ExampleFishingRod.
	// Aside 从 code in SetDefaults, 每个thing else 应该 ignored when making a typical bobber 弹幕.
	// Typically the fishing rod 项 decides the line 颜色, but this bobber decides its own line 颜色 and serves as an example of using OnSpawn, SendExtraAI, and ReceiveExtraAI to 同步 a 随机 值 determined when spawned.
	public class ExampleBobber : ModProjectile
	{
		public static readonly Color[] PossibleLineColors = new Color[] {
			new Color(255, 215, 0), // A 金币 颜色
			new Color(0, 191, 255) // A blue 颜色
		};

		// This holds the 索引 的 fishing line 颜色 在 PossibleLineColors 数组.
		private int fishingLineColorIndex;

		public Color FishingLineColor => PossibleLineColors[fishingLineColorIndex];

		public override void SetDefaults() {
			// These are copied through the CloneDefaults 方法
			// 弹幕.宽度 = 14;
			// 弹幕.高度 = 14;
			// 弹幕.aiStyle = 61;
			// 弹幕.bobber = 真;
			// 弹幕.penetrate = -1;
			// 弹幕.netImportant = 真;
			Projectile.CloneDefaults(ProjectileID.BobberWooden);

			DrawOriginOffsetY = -8; // Adjusts the draw 位置
		}

		public override void OnSpawn(IEntitySource source) {
			// Decide 颜色 的 pole by getting the 索引 of a 随机 entry 从 PossibleLineColors 数组.
			fishingLineColorIndex = (byte)Main.rand.Next(PossibleLineColors.Length);
		}

		public override void AI() {
			// 始终 ensure that graphics-related code doesn't run on dedicated servers via this check.
			if (!Main.dedServ) {
				// 创建 some light based 在 颜色 的 line.
				Lighting.AddLight(Projectile.Center, FishingLineColor.ToVector3());
			}
		}

		// These last two methods are required so the line 颜色 is properly synced in multiplayer.
		public override void SendExtraAI(BinaryWriter writer) {
			writer.Write((byte)fishingLineColorIndex);
		}

		public override void ReceiveExtraAI(BinaryReader reader) {
			fishingLineColorIndex = reader.ReadByte();
		}
	}
}