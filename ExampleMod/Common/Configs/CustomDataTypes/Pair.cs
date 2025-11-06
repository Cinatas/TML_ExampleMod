using Terraria.ModLoader.Config;

// 此文件定义具有两个字段的自定义简单数据类型 - 布尔值和整数值。
namespace ExampleMod.Common.Configs.CustomDataTypes
{
	[BackgroundColor(0, 255, 255)]
	public class Pair
	{
		public bool enabled;
		public int boost;

		// 如果你重写 ToString，它将显示为附加到 ModConfig UI 中的标签。
		public override string ToString() {
			return $"Boost: {(enabled ? "" + boost : "disabled")}";
		}

		// 为你使用的任何类实现 Equals 和 GetHashCode 至关重要。
		public override bool Equals(object obj) {
			if (obj is Pair other)
				return enabled == other.enabled && boost == other.boost;
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return new { boost, enabled }.GetHashCode();
		}
	}
}
