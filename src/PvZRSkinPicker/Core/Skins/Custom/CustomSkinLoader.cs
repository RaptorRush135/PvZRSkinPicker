namespace PvZRSkinPicker.Skins.Custom;

using System.Collections.Immutable;
using System.Diagnostics;

using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

using Il2CppSource.Controllers;

using Il2CppSpine.Unity;

using Microsoft.Extensions.Logging;

using PvZRSkinPicker.Almanac.Extensions;
using PvZRSkinPicker.Almanac.SeedPackets.Renderer;
using PvZRSkinPicker.Assets;
using PvZRSkinPicker.Environment;
using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.Skins.Custom.Manifest;

using SolarApi.Collections.Extensions;
using SolarApi.IO.Extensions;
using SolarApi.Logging.Extensions;
using SolarApi.Unity;
using SolarApi.Unity.Resources;

using UnityEngine;

internal sealed class CustomSkinLoader(
    ILogger<CustomSkinLoader> logger,
    IDataService dataService,
    SkinPickerModEnvironment environment,
    AddressableAssetRegistry assetRegistry,
    PacketRenderer packetRenderer)
{
    public CustomSkinSet GetSkins()
    {
        var stopwatch = Stopwatch.StartNew();

        logger.LogSpacer();
        logger.LogInformation("Reading skin manifests...");

        List<SkinPackManifestSource> sources = [.. environment.SkinPacksDirectory
            .GetDirectories()
            .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
            .Select(this.TryGetManifest)
            .WhereNotNull()
            .GroupBy(s => s.Manifest.Header.Id)
            .Select(group =>
            {
                var ordered = group
                    .OrderByDescending(s => s.Manifest.Header.Version)
                    .ToList();

                foreach (var ignored in ordered.Skip(1))
                {
                    logger.LogWarning("Ignoring '{IgnoredSource}' since a new version exists", ignored);
                }

                return ordered[0];
            }),];

        var skins = sources
            .SelectMany(this.LoadManifestSkins)
            .GroupBy(
                s => s.Type,
                s => s.Build(assetRegistry))
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<Skin>)[.. g]);

        stopwatch.Stop();

        int totalSkins = skins.Values.Sum(list => list.Count);

        logger.LogSpacer();

        logger.LogInformation(
            "Loaded {TotalSkins} custom skins in {ElapsedMilliseconds} ms",
            totalSkins,
            stopwatch.ElapsedMilliseconds);

        logger.LogSpacer();

        return new(skins, ImmutableDictionary<ZombieType, IReadOnlyList<Skin>>.Empty);
    }

    private SkinPackManifestSource? TryGetManifest(DirectoryInfo directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        try
        {
            if (!DirectoryHasVersionSuffix(out int directoryVersion))
            {
                logger.LogError(
                    "Directory '{DirectoryName}' does not have a valid -V{{N}} suffix",
                    directory.FullName);

                return null;
            }

            var manifestFile = directory.GetFile("manifest.json");
            if (!manifestFile.Exists)
            {
                logger.LogWarning("No file manifest at '{DirectoryName}'", directory.FullName);

                return null;
            }

            try
            {
                using var fileStream = manifestFile.OpenRead();
                var manifest = SkinPackManifest.Load(fileStream, logger);

                if (!manifest.Validate(out string? error))
                {
                    logger.LogError(
                        "Manifest validation of '{ManifestFileName}' failed: {Error}",
                        manifestFile.FullName,
                        error);

                    return null;
                }

                if (directoryVersion != manifest.Header.Version)
                {
                    logger.LogError(
                        "Directory version mismatch: directory has version {DirectoryVersion} but manifest " +
                        "specifies version {HeaderVersion}",
                        directoryVersion,
                        manifest.Header.Version);

                    return null;
                }

                return new(manifest, directory);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not load file manifest '{ManifestFileName}'", manifestFile.FullName);
                return null;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not load file manifest at '{DirectoryName}'", directory.FullName);
            return null;
        }

        bool DirectoryHasVersionSuffix(out int directoryVersion)
        {
            const string versionPattern = "-V";
            string directoryName = directory.Name;

            directoryVersion = 0;
            int suffixIndex = directoryName.LastIndexOf(versionPattern, StringComparison.Ordinal);
            if (suffixIndex < 0)
            {
                logger.LogWarning(
                    "Directory '{DirectoryName}' does not contain '{VersionPattern}' suffix",
                    directoryName,
                    versionPattern);

                return false;
            }

            string versionPart = directoryName[(suffixIndex + versionPattern.Length)..];
            if (versionPart.Length == 0)
            {
                logger.LogWarning(
                    "Directory '{DirectoryName}' has no version number after '{VersionPattern}'",
                    directoryName,
                    versionPattern);

                return false;
            }

            if (!versionPart.All(ch => char.IsAscii(ch) && char.IsDigit(ch))
                || !int.TryParse(versionPart, out directoryVersion))
            {
                logger.LogWarning(
                    "Directory '{DirectoryName}' has invalid version format: '{VersionPart}'",
                    directoryName,
                    versionPart);

                return false;
            }

            return true;
        }
    }

    private IReadOnlyCollection<SkinPrototype<SeedType>> LoadManifestSkins(SkinPackManifestSource manifestSource)
    {
        ArgumentNullException.ThrowIfNull(manifestSource);

        var header = manifestSource.Manifest.Header;

        logger.LogSpacer();
        logger.LogInformation("Processing skin pack '{Header}' by {Authors}", header, header.FormattedAuthors);

        return [.. manifestSource.Manifest.Skins.Plants
            .Select(skin => this.TryLoadSkin(skin, manifestSource.Directory))
            .WhereNotNull(),];
    }

    private SkinPrototype<SeedType>? TryLoadSkin(SkinEntry skin, DirectoryInfo packDirectory)
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

            if (!Enum.TryParse<SeedType>(skin.Type, ignoreCase: true, out var targetType)
                || !targetType.IsInAlmanac())
            {
                logger.LogWarning("Could not parse skin type: '{SkinType}'", skin.Type);
                LogFailure();
                return null;
            }

            var definition = dataService.GetPlantDefinition(targetType);

            var prefab = AssetPrefabCloner.Clone(definition.m_prefab, expectLoaded: true);

            try
            {
                var controller = prefab.GetComponent<PlantController>();
                if (!this.TryLoadSkin(skinDirectory, controller, targetType, usePointFilter: skin.Pixelated))
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
        PlantController controller,
        SeedType type,
        bool usePointFilter)
    {
        var animation = controller.AnimationController.GetComponent<SkeletonAnimation>();

        string? initialSkinName = GetInitialSkinName(animation, type);

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

            bool replaced = CustomSkinAssetReplacer.TryReplace(
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

        static string? GetInitialSkinName(
            SkeletonAnimation animation,
            SeedType type)
        {
            // Initial skin is Repeater in vanilla
            if (type == SeedType.Peashooter)
            {
                return nameof(SeedType.Peashooter);
            }

            return animation.initialSkinName;
        }
    }

    private Sprite RenderSrite(GameObject prefab, SeedType type, SeedPacketOverride? seedPacketOverride)
    {
        var transform = this.GetDefaultSeedPacketTypeTransform(type)
            .Apply(seedPacketOverride?.Transform);

        var renderSpec = new PacketRenderSpec<SeedType>(type, transform);

        return packetRenderer.RenderPlantToSprite(prefab, renderSpec);
    }

    private SkinTransform GetDefaultSeedPacketTypeTransform(SeedType seedType)
    {
        return seedType switch
        {
            SeedType.Peashooter => new(0.2f, -1.55f),
            SeedType.Sunflower => new(0.5f, -1.7f, 0.95f),
            SeedType.Cherrybomb => new(-0.15f, -1.45f, 0.75f),
            SeedType.Wallnut => new(-0.09f, -2.12f, 0.9f),
            SeedType.Potatomine => new(.17f, -1.06f, 0.75f),
            SeedType.Snowpea => new(0.27f, -1.38f, 0.95f),
            SeedType.Chomper => new(-0.15f, -1.92f, 0.7f),
            SeedType.Repeater => new(0.26f, -1.54f),

            // TODO: Complete
            // SeedType.Puffshroom = 8,
            // SeedType.Sunshroom = 9,
            // SeedType.Fumeshroom = 10,
            // SeedType.Gravebuster = 11,
            // SeedType.Hypnoshroom = 12,
            // SeedType.Scaredyshroom = 13,
            // SeedType.Iceshroom = 14,
            // SeedType.Doomshroom = 15,
            // SeedType.Lilypad = 16,
            // SeedType.Squash = 17,
            // SeedType.Threepeater = 18,
            // SeedType.Tanglekelp = 19,
            // SeedType.Jalapeno = 20,
            // SeedType.Spikeweed = 21,
            // SeedType.Torchwood = 22,
            // SeedType.Tallnut = 23,
            // SeedType.Seashroom = 24,
            // SeedType.Plantern = 25,
            // SeedType.Cactus = 26,
            // SeedType.Blover = 27,
            // SeedType.Splitpea = 28,
            // SeedType.Starfruit = 29,
            // SeedType.Pumpkinshell = 30,
            // SeedType.Magnetshroom = 31,
            // SeedType.Cabbagepult = 32,
            // SeedType.Flowerpot = 33,
            // SeedType.Kernelpult = 34,
            // SeedType.InstantCoffee = 35,
            // SeedType.Garlic = 36,
            // SeedType.Umbrella = 37,
            // SeedType.Marigold = 38,
            // SeedType.Melonpult = 39,
            // SeedType.Gatlingpea = 40,
            // SeedType.Twinsunflower = 41,
            // SeedType.Gloomshroom = 42,
            // SeedType.Cattail = 43,
            // SeedType.Wintermelon = 44,
            // SeedType.GoldMagnet = 45,
            // SeedType.Spikerock = 46,
            // SeedType.Cobcannon = 47,
            // SeedType.Imitater = 48,
            _ => new(.5f, -1.75f), // TODO: throw
        };
    }

    private sealed record SkinPackManifestSource(
        SkinPackManifest Manifest,
        DirectoryInfo Directory);

    private sealed record SkinPrototype<T>(
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
}
