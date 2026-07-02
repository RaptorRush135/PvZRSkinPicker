namespace PvZRSkinPicker.Almanac.Extensions;

using Il2CppReloaded.DataModels;

using UnityEngine.AddressableAssets;

internal static class AlmanacEntryModelExtensions
{
    public static void SetThumbnail(
        this AddressableSpriteValueModel thumbnailModel,
        AssetReferenceSprite? sprite)
    {
        if (sprite == null)
        {
            return;
        }

        thumbnailModel.Reference = sprite;
    }
}
