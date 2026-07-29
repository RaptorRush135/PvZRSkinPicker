namespace PvZRSkinPicker.Skins.Custom;

using Il2CppSpine.Unity;

using Microsoft.Extensions.Logging;

using PvZRSkinPicker.Almanac.SeedPackets.Renderer;
using PvZRSkinPicker.Assets;
using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.Skins.Custom.Manifest;

using SolarApi.IO.Extensions;
using SolarApi.Logging.Extensions;
using SolarApi.Unity;

using UnityEngine;

using ILogger = Microsoft.Extensions.Logging.ILogger;

internal sealed class SkinLoader<T>(
    ILogger logger,
    ISkinTypeHandler<T> skinTypeHandler)
    where T : struct, Enum
{
    private readonly CustomSkinAssetReplacer assetReplacer = new(logger);

    public SkinPrototype<T>? TryLoadSkin(
        SkinEntry skin,
        DirectoryInfo packDirectory)
    {
        logger.LogLine();
        logger.LogInformation("Processing skin '{Skin}'", skin);

        try
        {
            DirectoryInfo skinDirectory = packDirectory.GetDirectory(skin.Directory);
            if (!skinDirectory.Exists)
            {
                logger.LogWarning("Skin directory not found: '{SkinDirectory}'", skinDirectory.FullName);
                LogFailure();
                return null;
            }

            if (!Enum.TryParse<T>(skin.Type, ignoreCase: true, out var targetType)
                || !skinTypeHandler.IsSkinPickerSupported(targetType))
            {
                logger.LogWarning("Could not parse skin type: '{SkinType}'", skin.Type);
                LogFailure();
                return null;
            }

            var prefab = AssetPrefabCloner.Clone(skinTypeHandler.GetPrefab(targetType), expectLoaded: true);

            try
            {
                var animation = skinTypeHandler.GetSkeletonAnimation(prefab);

                if (!this.TryLoadSkin(
                    skinDirectory, animation, targetType, usePointFilter: skin.Pixelated))
                {
                    Object.Destroy(prefab);

                    logger.LogWarning(
                        "Failed to replace skin assets in the prefab. " +
                        "Check Unity debug logs for more details");

                    LogFailure();

                    return null;
                }

                var sprite = this.RenderSrite(prefab, targetType, skin.SeedPacketOverride);

                logger.LogInformation("Successfully processed skin");

                return new(targetType, skin.Name, skin.Id, prefab, sprite);
            }
            catch (Exception)
            {
                Object.Destroy(prefab);
                throw;
            }
        }
        catch (Exception ex)
        {
            LogFailure(ex);
            return null;
        }

        void LogFailure(Exception? exception = null)
        {
            logger.LogError(exception, "Failed to load skin");
        }
    }

    private bool TryLoadSkin(
        DirectoryInfo skinDirectory,
        SkeletonAnimation animation,
        T type,
        bool usePointFilter)
    {
        string? initialSkinName = skinTypeHandler.GetInitialSkinName(animation, type);

        var filterMode = usePointFilter ? FilterMode.Point : FilterMode.Bilinear;

        BytesAsset? textureData = skinDirectory.GetFileIfExists("skin.png")?.ReadBytesAsset();
        Texture2D? texture = textureData != null
            ? ModAssets.LoadTexture(textureData, filterMode)
            : null;

        try
        {
            var atlas = skinDirectory.GetFileIfExists("skin.atlas")?.ReadAllText();

            var skeleton = skinDirectory.GetFileIfExists("skin.skel")?.ReadAllBytes();

            logger.LogInformation(
                "Assets: texture={TextureMark} atlas={AtlasMark} skeleton={SkeletonMark}",
                PresenceMark(texture),
                PresenceMark(atlas),
                PresenceMark(skeleton));

            bool replaced = this.assetReplacer.TryReplace(
                animation,
                texture,
                atlas,
                skeleton,
                initialSkinName);

            if (!replaced)
            {
                Object.Destroy(texture);
            }

            return replaced;
        }
        catch (Exception)
        {
            Object.Destroy(texture);
            throw;
        }

        static string PresenceMark(object? value)
            => value != null ? "[x]" : "[ ]";
    }

    private Sprite RenderSrite(GameObject prefab, T type, SeedPacketOverride? seedPacketOverride)
    {
        var transform = skinTypeHandler.GetDefaultSeedPacketTransform(type)
            .Apply(seedPacketOverride?.Transform);

        var renderSpec = new PacketRenderSpec<T>(type, transform);

        return skinTypeHandler.RenderSrite(prefab, renderSpec);
    }
}
