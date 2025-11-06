using ExampleMod.Content.Items;
using ExampleMod.Content.Items.Placeable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Content.Tiles
{
	// An enum 对于 3 stages of herb growth
	public enum PlantStage : byte
	{
		Planted,
		Growing,
		Grown
	}

	// 一个 plant with 3 stages, planted, growing and grown
	// Sadly, modded plants are unable to be grown by the flower boots
	//TODO smart cursor support for herbs, see SmartCursorHelper.Step_AlchemySeeds
	//TODO Staff of Regrowth:
	//- 玩家.PlaceThing_Tiles_BlockPlacementForAssortedThings: check where 类型 == 84 (grown herb)
	//- 玩家.ItemCheck_GetTileCutIgnoreList: maybe generalize?
	//TODO vanilla seeds to 替换 fully grown herb
	public class ExampleHerb : ModTile
	{
		private const int FrameWidth = 18; // A constant for readability and to kick out those magic numbers

		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileNoFail[Type] = true;
			TileID.Sets.ReplaceTileBreakUp[Type] = true;
			TileID.Sets.IgnoredInHouseScore[Type] = true;
			TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]); // 使 this 图格 interact with golf balls 在 same way other plants do

			// 我们 do not use this because our 图格 should only be spelunkable when it's fully grown. That's why we use the IsTileSpelunkable hook instead
			//Main.tileSpelunker[类型] = 真;

			// Do NOT use this, it causes many unintended side effects
			//Main.tileAlch[类型] = 真;

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(128, 128, 128), name);

			TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
			TileObjectData.newTile.AnchorValidTiles = new int[] {
				TileID.Grass,
				TileID.HallowedGrass,
				ModContent.TileType<ExampleBlock>()
			};
			TileObjectData.newTile.AnchorAlternateTiles = new int[] {
				TileID.ClayPot,
				TileID.PlanterBox
			};
			TileObjectData.addTile(Type);

			HitSound = SoundID.Grass;
			DustType = DustID.Ambient_DarkBrown;
		}

		public override bool CanPlace(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j); // Safe way of getting a 图格 实例

			if (tile.HasTile) {
				int tileType = tile.TileType;
				if (tileType == Type) {
					PlantStage stage = GetStage(i, j); // The current 阶段 的 herb

					// Can only place 在 same herb again if it's grown already
					return stage == PlantStage.Grown;
				}
				else {
					// Support for vanilla herbs/grasses:
					if (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType] || tileType == TileID.WaterDrip || tileType == TileID.LavaDrip || tileType == TileID.HoneyDrip || tileType == TileID.SandDrip) {
						bool foliageGrass = tileType == TileID.Plants || tileType == TileID.Plants2;
						bool moddedFoliage = tileType >= TileID.Count && (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType]);
						bool harvestableVanillaHerb = Main.tileAlch[tileType] && WorldGen.IsHarvestableHerbWithSeed(tileType, tile.TileFrameX / 18);

						if (foliageGrass || moddedFoliage || harvestableVanillaHerb) {
							WorldGen.KillTile(i, j);
							if (!tile.HasTile && Main.netMode == NetmodeID.MultiplayerClient) {
								NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
							}

							return true;
						}
					}

					return false;
				}
			}

			return true;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) {
			if (i % 2 == 0) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
			offsetY = -2; // This is -1 for tiles using StyleAlch, but vanilla sets to -2 for herbs, which causes a slight visual 偏移 between the placement preview and the placed 图格. 
		}

		public override bool CanDrop(int i, int j) {
			PlantStage stage = GetStage(i, j);

			if (stage == PlantStage.Planted) {
				// Do not 放下 anything when just planted
				return false;
			}
			return true;
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j) {
			PlantStage stage = GetStage(i, j);

			Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
			Player nearestPlayer = Main.player[Player.FindClosest(worldPosition, 16, 16)];

			int herbItemType = ModContent.ItemType<ExampleItem>();
			int herbItemStack = 1;

			int seedItemType = ModContent.ItemType<ExampleHerbSeeds>();
			int seedItemStack = 1;

			if (nearestPlayer.active && (nearestPlayer.HeldItem.type == ItemID.StaffofRegrowth || nearestPlayer.HeldItem.type == ItemID.AcornAxe)) {
				// Increased yields with Staff of Regrowth, even when not fully grown
				herbItemStack = Main.rand.Next(1, 3);
				seedItemStack = Main.rand.Next(1, 6);
			}
			else if (stage == PlantStage.Grown) {
				// 默认 yields, only when fully grown
				herbItemStack = 1;
				seedItemStack = Main.rand.Next(1, 4);
			}

			if (herbItemType > 0 && herbItemStack > 0) {
				yield return new Item(herbItemType, herbItemStack);
			}

			if (seedItemType > 0 && seedItemStack > 0) {
				yield return new Item(seedItemType, seedItemStack);
			}
		}

		public override bool IsTileSpelunkable(int i, int j) {
			PlantStage stage = GetStage(i, j);

			// 仅 glow if the herb is grown
			return stage == PlantStage.Grown;
		}

		public override void RandomUpdate(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			PlantStage stage = GetStage(i, j);

			// 仅 grow 到 next 阶段 if there is a next 阶段. We don't want our 图格 turning pink!
			if (stage != PlantStage.Grown) {
				// Increase the x 帧 to change the 阶段
				tile.TileFrameX += FrameWidth;

				// 如果 in multiplayer, 同步 the 帧 change
				if (Main.netMode != NetmodeID.SinglePlayer) {
					NetMessage.SendTileSquare(-1, i, j, 1);
				}
			}
		}

		// 一个 helper 方法 to quickly get the current 阶段 的 herb (assuming the 图格 在 coordinates is our herb)
		private static PlantStage GetStage(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			return (PlantStage)(tile.TileFrameX / FrameWidth);
		}
	}
}
