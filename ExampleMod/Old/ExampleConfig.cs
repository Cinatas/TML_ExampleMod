using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace ExampleMod
{
	// This 文件 contains 2 real ModConfigs (and also a bunch of fake ModConfigs showcasing 各种 ideas). One is set to ConfigScope.ServerSide and the other ConfigScope.ClientSide
	// ModConfigs contain Public Fields and Properties that represent the choices available 到 用户. 
	// Those Fields or Properties 将 presented to users 在 配置 菜单.
	// DONT use static members 任何where in this 类 (除了 an automatically assigned 字段 named 实例 与 same 类型 as the ModConfig 类, if you'd rather write "MyConfigClass.实例" 代替 "ModContent.GetInstance<MyConfigClass>()"), tModLoader maintains 几个 instances of ModConfig classes which will not work well with static properties or fields.

	/// <summary>
	/// ExampleConfigServer has 服务器-wide effects. Things that happen 在 服务器, 在 世界, or influence autoload go here
	/// ConfigScope.ServerSide ModConfigs are SHARED 从 服务器 to all clients connecting in MP.
	/// </summary>
	public class ExampleConfigServer : ModConfig
	{
		// You MUST specify a ConfigScope.
		public override ConfigScope Mode => ConfigScope.ServerSide;

		// 我们将 use attributes to annotate our fields or properties so tModLoader can properly 处理 them.

		// 首先, we will learn about DefaultValue. You might assume "public bool BoolExample = 真;" to work, 
		// but because tModLoader is overwriting with JSON, that 值 将 overwritten when the mod loads.
		// 我们必须 use the DefaultValue attribute 代替 设置 the 值 normally:
		[DefaultValue(true)]
		public bool UselessBoolExample;

		// This is private. You'll notice that it doesn't show up 在 配置 菜单. Don't set something private.
#pragma warning disable CS0169 // Unused 字段
		private bool PrivateFieldBoolExample;
#pragma warning restore CS0169

		// This is ignored, it also shouldn't show up 在 配置 菜单 despite being public.
		[JsonIgnore]
		public bool IgnoreExample;

		// You'll notice this next one is a 属性 代替 a 字段. That works too.
		// Here we see an attribute added by tModLoader: LabelAttribute. This one allows us to add a 标签 so the 用户 knows more about the 设置 they are changing. Without a 标签, the 名称 的 字段 or 属性 is displayed.
		[Label("Disable Example Wings Item")]
		// Similar to 标签, this sets the 工具提示. Tooltips are useful for slightly longer and more detailed explanations of 配置 options.
		[Tooltip("Prevents Loading the ExampleWings item. Requires a Reload")]
		// ReloadRequired hints that if this 值 is changed, a 重新加载 is required 对于 mod to properly work. 
		// Here we use it so if we 禁用 ExampleWings from being loaded, we can properly 防止 autoload in ExampleWings.cs
		// Failure to properly use ReloadRequired will cause m任何, m任何 problems including ID desync.
		[ReloadRequired]
		public bool DisableExampleWings { get; set; }

		[Label("Disable Volcanoes")]
		// Our game logic can 处理 toggling this 设置 in-game, so you'll notice we do NOT decorate this 属性 with ReloadRequired
		public bool DisableVolcanoes { get; set; }

		// Watch in action: https://gfycat.com/SickTerribleHoatzin
		[Label("Example Person free gift list")]
		[Tooltip("Each player can claim one free item from this list from Example Person\nSell the item back to Example Person to take a new item")]
		public List<ItemDefinition> ExamplePersonFreeGiftList { get; set; } = new List<ItemDefinition>();

		// AcceptClientChanges is called 在 服务器 when a 客户端 玩家 attempts to change ServerSide settings in-game. 默认情况下, 客户端 changes are accepted. (As long as they don't necessitate a 重新加载)
		// With more effort, a mod could implement more 控制 over changing mod settings.
		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message) {
			if (Main.player[whoAmI].name == "jopojelly") {
				message = "Sorry, players named jopojelly aren't allowed to change settings.";
				return false;
			}
			return true;
		}

		// While ReloadRequired is sufficient for most, some may require more logic in deciding if a 重新加载 is required. Here is an incomplete example
		/*public override bool NeedsReload(ModConfig pendingConfig)
		{
			bool defaultDecision = base.NeedsReload(pendingConfig);
			bool otherLogic = IntExample > (pendingConfig as ExampleConfigServer).IntExample; // This is just a 随机 example. Your logic depends on your mod.
			return defaultDecision || otherLogic; // 重新加载 needed if 任一 条件 is met.
		}*/
	}

	/// <summary>
	/// This 配置 operates on a per-客户端 basis. 
	/// These parameters are local to this computer and are NOT synced 从 服务器.
	/// </summary>
	public class ExampleConfigClient : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Label("Show the coin rate UI")]
		public bool ShowCoinUI;

		[Label("Show mod origin in tooltip")]
		public bool ShowModOriginTooltip;

		public override void OnChanged() {
			// Here we use the OnChanged hook to initialize ExampleUI.visible 与 new values.
			// We maintain 两者 ExampleUI.visible and ShowCoinUI as 分离 values so ShowCoinUI can act as a default while ExampleUI.visible can change within a play 会话.
			UI.ExampleUI.Visible = ShowCoinUI;
		}
	}
}
