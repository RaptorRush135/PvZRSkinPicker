namespace PvZRSkinPicker.Skins.Custom;

using System.Collections.Immutable;
using System.Diagnostics;

using Il2CppReloaded.Gameplay;

using Microsoft.Extensions.Logging;

using PvZRSkinPicker.Environment;
using PvZRSkinPicker.Skins.Custom.Manifest;

using SolarApi.Collections.Extensions;
using SolarApi.IO.Extensions;
using SolarApi.Logging.Extensions;
using SolarApi.Unity.Resources;

internal sealed class CustomSkinLoader(
    ILogger<CustomSkinLoader> logger,
    SkinPickerModEnvironment environment,
    SkinLoaderFactory skinLoaderFactory,
    AddressableAssetRegistry assetRegistry)
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

        var plantSkinLoader = skinLoaderFactory.CreateForPlants(logger);

        var skins = sources
            .SelectMany(s => this.LoadManifestSkins(s, plantSkinLoader))
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

    private IReadOnlyCollection<SkinPrototype<SeedType>> LoadManifestSkins(
        SkinPackManifestSource manifestSource,
        SkinLoader<SeedType> plantSkinLoader)
    {
        ArgumentNullException.ThrowIfNull(manifestSource);

        var header = manifestSource.Manifest.Header;

        logger.LogSpacer();
        logger.LogInformation("Processing skin pack '{Header}' by {Authors}", header, header.FormattedAuthors);

        return [.. manifestSource.Manifest.Skins.Plants
            .Select(skin => plantSkinLoader.TryLoadSkin(skin, manifestSource.Directory))
            .WhereNotNull(),];
    }

    private sealed record SkinPackManifestSource(
        SkinPackManifest Manifest,
        DirectoryInfo Directory);
}
