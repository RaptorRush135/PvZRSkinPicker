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
        ArgumentNullException.ThrowIfNull(animation);

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

    private static bool TryReplace(
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

        string initialSkinName = animation.initialSkinName;
        animation.skeletonDataAsset = newSkeleton;
        animation.initialSkinName = initialSkinName;
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
        var currentAtlasAssets = animation.SkeletonDataAsset.atlasAssets;
        var currentAtlas = currentAtlasAssets[0];
        var currentTexture = currentAtlas.PrimaryMaterial.mainTexture.Cast<Texture2D>();

        if (texture != null)
        {
            texture.name = currentTexture.name;
        }

        if (atlasText == null)
        {
            if (texture == null)
            {
                atlas = currentAtlas;
                return true;
            }

            atlas = CreateAtlas(currentAtlas.Cast<SpineAtlasAsset>().atlasFile, texture, currentAtlas);
        }
        else
        {
            if (!AtlasPageNameMatches(atlasText.text, currentTexture.name))
            {
                atlas = currentAtlas;
                return false;
            }

            var atlasTexture = texture.Ref() ?? currentTexture;
            atlas = CreateAtlas(atlasText, atlasTexture, currentAtlas);
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
        AtlasAssetBase originalAtlas)
    {
        ArgumentNullException.ThrowIfNull(atlasText);
        ArgumentNullException.ThrowIfNull(texture);
        ArgumentNullException.ThrowIfNull(originalAtlas);

        var materials = new List<Material>();
        for (int i = 0; i < originalAtlas.MaterialCount; i++)
        {
            var mat = new Material(originalAtlas.PrimaryMaterial)
            {
                mainTexture = texture,
            };
            materials.Add(mat);
        }

        var materialsArray = new Il2CppReferenceArray<Material>(materials.ToArray());

        return SpineAtlasAsset.CreateRuntimeInstance(
            atlasText,
            materialsArray,
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
