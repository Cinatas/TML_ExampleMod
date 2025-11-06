using ExampleMod.Backgrounds;
using ExampleMod.Common.Systems;
using ExampleMod.Content.Items.Placeable;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace ExampleMod.Content.Biomes
{
	// 展示 设置 up two basic biomes. For a more complicated example, please 请求.
	public class ExampleSurfaceBiome : ModBiome
	{
		// Select all the scenery
		public override ModWaterStyle WaterStyle => ModContent.GetInstance<ExampleWaterStyle>(); // 设置s a water style for when inside this 生物群系
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<ExampleSurfaceBackgroundStyle>();
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;

		// Select 音乐
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery");

		public override int BiomeTorchItemType => ModContent.ItemType<ExampleTorch>();
		public override int BiomeCampfireItemType => ModContent.ItemType<ExampleCampfire>();

		// Populate the Bestiary 过滤
		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;
		public override string MapBackground => BackgroundPath; // Re-uses Bestiary 背景 for 地图 背景

		// 计算 when the 生物群系 is active.
		public override bool IsBiomeActive(Player player) {
			// 首先, we will use the exampleBlockCount from our added ModSystem for our first custom 条件
			bool b1 = ModContent.GetInstance<ExampleBiomeTileCount>().exampleBlockCount >= 40;

			// 其次, we will 限制 this 生物群系 到 inner horizontal third 的 地图 as our second custom 条件
			bool b2 = Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2) < Main.maxTilesX / 6;

			// 最后, we will 限制 the 高度 at which this 生物群系 可以 active to above ground (ie sky and surface). Most (如果不是 all) surface biomes will use this 条件.
			bool b3 = player.ZoneSkyHeight || player.ZoneOverworldHeight;
			return b1 && b2 && b3;
		}

		// Declare 生物群系 priority. The default is BiomeLow so this is only necessary if it needs a higher priority.
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;
	}
}
