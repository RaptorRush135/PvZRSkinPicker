namespace PvZRSkinPicker.Skins;

using UnityEngine.AddressableAssets;

internal sealed record VanillaSkin<T> : Skin
    where T : struct, Enum
{
    public VanillaSkin(
        T type,
        string name,
        SkinType skinType,
        AssetReferenceGameObject prefab)
        : base(skinType, name, prefab, $"{type}:{skinType}")
    {
        if (skinType == SkinType.Custom)
        {
            throw new InvalidOperationException("Vanilla skins cannot be Custom.");
        }
    }
}
