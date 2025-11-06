using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ExampleMod.Content.Items.Consumables
{
	// This showcases how the CanStack hook 可以 used in conjunction with custom data
	// 自定义 data is also shown in ExampleDataItem, but here we need to use more hooks

	// This item, when crafted, stores the players name, and only lets other players open it. Bags 与 same stored name aren't stackable
	public class ExampleCanStackItem : ModItem
	{
		// 我们 set this when the item is crafted. In other contexts, this 将 an empty string
		public string craftedPlayerName = string.Empty;

		public override void SetDefaults() {
			Item.maxStack = Item.CommonMaxStack; // This item is stackable, otherwise the example wouldn't work
			Item.consumable = true;
			Item.width = 22;
			Item.height = 26;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool CanRightClick() {
			// bag can't be opened if it wasn't crafted
			if (craftedPlayerName == string.Empty) {
				return false;
			}

			// bag can't be opened by the player who crafted it
			return Main.LocalPlayer.name != craftedPlayerName;
		}

		public override bool CanStack(Item source) {
			// bag can only be stacked with other bags if the names match

			// 我们 have to cast the second item 到 class (This is safe to do as the hook is only called on items 的 same type)
			var name1 = craftedPlayerName;
			var name2 = ((ExampleCanStackItem)source.ModItem).craftedPlayerName;

			// let items which have been spawned in and not assigned to a player, to stack with other bags the the current player owns
			// This lets you craft multiple items in到 mouse-held stack
			if (name1 == string.Empty) {
				name1 = Main.LocalPlayer.name;
			}
			if (name2 == string.Empty) {
				name2 = Main.LocalPlayer.name;
			}

			return name1 == name2;
		}

		public override void OnStack(Item source, int numToTransfer) {
			// Combined with CanStack above, this ensures that empty spawned items can combine with bags made by the current player
			if (craftedPlayerName == string.Empty) {
				craftedPlayerName = ((ExampleCanStackItem)source.ModItem).craftedPlayerName;
			}
		}

		public override void ModifyItemLoot(ItemLoot itemLoot) {
			LeadingConditionRule hardmodeCondition = new(new Conditions.IsHardmode());
			hardmodeCondition.OnSuccess(ItemDropRule.Common(ItemID.ChocolateChipCookie));
			hardmodeCondition.OnFailedConditions(ItemDropRule.Common(ItemID.Coconut));
			itemLoot.Add(hardmodeCondition);
		}

		// following 4 hooks are needed if your item data 应该 persistent between saves, and work in multiplayer
		public override void SaveData(TagCompound tag) {
			tag.Add("craftedPlayerName", craftedPlayerName);
		}

		public override void LoadData(TagCompound tag) {
			craftedPlayerName = tag.GetString("craftedPlayerName");
		}

		public override void NetSend(BinaryWriter writer) {
			writer.Write(craftedPlayerName);
		}

		public override void NetReceive(BinaryReader reader) {
			craftedPlayerName = reader.ReadString();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			if (craftedPlayerName != string.Empty) {
				// 在这里 we make a distinction to disclose th在 bag can't be opened by the player who crafted it
				if (Main.LocalPlayer.name == craftedPlayerName) {
					tooltips.Add(new TooltipLine(Mod, "CraftedPlayerNameCannotOpen", $"You crafted this bag and cannot open it!"));
				}
				else {
					tooltips.Add(new TooltipLine(Mod, "CraftedPlayerNameOther", $"This is a bag from {craftedPlayerName}, open it to receive a gift!"));
				}
			}
			else {
				tooltips.Add(new TooltipLine(Mod, "CraftedPlayerNameEmpty", $"This bag was not crafted, it will do nothing"));
			}
		}

		public override void OnCreated(ItemCreationContext context) {
			if (context is RecipeItemCreationContext) {
				// 如果 the item was crafted, store the crafting players name
				craftedPlayerName = Main.LocalPlayer.name;
			}
		}

		public override void AddRecipes() {
			CreateRecipe().
				AddIngredient<ExampleItem>(20).
				AddTile(TileID.WorkBenches).
				Register();
		}
	}
}
