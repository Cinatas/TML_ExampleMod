using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	public class ExampleLuckPlayer : ModPlayer
	{
		public override void ModifyLuck(ref float luck) { // 修改Luck is what you'll normally use for any modded content that wants to modify luck.
			// 总运气在原版中有一个软上限为 1。你技术上可以超过该值，但使用原版运气计算没有好处。
			// 但是，模组作者可以按照他们想要的方式使用运气值，因此超过 1 可能是有益的。不过，仍然建议使用十进制值。
			if (Main.hardMode) { // If it is currently hardmode...
				luck += 0.5f; // ...add 0.5 luck to the total luck count!
			}
			// 当然，你也可以使运气为负，在这种情况下，软上限为 -1。

			// As the above code runs every time luck is calculated, and `hardMode` is accessible on both client and server, we don't need to worry about multiplayer syncing.
			// If you have some code which relies on client side calculations, you will need to sync the variables to calculate luck correctly on the server.
		}

		public override bool PreModifyLuck(ref float luck) { // PreModifyLuck is useful if you want to modify any vanilla luck values or want to prevent vanilla luck calculations from happening.
			Terraria.GameContent.Events.LanternNight.GenuineLanterns = true; // The game now thinks its a Lantern Night all the time, giving you the luck bonus.
			Player.HasGardenGnomeNearby = true; // The game now thinks there's a garden gnome nearby all the time, giving you the luck bonus.

			if (Player.ladyBugLuckTimeLeft < 0) { // If you have bad ladybug luck...
				Player.ladyBugLuckTimeLeft = 0; // ...completely cancel it out.
			}

			return true; // PreModifyLuck returns true by default, but you can also return false if you want to prevent vanilla luck calculations from happening at all.
		}
	}
}
