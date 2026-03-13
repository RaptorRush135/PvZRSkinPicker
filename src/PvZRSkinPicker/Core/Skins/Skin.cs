namespace PvZRSkinPicker.Skins;

using PvZRSkinPicker.Unity.Extensions;

using UnityEngine;
using UnityEngine.AddressableAssets;

internal sealed record Skin(
    SkinId Id,
    string Name,
    AssetReferenceGameObject Prefab)
{
    public static Skin Create(
        string name,
        SkinType skinType,
        AssetReferenceGameObject prefab)
    {
        if (skinType == SkinType.Custom)
        {
            throw new InvalidOperationException("Vanilla skins cannot be Custom.");
        }

        return new(SkinId.Create(skinType), name, prefab);
    }

    public static Skin CreateCustom(
        string name,
        Guid id,
        GameObject prefab)
    {
        return new(SkinId.CreateCustom(id), name, prefab.ToAssetReference());
    }
}
