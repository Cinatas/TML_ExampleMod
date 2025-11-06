using Microsoft.Xna.Framework.Audio;
using Terraria.ModLoader;

namespace ExampleMod.Sounds.Item
{
	public class Wooo : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type) {
			// By creating a new 实例, this ModSound allows for overlapping sounds. Non-ModSound behavior is to restart the 声音, only permitting 1 实例.
			soundInstance = sound.CreateInstance();
			soundInstance.Volume = volume * .5f;
			soundInstance.Pan = pan;
			soundInstance.Pitch = -1.0f;
			return soundInstance;
		}
	}
}
