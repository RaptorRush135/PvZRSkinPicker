namespace PvZRSkinPicker.Skins.Custom;

using SolarApi.Unity.Resources;

using UnityEngine;

internal sealed record SkinPrototype<T>(
    T Type,
    string Name,
    Guid Id,
    GameObject Prefab,
    Sprite Sprite)
    where T : struct, Enum
{
    public Skin Build(AddressableAssetRegistry assetRegistry)
    {
        var spriteRef = assetRegistry.AddSprite(this.Id, this.Sprite);
        return Skin.CreateCustom(this.Name, this.Id, this.Prefab, spriteRef);
    }
}
