namespace PvZRSkinPicker.Skins;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

using Il2CppSource.Controllers;

using Il2CppSpine.Unity;

using PvZRSkinPicker.Environment;
using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.Unity;
using PvZRSkinPicker.Unity.Extensions;

using UnityEngine;

internal sealed class CustomSkinLoader(
    IDataService dataService)
{
    public IReadOnlyDictionary<SeedType, IEnumerable<Skin>> GetPlantSkins()
    {
        // TODO: Read from json
        // TODO: Return IReadOnlyCollection<Skin> as TValue?
        var definition = dataService.GetPlantDefinition(SeedType.Sunflower);

        var prefab = PrefabCloner.InstantiateInactiveFromPrefabAsset(definition.m_prefab);

        var controller = prefab.GetComponent<PlantController>();

        ReplaceDataAssets(
            controller.AnimationController.GetComponent<SkeletonAnimation>(),
            ModEnvironment.SkinsDirectory.GetFileIfExists("SunFlower.skel")?.ReadAllBytes());

        return new Dictionary<SeedType, IEnumerable<Skin>>()
        {
            [SeedType.Sunflower] = [new Skin(SkinType.Custom, prefab.ToAssetReference())],
        };
    }

    private static void ReplaceDataAssets(SkeletonAnimation animation, byte[]? skeletonData)
    {
        // TODO: Atlas
        // TODO: Texture
        ReplaceDataAssets(animation, GetSkeletonDataFile());

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

    private static void ReplaceDataAssets(SkeletonAnimation animation, TextAsset? skeletonData)
    {
        if (skeletonData == null)
        {
            return;
        }

        animation.skeletonDataAsset = SkeletonDataAsset.CreateRuntimeInstance(
            skeletonData,
            animation.skeletonDataAsset.atlasAssets,
            initialize: true,
            scale: animation.skeletonDataAsset.scale);

        animation.Initialize(overwrite: true);
    }
}
