namespace PvZRSkinPicker.Skins;

using SolarApi.Unity.Extensions;

using UnityEngine;
using UnityEngine.AddressableAssets;

internal sealed record Skin(
    SkinId Id,
    string Name,
    AssetReferenceGameObject Prefab,
    AssetReferenceSprite Image,
    SkinPreview? Preview)
{
    public static Skin Create(
        string name,
        SkinType skinType,
        AssetReferenceGameObject prefab,
        AssetReferenceSprite image,
        SkinPreview? preview)
    {
        ArgumentNullException.ThrowIfNull(prefab);
        ArgumentNullException.ThrowIfNull(image);

        if (skinType == SkinType.Custom)
        {
            throw new InvalidOperationException("Vanilla skins cannot be Custom.");
        }

        return new(SkinId.Create(skinType), name, prefab, image, preview);
    }

    public static Skin CreateCustom(
        string name,
        Guid id,
        GameObject prefab,
        AssetReferenceSprite sprite)
    {
        ArgumentNullException.ThrowIfNull(prefab.Ref());
        ArgumentNullException.ThrowIfNull(sprite);

        return new(SkinId.CreateCustom(id), name, prefab.ToAssetReference(), sprite, null);
    }
}
