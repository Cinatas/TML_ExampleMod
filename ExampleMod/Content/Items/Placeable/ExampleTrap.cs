using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	// This 项 shows off using 1 类 to 加载 multiple items. This is an alternate to typical inheritance.
	// Read the comments in this example carefully, as there are many parts necessary to make this approach work.
	// real strength of this approach is when you have many items that vary by small changes, like how these 2 陷阱 items vary only by placeStyle.
	public class ExampleTrap : ModItem
	{
		// This inner 类 is an ILoadable, the game will automatically call the 加载 方法 when loading this mod.
		// 使用 this 类, we manually call AddContent with 2 instances 的 ExampleTrap 类. This adds them 到 game.
		public class ExampleTrapLoader : ILoadable
		{
			public void Load(Mod mod) {
				mod.AddContent(new ExampleTrap(0));
				mod.AddContent(new ExampleTrap(1));
			}

			public void Unload() {
			}
		}

		// CloneNewInstances is needed so that fields in this 类 are Cloned onto new instances, 例如 when this 项 is crafted or hovered over.
		// 默认情况下, the game creates new instances rather than clone. By forcing Clone, we can preserve fields per 项 added by the mod while sharing the same 类.
		protected override bool CloneNewInstances => true;
		private readonly int placeStyle;

		// internal 名称 of each ModItem 必须 unique. This code ensures that each 的 2 ExampleTrap instances added have a unique 名称.
		// 在 the localization files, these internal names are used as keys for DisplayName and 工具提示, rather than the classname.
		public override string Name => GetInternalNameFromStyle(placeStyle);

		// This helper 方法 converts 从 custom instanced 数据 到 internal 名称. In this example the placeStyle 值 is the only custom 数据.
		// 此方法 is called by the 名称 属性 and 
		public static string GetInternalNameFromStyle(int style) {
			// 在这里 we define some strings that 将 used as the ModItem.名称, the internal 名称 的 ModItem.
			// Every ModItem must have a unique internal 名称, so this 步骤 is necessary.
			// 我们 use these 在 ExampleMod.Content.Tiles.ExampleTrap.GetItemDrops rather than ModContent.ItemType<Items.Placeable.ExampleTrap>() to retrieve the correct ItemID.
			if (style == 0) {
				return "ExampleTrapIchorBullet";
			}
			if (style == 1) {
				return "ExampleTrapChlorophyteBullet";
			}
			throw new Exception("Invalid style");
		}

		// Content loaded multiple times must have a non-default constructor. This is where unique 数据 is passed in to be used later. This also prevents the game from attempting to add this ModItem 到 game automatically.
		public ExampleTrap(int placeStyle) {
			this.placeStyle = placeStyle;
		}

		public override void SetDefaults() {
			// With all the setup above, placeStyle 将 either 0 or 1 对于 2 ExampleTrap instances we've loaded.
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.ExampleTrap>(), placeStyle);

			Item.width = 12;
			Item.height = 12;
			Item.value = 10000;
			Item.mech = true; // lets you see wires while holding.
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.DartTrap)
				.Register();
		}
	}
}
