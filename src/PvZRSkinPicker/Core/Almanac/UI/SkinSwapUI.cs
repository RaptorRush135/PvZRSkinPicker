namespace PvZRSkinPicker.Almanac.UI;

using Il2CppReloaded.Data;

using PvZRSkinPicker.Assets;

using UnityEngine;

internal static class SkinSwapUI
{
    private static EmbeddedResourceAsset IconAsset => ModAssets.SkinSwap;

    private static Sprite Icon => field ??= ModAssets.LoadSprite(IconAsset);

    public static ModButton CreateButton(AlmanacEntryType type)
    {
        return AlmanacUI.CreatePortraitOverlayButton(
            IconAsset.GetNameWithoutExtension(),
            Icon,
            type);
    }
}
