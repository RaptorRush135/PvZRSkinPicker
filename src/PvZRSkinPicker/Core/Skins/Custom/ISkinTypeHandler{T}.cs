namespace PvZRSkinPicker.Skins.Custom;

using Il2CppSpine.Unity;

using PvZRSkinPicker.Almanac.SeedPackets.Renderer;
using PvZRSkinPicker.Skins.Custom.Manifest;

using UnityEngine;
using UnityEngine.AddressableAssets;

internal interface ISkinTypeHandler<T>
    where T : struct, Enum
{
    bool IsSkinPickerSupported(T type);

    AssetReferenceGameObject GetPrefab(T type);

    SkeletonAnimation GetSkeletonAnimation(GameObject @object);

    string? GetInitialSkinName(SkeletonAnimation animation, T type);

    SkinTransform GetDefaultSeedPacketTransform(T type);

    Sprite RenderSrite(GameObject prefab, PacketRenderSpec<T> renderSpec);
}
