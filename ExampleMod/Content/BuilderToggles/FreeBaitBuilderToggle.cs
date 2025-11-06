using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.BuilderToggles;

// 此示例 shows almost all BuilderToggle hooks.
// As it is just an example, it behaves more like a "按钮" than a "toggle".
// 左 clicking allows you to select bait 类型 and 右 clicking gives you 10 free bait 的 selected 类型.
// 自定义 drawing is showcased in this example to 处理 帧 changes.
public class FreeBaitBuilderToggle : BuilderToggle
{
	public static LocalizedText NameText { get; private set; }

	public override string HoverTexture => Texture;

	public override bool Active() => !Main.LocalPlayer.HeldItem.IsAir && Main.LocalPlayer.HeldItem.fishingPole > 0;

	public override int NumberOfStates => 4;

	// Sorted after Torch God toggle because that 将 cool.
	public override Position OrderPosition => new After(TorchBiome);

	public override bool OnLeftClick(ref SoundStyle? sound) {
		// 更改 the 点击 声音.
		// 如果 you don't want a 声音 to play, set 声音 to 空.
		sound = SoundID.DrumTomHigh;
		return true;
	}

	public override void OnRightClick() {
		// Give the 玩家 free baits when 右 clicked.
		SoundEngine.PlaySound(Main.rand.NextBool() ? SoundID.DrumCymbal1 : SoundID.DrumCymbal2);
		int itemType = CurrentState switch {
			0 => ItemID.ApprenticeBait,
			1 => ItemID.JourneymanBait,
			2 => ItemID.MasterBait,
			3 => ItemID.TruffleWorm,
			_ => throw new ArgumentOutOfRangeException()
		};

		Main.LocalPlayer.QuickSpawnItem(new EntitySource_Gift(Main.LocalPlayer), itemType, 10);
	}

	// 使用 custom drawing to 处理 帧 changes.
	public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams) {
		drawParams.Frame = drawParams.Texture.Frame(4, 2, CurrentState % 4);
		return true;
	}

	// Truffle Worm has a unique 悬停 纹理.
	public override bool DrawHover(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams) {
		int column = CurrentState == 3 ? 1 : 0; // The 悬停 纹理 for TruffleWorm is unique
		drawParams.Frame = drawParams.Texture.Frame(4, 2, column, 1);
		return true;
	}

	public override void SetStaticDefaults() {
		NameText = this.GetLocalization(nameof(NameText));
	}

	public override string DisplayValue() {
		string itemName = CurrentState switch {
			0 => Lang.GetItemNameValue(ItemID.ApprenticeBait),
			1 => Lang.GetItemNameValue(ItemID.JourneymanBait),
			2 => Lang.GetItemNameValue(ItemID.MasterBait),
			3 => Lang.GetItemNameValue(ItemID.TruffleWorm),
			_ => "Unknown (How did you get here?)"
		};
		return NameText.Format(itemName);
	}
}