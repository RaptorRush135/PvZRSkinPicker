namespace PvZRSkinPicker.Skins;

using PvZRSkinPicker.Unity.Extensions;

using UnityEngine;
using UnityEngine.AddressableAssets;

internal sealed record Skin(
    SkinType Type,
    string Name,
    AssetReferenceGameObject Prefab,
    string PersistenceId)
{
    public static Skin Create<T>(
        T type,
        string name,
        SkinType skinType,
        AssetReferenceGameObject prefab)
        where T : struct, Enum
    {
        if (skinType == SkinType.Custom)
        {
            throw new InvalidOperationException("Vanilla skins cannot be Custom.");
        }

        return new(skinType, name, prefab, $"{type}:{skinType}");
    }

    public static Skin CreateCustom(
        string name,
        Guid packId,
        Guid id,
        GameObject prefab)
    {
        return new(SkinType.Custom, name, prefab.ToAssetReference(), $"{packId}:{id}");
    }
}
