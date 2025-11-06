using ExampleMod.Common.Configs.CustomDataTypes;
using ExampleMod.Content.Prefixes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

// 此文件包含展示创建配置部分的假 ModConfig 类
// 通过使用具有各种数据类型的字段。

// 因为此配置旨在展示各种 用户界面 功能，
// 此配置对模组没有影响，纯粹提供教学示例。
namespace ExampleMod.Common.Configs.ModConfigShowcases
{
	[BackgroundColor(144, 252, 249)]
	public class ModConfigShowcaseDataTypes : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		// 值类型
		public bool SomeBool;
		public int SomeInt;
		public float SomeFloat;
		public string SomeString;
		public EquipType SomeEnum;
		public byte SomeByte;
		public uint SomeUInt;

		// 结构 - 这些需要特殊代码。到目前为止，我们已经实现了 颜色 和 Vector2。
		public Color SomeColor;
		public Vector2 SomeVector2;
		public Point SomePoint; // notice the not implemented 消息.

		// 数据结构（引用类型）
		public int[] SomeArray = new int[] { 25, 70, 12 }; // Arrays have a specific 长度 and need a default 值 specified.
		public List<int> SomeList = new List<int>() { 1, 3, 5 }; // 初始化rs 可以 用于 declare defaults for 数据 structures.
		public Dictionary<string, int> SomeDictionary = new Dictionary<string, int>();
		public HashSet<string> SomeSet = new HashSet<string>();

		// 类（引用类型）- 类在 用户界面 中自动实现。
		public SimpleData SomeClassA;
		// EntityDefinition 类存储由模组或原版添加的实体（物品、NPC、弹幕等）的标识。仅保留标识，不保留其他模组数据或堆叠。
		// 使用 XDefinition 类时，你可以使用 .类型 属性获取物品的 ID。你可以使用 .IsUnloaded 检查有问题的物品是否已加载。
		// 请注意，由于配置在内容之前加载，因此在 ModConfig 代码中使用 XDefinition 类的模组作者必须使用带字符串参数的构造函数。例如，在采用 int 的构造函数中使用 ModContent.XType<ClassName>() 将导致麻烦的错误。
		public ItemDefinition itemDefinitionExample;
		public NPCDefinition npcDefinitionExample = new NPCDefinition(NPCID.Bunny);
		public ProjectileDefinition projectileDefinitionExample = new ProjectileDefinition("ExampleMod", nameof(Content.Projectiles.ExampleHomingProjectile));
		public BuffDefinition buffDefinitionExample = new BuffDefinition("ExampleMod", nameof(Content.Buffs.ExampleDefenseBuff));
		public TileDefinition tileDefinitionExample = new TileDefinition("ExampleMod", nameof(Content.Tiles.ExampleBlock));

		// 引用类型的数据结构
		public Dictionary<PrefixDefinition, float> prefixDefinitionDictionaryExample = new Dictionary<PrefixDefinition, float>() {
			[new PrefixDefinition(nameof(ExampleMod), nameof(ExamplePrefix))] = 0.5f,
			[new PrefixDefinition(PrefixID.Awkward)] = 0.8f,
		};

		// TODO：目前不工作。
		// 在字典中使用自定义类作为键。当用作字典键时，必须使用特殊代码。
		public Dictionary<ClassUsedAsKey, Color> CustomKey = new Dictionary<ClassUsedAsKey, Color>();

		public ModConfigShowcaseDataTypes() {
			// 在构造函数中对引用类型的默认值进行初始化也是可以接受的。
			SomeClassA = new SimpleData() {
				percent = .85f
			};

			CustomKey.Add(new ClassUsedAsKey() {
				SomeBool = true,
				SomeNumber = 42
			},
			new Color(1, 2, 3, 4));

			itemDefinitionExample = new ItemDefinition("Terraria/GoldOre"); // EntityDefinition uses ItemID 字段 names rather than the numbers themselves for readability.
		}
	}
}
