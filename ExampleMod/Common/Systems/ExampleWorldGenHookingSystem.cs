using MonoMod.Cil;
using System;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace ExampleMod.Common.Systems
{
	// 此 ModSystem 将演示如何 IL 编辑和 Detour 世界生成过程
	// 由于世界生成过程是匿名方法（它们没有名称），因此无法以标准方式编辑它们（使用 IL_xx 或 On_xx）
	public class ExampleWorldGenHookingSystem : ModSystem
	{
		// 所有注册都应该在加载中进行
		// 生成过程钩子是手动卸载的，因此不需要 Unload 方法
		public override void Load() {
			// IL 编辑金字塔过程
			WorldGen.ModifyPass((PassLegacy)WorldGen.VanillaGenPasses["Pyramids"], Modify_Pyramids);

			// Detouring the shinies pass (generates ore)
			WorldGen.DetourPass((PassLegacy)WorldGen.VanillaGenPasses["Shinies"], Detour_Shinies);
		}

		void Modify_Pyramids(ILContext il) {
			try {
				var c = new ILCursor(il);
				c.EmitDelegate(() => ModContent.GetInstance<ExampleMod>().Logger.Debug("(In ILHook) Generating Pyramids"));
			}
			catch (Exception) {
				MonoModHooks.DumpIL(ModContent.GetInstance<ExampleMod>(), il);
			}
		}

		// Detouring 应该 the same (除了 one thing mentioned below), this is just an example so you can check this is actually working
		// One thing to note is that for technical reasons, the self 参数 is an 对象 类型
		// 你将 never 需要 actually cast it to 类型 WorldGen though, since it contains no 实例 fields or methods
		void Detour_Shinies(WorldGen.orig_GenPassDetour orig, object self, GenerationProgress progress, GameConfiguration configuration) {
			ModContent.GetInstance<ExampleMod>().Logger.Debug("(On Hook) Before Shinies");
			orig(self, progress, configuration);
			ModContent.GetInstance<ExampleMod>().Logger.Debug("(On Hook) After Shinies");
		}
	}
}
