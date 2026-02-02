namespace PvZRSkinPicker.Skins.Custom;

using System.Diagnostics;

using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

using Il2CppSource.Controllers;

using Il2CppSpine.Unity;

using MelonLoader;

using PvZRSkinPicker.Assets;
using PvZRSkinPicker.Environment;
using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.Skins.Custom.Manifest;
using PvZRSkinPicker.Unity;
using PvZRSkinPicker.Unity.Extensions;

using UnityEngine;

internal sealed class CustomSkinLoader(
    MelonLogger.Instance logger,
    IDataService dataService)
{
    public IReadOnlyDictionary<SeedType, IReadOnlyList<Skin>> GetPlantSkins()
    {
        var stopwatch = Stopwatch.StartNew();

        List<SkinPackManifestSource> sources = [.. ModEnvironment.SkinPacksDirectory
            .GetDirectories()
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
            .SelectMany(this.LoadManifestSkins)
            .GroupBy(
                s => s.Type,
                s => new Skin(SkinType.Custom, s.Prefab.ToAssetReference()))
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<Skin>)[.. g]);

        stopwatch.Stop();

        int totalSkins = skins.Values.Sum(list => list.Count);

        logger.Msg($"Loaded {totalSkins} custom skins in {stopwatch.ElapsedMilliseconds} ms.");

        return skins;
    }

    private SkinPackManifestSource? TryGetManifest(DirectoryInfo directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        try
        {
            var manifestFile = directory.GetFile("manifest.json");
            if (!manifestFile.Exists)
            {
                logger.Warning($"No file manifest at '{directory.FullName}'");
                return null;
            }

            try
            {
                using var fileStream = manifestFile.OpenRead();
                var manifest = SkinPackManifest.Load(fileStream);

                if (!manifest.Validate(out string? error))
                {
                    logger.Error($"Manifest validation of '{manifestFile.FullName}' failed: {error}");
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
    }

    private IEnumerable<SkinPrototype<SeedType>> LoadManifestSkins(SkinPackManifestSource manifestSource)
    {
        ArgumentNullException.ThrowIfNull(manifestSource);

        return [.. manifestSource.Manifest.Skins.Plants
            .Select(s => this.TryLoadSkin(manifestSource.Directory, s))
            .WhereNotNull()];
    }

    private SkinPrototype<SeedType>? TryLoadSkin(DirectoryInfo packDirectory, SkinEntry skin)
    {
        logger.Msg($"Processing '{skin}'");

        try
        {
            DirectoryInfo skinDirectory = packDirectory.GetDirectory(skin.Directory);
            if (!skinDirectory.Exists)
            {
                logger.Warning($"Directory of '{skin}' not found: '{skinDirectory.FullName}'");
                return null;
            }

            if (!Enum.TryParse<SeedType>(skin.Type, ignoreCase: true, out var targetType))
            {
                logger.Warning($"Could not parse type of '{skin}': '{skin.Type}'");
                return null;
            }

            var definition = dataService.GetPlantDefinition(targetType);

            // TODO: Destroy prefab on fail
            var prefab = PrefabCloner.InstantiateInactiveFromPrefabAsset(definition.m_prefab, expectLoaded: true);

            var controller = prefab.GetComponent<PlantController>();

            // TODO: Cache texture by path?
            BytesAsset? textureData = skinDirectory.GetFileIfExists("skin.png")?.ReadBytesAsset();
            Texture2D? texture = textureData != null
                ? ModAssets.LoadTexture(textureData)
                : null;

            // TODO: Allow custom path/name?
            bool replaceSucceeded = CustomSkinAssetReplacer.TryReplace(
                controller.AnimationController.GetComponent<SkeletonAnimation>(),
                texture,
                skinDirectory.GetFileIfExists("skin.atlas")?.ReadAllText(),
                skinDirectory.GetFileIfExists("skin.skel")?.ReadAllBytes());

            if (!replaceSucceeded)
            {
                logger.Warning(
                    $"Failed to replace assets of '{skin}' in the prefab. " +
                    "Check Unity debug logs for more details");

                return null;
            }

            logger.Msg("Successfully processed skin");

            return new(targetType, prefab);
        }
        catch (Exception ex)
        {
            logger.Error($"Could not load skin '{skin}'", ex);
            return null;
        }
    }

    private sealed record SkinPackManifestSource(
        SkinPackManifest Manifest,
        DirectoryInfo Directory);

    private sealed record SkinPrototype<T>(
        T Type,
        GameObject Prefab)
        where T : struct, Enum;
}
