namespace PvZRSkinPicker.Skins;

using UnityEngine.AddressableAssets;

internal sealed record Skin(
    string Name,
    AssetReferenceGameObject Prefab,
    string? Author = null);
