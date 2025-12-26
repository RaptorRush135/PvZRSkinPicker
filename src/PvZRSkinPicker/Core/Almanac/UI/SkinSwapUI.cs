namespace PvZRSkinPicker.Almanac.UI;

using Il2CppReloaded.Data;

using PvZRSkinPicker.Assets;

using UnityEngine;

internal static class SkinSwapUI
{
    private static ModResourceName IconResourceName => ModAssets.SkinSwap;

    private static Sprite Icon => field ??= ModAssets.LoadSprite(IconResourceName);

    public static ModButton CreateButton(AlmanacEntryType type)
    {
        return AlmanacUI.CreatePortraitOverlayButton(
            IconResourceName.GetNameWithoutExtension(),
            Icon,
            type);
    }
}
