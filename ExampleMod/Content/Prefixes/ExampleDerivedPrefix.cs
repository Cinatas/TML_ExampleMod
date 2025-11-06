namespace ExampleMod.Content.Prefixes
{
	// Be sure to see 'ExamplePrefix' first.
	// 此类 showcases how you can use inheritance to have an easier 时间 with making variants of a single 前缀.
	// With that said, remember that inheritance is just one 的 thousands of tools that are avail能够 programmers, and that with great power comes great responsibility.
	public class ExampleDerivedPrefix : ExamplePrefix // Deriving from ExamplePrefix!
	{
		// Overriding the Power 属性 to make it 返回 2.0 代替 1.0!
		// 如果 you 想要 modify the base 类' implementation's 值, you can write 'base.Power' to 引用 it. 
		public override float Power => base.Power * 2f;

		// No need for 任何thing else, since members get inherited 从 base 类.
	}
}
