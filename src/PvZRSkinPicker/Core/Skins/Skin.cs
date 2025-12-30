namespace PvZRSkinPicker.Skins;

using UnityEngine.AddressableAssets;

internal sealed record Skin(
    SkinType Type,
    AssetReferenceGameObject Prefab);
