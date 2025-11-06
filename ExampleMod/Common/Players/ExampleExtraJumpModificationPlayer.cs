using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	// 展示修改沙暴瓶中的额外跳跃
	public class ExampleExtraJumpModificationPlayer : ModPlayer
	{
		public override void ModifyExtraJumpDurationMultiplier(ExtraJump jump, ref float duration) {
			// 使跳跃持续时间比正常时间长 2 倍
			if (jump == ExtraJump.SandstormInABottle)
				duration *= 2f;
		}
	}
}
