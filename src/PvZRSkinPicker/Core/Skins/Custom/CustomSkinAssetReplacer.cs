namespace PvZRSkinPicker.Skins.Custom;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

using Il2CppSpine.Unity;

using MelonLoader;

using PvZRSkinPicker.Unity.Extensions;

using UnityEngine;

internal static class CustomSkinAssetReplacer
{
    private static readonly MelonLogger.Instance Logger = Melon<Core>.Logger;

    public static bool TryReplace(
        SkeletonAnimation animation,
        Texture2D? texture,
        string? atlasText,
        byte[]? skeletonData)
    {
        return TryReplace(animation, texture, ConvertAtlasTextFile(), ConvertSkeletonDataFile());

        TextAsset? ConvertAtlasTextFile()
        {
            return atlasText == null
                ? null
                : new TextAsset(atlasText);
        }

        TextAsset? ConvertSkeletonDataFile()
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

    public static bool TryReplace(
        SkeletonAnimation animation,
        Texture2D? texture,
        TextAsset? atlasText,
        TextAsset? skeletonData)
    {
        if (texture == null && atlasText == null && skeletonData == null)
        {
            Logger.Warning("No replacement data provided");
            return false;
        }

        if (!TryReplaceAtlas(animation, texture, atlasText, out var atlas))
        {
            return false;
        }

        bool atlasChanged = atlas != animation.SkeletonDataAsset.atlasAssets[0];

        if (skeletonData == null && !atlasChanged)
        {
            return true;
        }

        var skeletonSource = skeletonData.Ref() ?? animation.skeletonDataAsset.skeletonJSON;

        var newSkeleton = SkeletonDataAsset.CreateRuntimeInstance(
           skeletonSource,
           atlas,
           initialize: false,
           scale: animation.skeletonDataAsset.scale);

        if (newSkeleton.GetSkeletonData(quiet: false) == null)
        {
            Logger.Warning(
                "Failed to initialize skeleton. " +
                "Make sure the skeleton format version is 4.2.x");

            return false;
        }

        animation.skeletonDataAsset = newSkeleton;
        animation.Initialize(overwrite: true);

        if (!animation.valid)
        {
            Logger.Warning("Animation initialization failed");
            return false;
        }

        return true;
    }

    private static bool TryReplaceAtlas(
        SkeletonAnimation animation,
        Texture2D? texture,
        TextAsset? atlasText,
        out AtlasAssetBase atlas)
    {
        var currentAtlas = animation.SkeletonDataAsset.atlasAssets[0];
        var currentTexture = currentAtlas.PrimaryMaterial.mainTexture.Cast<Texture2D>();
        texture.Ref()?.name = currentTexture.name;

        if (atlasText == null)
        {
            if (texture == null)
            {
                atlas = currentAtlas;
                return true;
            }

            atlas = CreateAtlas(currentAtlas.Cast<SpineAtlasAsset>().atlasFile, texture, currentAtlas.PrimaryMaterial);
        }
        else
        {
            var atlasTexture = texture.Ref() ?? currentTexture;
            atlas = CreateAtlas(atlasText, atlasTexture, currentAtlas.PrimaryMaterial);
        }

        if (atlas.GetAtlas() == null)
        {
            Logger.Warning("Failed to initialize atlas");
            return false;
        }

        return true;
    }

    private static SpineAtlasAsset CreateAtlas(
        TextAsset atlasText,
        Texture2D texture,
        Material material)
    {
        ArgumentNullException.ThrowIfNull(atlasText);
        ArgumentNullException.ThrowIfNull(texture);
        ArgumentNullException.ThrowIfNull(material);

        var newMaterial = new Material(material)
        {
            mainTexture = texture,
        };

        return SpineAtlasAsset.CreateRuntimeInstance(
            atlasText,
            new([newMaterial]),
            initialize: false);
    }
}
