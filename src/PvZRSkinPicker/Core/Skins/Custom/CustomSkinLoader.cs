namespace PvZRSkinPicker.Skins.Custom;

using System.Diagnostics;

using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

using Il2CppSource.Controllers;

using Il2CppSpine.Unity;

using MelonLoader;

using PvZRSkinPicker.Almanac.Extensions;
using PvZRSkinPicker.Assets;
using PvZRSkinPicker.Environment;
using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.Skins.Custom.Manifest;
using PvZRSkinPicker.Unity;

using UnityEngine;
using UnityEngine.AddressableAssets;

internal sealed class CustomSkinLoader(
    MelonLogger.Instance logger,
    IDataService dataService)
{
    public IReadOnlyDictionary<SeedType, IReadOnlyList<Skin>> GetPlantSkins()
        => this.GetSkins<SeedType>(
            m => m.Skins.Plants,
            this.TryLoadPlantSkin);

    public IReadOnlyDictionary<ZombieType, IReadOnlyList<Skin>> GetZombieSkins()
        => this.GetSkins<ZombieType>(
            m => m.Skins.Zombies,
            this.TryLoadZombieSkin);

    private IReadOnlyDictionary<T, IReadOnlyList<Skin>> GetSkins<T>(
        Func<SkinPackManifest, IEnumerable<SkinEntry>> entriesSelector,
        Func<SkinEntry, DirectoryInfo, SkinPrototype<T>?> loadFunc)
        where T : struct, Enum
    {
        var stopwatch = Stopwatch.StartNew();

        logger.WriteSpacer();
        logger.Msg($"Reading {typeof(T).Name} skin manifests...");

        List<SkinPackManifestSource> sources = [.. ModEnvironment.SkinPacksDirectory
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
                logger.Warning($"Ignoring '{ignored}' since a new version exists");
                }

                return ordered[0];
            })];

        var skins = sources
            .SelectMany(source => entriesSelector(source.Manifest)
                .Select(skin => loadFunc(skin, source.Directory))
                .WhereNotNull())
            .GroupBy(
                s => s.Type,
                s => Skin.CreateCustom(s.Name, s.Id, s.Prefab))
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<Skin>)[.. g]);

        stopwatch.Stop();

        int totalSkins = skins.Values.Sum(list => list.Count);

        logger.WriteSpacer();
        logger.Msg($"Loaded {totalSkins} custom {typeof(T).Name} skins in {stopwatch.ElapsedMilliseconds} ms");
        logger.WriteSpacer();

        return skins;
    }

    private SkinPackManifestSource? TryGetManifest(DirectoryInfo directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        try
        {
            if (!DirectoryHasVersionSuffix(out int directoryVersion))
            {
                logger.Error($"Directory '{directory.FullName}' does not have a valid -V{{N}} suffix");
                return null;
            }

            var manifestFile = directory.GetFile("manifest.json");
            if (!manifestFile.Exists)
            {
                logger.Warning($"No file manifest at '{directory.FullName}'");
                return null;
            }

            try
            {
                using var fileStream = manifestFile.OpenRead();
                var manifest = SkinPackManifest.Load(fileStream, logger.Warning);

                if (!manifest.Validate(out string? error))
                {
                    logger.Error($"Manifest validation of '{manifestFile.FullName}' failed: {error}");
                    return null;
                }

                if (directoryVersion != manifest.Header.Version)
                {
                    logger.Error(
                        $"Directory version mismatch: directory has version {directoryVersion} but manifest " +
                        $"specifies version {manifest.Header.Version}");
                    return null;
                }

                return new(manifest, directory);
            }
            catch (Exception ex)
            {
                logger.Error($"Could not load file manifest '{manifestFile.FullName}'", ex);
                return null;
            }
        }
        catch (Exception ex)
        {
            logger.Error($"Could not load file manifest at '{directory.FullName}'", ex);
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
                logger.Warning($"Directory '{directoryName}' does not contain '{versionPattern}' suffix");
                return false;
            }

            string versionPart = directoryName[(suffixIndex + versionPattern.Length)..];
            if (versionPart.Length == 0)
            {
                logger.Warning($"Directory '{directoryName}' has no version number after '{versionPattern}'");
                return false;
            }

            if (!versionPart.All(ch => char.IsAscii(ch) && char.IsDigit(ch))
                || !int.TryParse(versionPart, out directoryVersion))
            {
                logger.Warning($"Directory '{directoryName}' has invalid version format: '{versionPart}'");
                return false;
            }

            return true;
        }
    }

    private SkinPrototype<SeedType>? TryLoadPlantSkin(SkinEntry skin, DirectoryInfo packDirectory)
    {
        return this.TryLoadSkin<SeedType, PlantController>(
            skin,
            packDirectory,
            dataService.GetPlantDefinition,
            d => d.m_prefab,
            controller => controller.AnimationController.GetComponent<SkeletonAnimation>());
    }

    private SkinPrototype<ZombieType>? TryLoadZombieSkin(SkinEntry skin, DirectoryInfo packDirectory)
    {
        return this.TryLoadSkin<ZombieType, ZombieController>(
            skin,
            packDirectory,
            dataService.GetZombieDefinition,
            d => d.m_prefab,
            controller => controller.GetComponentInChildren<SkeletonAnimation>());
    }

    private SkinPrototype<T>? TryLoadSkin<T, TController>(
        SkinEntry skin,
        DirectoryInfo packDirectory,
        Func<T, dynamic> definitionSelector,
        Func<dynamic, AssetReferenceGameObject> prefabSelector,
        Func<TController, SkeletonAnimation> animationSelector)
        where T : struct, Enum
        where TController : Component
    {
        logger.WriteLine();
        logger.Msg($"Processing {typeof(T).Name} skin '{skin}'");

        try
        {
            DirectoryInfo skinDirectory = packDirectory.GetDirectory(skin.Directory);
            if (!skinDirectory.Exists)
            {
                logger.Warning($"Skin directory not found: '{skinDirectory.FullName}'");
                LogFailure();
                return null;
            }

            if (!Enum.TryParse<T>(skin.Type, ignoreCase: true, out var targetType))
            {
                logger.Warning($"Could not parse skin type: '{skin.Type}'");
                LogFailure();
                return null;
            }

            if (targetType is SeedType seed && !seed.IsInAlmanac())
            {
                logger.Warning($"Seed type not in almanac: '{seed}'");
                LogFailure();
                return null;
            }

            var definition = definitionSelector(targetType);

            var prefab = PrefabCloner.InstantiateInactiveFromPrefabAsset(prefabSelector(definition), expectLoaded: true);

            try
            {
                var controller = prefab.GetComponent<TController>();
                var animation = animationSelector(controller);

                if (!this.TryReplaceAssets(skinDirectory, animation, usePointFilter: skin.Pixelated))
                {
                    Object.Destroy(prefab);

                    logger.Warning(
                        "Failed to replace skin assets in the prefab. " +
                        "Check Unity debug logs for more details");

                    LogFailure();

                    return null;
                }

                logger.Msg("Successfully processed skin");

                return new(targetType, skin.Name, skin.Id, prefab);
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
            const string message = "Failed to load skin";
            if (exception != null)
            {
                logger.Error(message, exception);
            }
            else
            {
                logger.Error(message);
            }
        }
    }

    private bool TryReplaceAssets(DirectoryInfo skinDirectory, SkeletonAnimation animation, bool usePointFilter)
    {
        var filterMode = usePointFilter ? FilterMode.Point : FilterMode.Bilinear;

        BytesAsset? textureData = skinDirectory.GetFileIfExists("skin.png")?.ReadBytesAsset();
        Texture2D? texture = textureData != null
            ? ModAssets.LoadTexture(textureData, filterMode)
            : null;

        try
        {
            var atlas = skinDirectory.GetFileIfExists("skin.atlas")?.ReadAllText();

            var skeleton = skinDirectory.GetFileIfExists("skin.skel")?.ReadAllBytes();

            logger.Msg(
                "Assets: " +
                $"texture={PresenceMark(texture)} " +
                $"atlas={PresenceMark(atlas)} " +
                $"skeleton={PresenceMark(skeleton)}");

            bool replaced = CustomSkinAssetReplacer.TryReplace(animation, texture, atlas, skeleton);
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

    private sealed record SkinPackManifestSource(
        SkinPackManifest Manifest,
        DirectoryInfo Directory);

    private sealed record SkinPrototype<T>(
        T Type,
        string Name,
        Guid Id,
        GameObject Prefab)
        where T : struct, Enum;
}
