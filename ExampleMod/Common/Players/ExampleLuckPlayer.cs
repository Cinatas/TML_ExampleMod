using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	public class ExampleLuckPlayer : ModPlayer
	{
		public override void ModifyLuck(ref float luck) { // 修改Luck is what you'll normally use for 任何 modded content that wants to modify luck.
			// 总运气在原版中有一个软上限为 1。你技术上可以超过该值，但使用原版运气计算没有好处。
			// 但是，模组作者可以按照他们想要的方式使用运气值，因此超过 1 可能是有益的。不过，仍然建议使用十进制值。
			if (Main.hardMode) { // If it is currently hardmode...
				luck += 0.5f; // ...add 0.5 luck 到 total luck 计数!
			}
			// 当然，你也可以使运气为负，在这种情况下，软上限为 -1。

			// As the above code runs 每次 luck is calculated, and `hardMode` is accessible on 两者 客户端 and 服务器, we don't 需要 worry about multiplayer syncing.
			// If you have some code which relies on 客户端 side calculations, you will 需要 同步 the variables to calculate luck correctly 在 服务器.
		}

		public override bool PreModifyLuck(ref float luck) { // PreModifyLuck is useful if you 想要 modify 任何 vanilla luck values or 想要 防止 vanilla luck calculations from happening.
			Terraria.GameContent.Events.LanternNight.GenuineLanterns = true; // The game now thinks its a Lantern Night all the 时间, giving you the luck 奖励.
			Player.HasGardenGnomeNearby = true; // The game now thinks there's a garden gnome nearby all the 时间, giving you the luck 奖励.

			if (Player.ladyBugLuckTimeLeft < 0) { // If you have bad ladybug luck...
				Player.ladyBugLuckTimeLeft = 0; // ...completely 取消 it out.
			}

			return true; // PreModifyLuck returns 真 默认情况下, but you can also 返回 假 if you 想要 防止 vanilla luck calculations from happening at all.
		}
	}
}
