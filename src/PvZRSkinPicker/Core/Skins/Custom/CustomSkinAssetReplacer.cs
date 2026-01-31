namespace PvZRSkinPicker.Skins.Custom;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

using Il2CppSpine.Unity;

using UnityEngine;

internal static class CustomSkinAssetReplacer
{
    public static bool TryReplace(SkeletonAnimation animation, byte[]? skeletonData)
    {
        // TODO: Atlas
        // TODO: Texture
        return TryReplace(animation, GetSkeletonDataFile());

        TextAsset? GetSkeletonDataFile()
        {
            if (skeletonData == null)
            {
                return null;
            }

            var skeletonBytes = new Il2CppStructArray<byte>(skeletonData);
            return new TextAsset(skeletonBytes)
            {
                name = animation.skeletonDataAsset.skeletonJSON.name,
            };
        }
    }

    public static bool TryReplace(SkeletonAnimation animation, TextAsset? skeletonData)
    {
        if (skeletonData == null)
        {
            return true;
        }

        animation.skeletonDataAsset = SkeletonDataAsset.CreateRuntimeInstance(
            skeletonData,
            animation.skeletonDataAsset.atlasAssets,
            initialize: true,
            scale: animation.skeletonDataAsset.scale);

        animation.Initialize(overwrite: true);

        return animation.valid;
    }
}
