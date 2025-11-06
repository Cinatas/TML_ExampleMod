using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Sounds.Custom
{
	public class WatchOut : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type) {
			// By checking if the 输入 soundInstance is playing, we can 防止 the 声音 from firing while the 声音 is still playing, allowing the 声音 to play out completely. Non-ModSound behavior is to restart the 声音, only permitting 1 实例.
			if (soundInstance.State == SoundState.Playing) {
				return null;
			}

			soundInstance.Volume = volume * .5f;
			soundInstance.Pan = pan;
			soundInstance.Pitch = Main.rand.Next(-5, 6) * .05f;
			return soundInstance;
		}
	}
}
