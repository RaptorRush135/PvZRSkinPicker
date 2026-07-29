namespace PvZRSkinPicker.Skins.Custom;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

using Il2CppSpine.Unity;

using MelonLoader;

using SolarApi.Unity.Extensions;

using UnityEngine;

internal static class CustomSkinAssetReplacer
{
    private static readonly MelonLogger.Instance Logger = Melon<Core>.Logger;

    public static bool TryReplace(
        SkeletonAnimation animation,
        Texture2D? texture,
        string? atlasText,
        byte[]? skeletonData,
        string? initialSkinName)
    {
        ArgumentNullException.ThrowIfNull(animation);

        return TryReplace(animation, texture, ConvertAtlasTextFile(), ConvertSkeletonDataFile(), initialSkinName);

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

    private static bool TryReplace(
        SkeletonAnimation animation,
        Texture2D? texture,
        TextAsset? atlasText,
        TextAsset? skeletonData,
        string? initialSkinName)
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

        var originalSkeletonData = animation.SkeletonDataAsset;

        bool atlasChanged = atlas != originalSkeletonData.atlasAssets[0];

        if (skeletonData == null && !atlasChanged)
        {
            return true;
        }

        var skeletonSource = skeletonData.Ref() ?? originalSkeletonData.skeletonJSON;

        var newSkeleton = SkeletonDataAsset.CreateRuntimeInstance(
           skeletonSource,
           atlas,
           initialize: false,
           scale: originalSkeletonData.scale);

        // TODO: Allow override
        newSkeleton.fromAnimation = originalSkeletonData.fromAnimation;
        newSkeleton.toAnimation = originalSkeletonData.toAnimation;
        newSkeleton.duration = originalSkeletonData.duration;
        newSkeleton.defaultMix = originalSkeletonData.defaultMix;

        if (newSkeleton.GetSkeletonData(quiet: false) == null)
        {
            Logger.Warning(
                "Failed to initialize skeleton. " +
                "Make sure the skeleton format version is 4.2.x");

            return false;
        }

        animation.initialSkinName = newSkeleton.skeletonData.FindSkin(initialSkinName) != null
            ? initialSkinName
            : null;

        animation.skeletonDataAsset = newSkeleton;
        animation.Initialize(overwrite: true);

        if (!animation.valid)
        {
            Logger.Warning("Animation initialization failed");
            return false;
        }

        if (animation.meshRenderer.sharedMaterial == null)
        {
            Logger.Warning("No material in the mesh renderer. " +
                "This is likely caused by not having visible attachments in the default pose");

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
            if (!AtlasPageNameMatches(atlasText.text, currentTexture.name))
            {
                atlas = currentAtlas;
                return false;
            }

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

    private static bool AtlasPageNameMatches(ReadOnlySpan<char> text, ReadOnlySpan<char> expectedPageName)
    {
        int index = text.IndexOfAny('\r', '\n');
        if (index < 0)
        {
            Logger.Warning("Expected multi-line atlas content");
            return false;
        }

        var pageName = RemoveOptionalSuffix(text[..index], ".png");
        if (!pageName.SequenceEqual(expectedPageName))
        {
            Logger.Warning($"Atlas page name did not match (Expected: '{expectedPageName}'. Actual: '{pageName}')");
            return false;
        }

        return true;

        static ReadOnlySpan<char> RemoveOptionalSuffix(ReadOnlySpan<char> value, ReadOnlySpan<char> suffix)
        {
            return value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)
                ? value[..^suffix.Length]
                : value;
        }
    }
}
