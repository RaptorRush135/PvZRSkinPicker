namespace PvZRSkinPicker.Skins;

using UnityEngine.AddressableAssets;

internal abstract record Skin(
    SkinType Type,
    string Name,
    AssetReferenceGameObject Prefab,
    string PersistenceId);
