namespace PvZRSkinPicker.Skins;

using PvZRSkinPicker.Unity.Extensions;

using UnityEngine;

internal sealed record CustomSkin : Skin
{
    public CustomSkin(string name, Guid packId, Guid id, GameObject prefab)
        : base(SkinType.Custom, name, prefab.ToAssetReference(), $"{packId}:{id}")
    {
    }
}
