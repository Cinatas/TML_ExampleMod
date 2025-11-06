using Terraria.ModLoader;

namespace ExampleMod.Content.Walls
{
	public class ExampleGlobalWall : GlobalWall
	{
		public override bool WallFrame(int i, int j, int type, bool randomizeFrame, ref int style, ref int frameNumber) {
			// 此代码是一个学习工具，用于可视化何时调用此钩子。取消注释代码并重新构建以进行测试。
			/*
			if (Main.rand.NextBool(20)) {
				Dust.NewDustPerfect(new Vector2(i * 16 + 8, j * 16 + 8), ModContent.DustType<Sparkle>(), Vector2.Zero);
			}
			*/

			// 此钩子的更典型用法是实现自定义墙体框架模式。

			return base.WallFrame(i, j, type, randomizeFrame, ref style, ref frameNumber);
		}
	}
}