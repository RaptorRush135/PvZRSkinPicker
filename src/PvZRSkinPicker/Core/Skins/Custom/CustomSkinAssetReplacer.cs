namespace PvZRSkinPicker.Skins.Custom;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

using Il2CppSpine.Unity;

using Microsoft.Extensions.Logging;

using SolarApi.Unity.Extensions;

using UnityEngine;

using ILogger = Microsoft.Extensions.Logging.ILogger;

internal sealed class CustomSkinAssetReplacer(
    ILogger logger)
{
    public bool TryReplace(
        SkeletonAnimation animation,
        Texture2D? texture,
        string? atlasText,
        byte[]? skeletonData,
        string? initialSkinName)
    {
        ArgumentNullException.ThrowIfNull(animation);

        return this.TryReplace(animation, texture, ConvertAtlasTextFile(), ConvertSkeletonDataFile(), initialSkinName);

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

    private bool TryReplace(
        SkeletonAnimation animation,
        Texture2D? texture,
        TextAsset? atlasText,
        TextAsset? skeletonData,
        string? initialSkinName)
    {
        if (texture == null && atlasText == null && skeletonData == null)
        {
            logger.LogWarning("No replacement data provided");
            return false;
        }

        if (!this.TryReplaceAtlas(animation, texture, atlasText, out var atlas))
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
            logger.LogWarning(
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
            logger.LogWarning("Animation initialization failed");
            return false;
        }

        if (animation.meshRenderer.sharedMaterial == null)
        {
            logger.LogWarning("No material in the mesh renderer. " +
                "This is likely caused by not having visible attachments in the default pose");

            return false;
        }

        return true;
    }

    private bool TryReplaceAtlas(
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

            atlas = this.CreateAtlas(
                currentAtlas.Cast<SpineAtlasAsset>().atlasFile, texture, currentAtlas.PrimaryMaterial);
        }
        else
        {
            if (!this.AtlasPageNameMatches(atlasText.text, currentTexture.name))
            {
                atlas = currentAtlas;
                return false;
            }

            var atlasTexture = texture.Ref() ?? currentTexture;
            atlas = this.CreateAtlas(atlasText, atlasTexture, currentAtlas.PrimaryMaterial);
        }

        if (atlas.GetAtlas() == null)
        {
            logger.LogWarning("Failed to initialize atlas");
            return false;
        }

        return true;
    }

    private SpineAtlasAsset CreateAtlas(
        TextAsset atlasText,
        Texture2D texture,
        Material material)
    {
        ArgumentNullException.ThrowIfNull(atlasText);
        ArgumentNullException.ThrowIfNull(texture);
        ArgumentNullException.ThrowIfNull(material);

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                "Creating Spine atlas asset from atlas '{AtlasName}' and texture '{TextureName}'",
                atlasText.name,
                texture.name);
        }

        var newMaterial = new Material(material)
        {
            mainTexture = texture,
        };

        return SpineAtlasAsset.CreateRuntimeInstance(
            atlasText,
            new([newMaterial]),
            initialize: false);
    }

    private bool AtlasPageNameMatches(ReadOnlySpan<char> text, ReadOnlySpan<char> expectedPageName)
    {
        int index = text.IndexOfAny('\r', '\n');
        if (index < 0)
        {
            logger.LogWarning("Expected multi-line atlas content");
            return false;
        }

        var pageName = RemoveOptionalSuffix(text[..index], ".png");
        if (!pageName.SequenceEqual(expectedPageName))
        {
            logger.LogWarning(
                "Atlas page name did not match (Expected: '{ExpectedPageName}'. Actual: '{ActualPageName}')",
                expectedPageName.ToString(),
                pageName.ToString());

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
