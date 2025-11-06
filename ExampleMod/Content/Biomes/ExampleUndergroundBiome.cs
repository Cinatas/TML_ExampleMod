using ExampleMod.Backgrounds;
using ExampleMod.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Biomes
{
	public class ExampleUndergroundBiome : ModBiome
	{
		// Select all the scenery
		public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<ExampleUndergroundBackgroundStyle>();

		// Select 音乐
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery");

		// 设置s how the 场景 效果 associated with this 生物群系 将 displayed with respect to vanilla 场景 Effects. F或更多 information see SceneEffectPriority & its values.
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow; // 我们有 set the SceneEffectPriority to be BiomeLow for purpose of example, however default behavior is BiomeLow.

		// Populate the Bestiary 过滤
		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

		// 计算 when the 生物群系 is active.
		public override bool IsBiomeActive(Player player) {
			// 限制 the 生物群系 高度 to be underground in 任一 rock 层 or dirt 层
			return (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) &&
				// 检查 how m任何 tiles of our 生物群系 are present, such that 生物群系 应该 active
				ModContent.GetInstance<ExampleBiomeTileCount>().exampleBlockCount >= 40 &&
				// 限制 our 生物群系 to be in only the horizontal 中心 third 的 世界.
				Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2) < Main.maxTilesX / 6;
		}

		// 在 the 事件 that 两者 our 生物群系 AND one 或更多 modded SceneEffect layers are active 与 same SceneEffect Priority, this can decide which one.
		// It's uncommon that 需要 assign a weight - you'd 必须 specifically believe that you don't need higher SceneEffectPriority, but do 需要 be the active SceneEffect with在 priority you designated
		// 在 this case, we don't need it, so this inclusion is purely to demonstrate this is available.
		// 参见 the GetWeight documentation f或更多 information.
		/*
		public override float GetWeight(Player player) {
			int distanceToCenter = Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2);
			// 我们 declare that our 生物群系 should have be more likely than not to be active if in 中心 1/6 的 世界, and decreases in 需要 be active as 玩家 gets further away 到 1/3 mark.
			if (distanceToCenter <= Main.maxTilesX / 12) {
				return 1f;
			}
			else {
				return 1f - (distanceToCenter - Main.maxTilesX / 12) / (Main.maxTilesX / 12);
			}
		}
		*/
	}
}
