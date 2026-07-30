namespace PvZRSkinPicker.Skins.Custom;

using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

using Il2CppSource.Controllers;

using Il2CppSpine.Unity;

using PvZRSkinPicker.Almanac.SeedPackets.Renderer;
using PvZRSkinPicker.Skins.Custom.Manifest;
using PvZRSkinPicker.Skins.Picker;

using UnityEngine;
using UnityEngine.AddressableAssets;

internal sealed class PlantSkinHandler(
    IDataService dataService,
    PacketRenderer packetRenderer)
    : ISkinTypeHandler<SeedType>
{
    public bool IsSkinPickerSupported(SeedType type)
        => type.IsSkinPickerSupported();

    public AssetReferenceGameObject GetPrefab(SeedType type)
        => dataService.GetPlantDefinition(type).m_prefab;

    public SkeletonAnimation GetSkeletonAnimation(GameObject @object)
    {
        var controller = @object.GetComponent<PlantController>();
        return controller.AnimationController.GetComponent<SkeletonAnimation>();
    }

    public string? GetInitialSkinName(SkeletonAnimation animation, SeedType type)
    {
        // Initial skin is Repeater in vanilla
        if (type == SeedType.Peashooter)
        {
            return nameof(SeedType.Peashooter);
        }

        return animation.initialSkinName;
    }

    public SkinTransform GetDefaultSeedPacketTransform(SeedType type)
    {
        return type switch
        {
            SeedType.Peashooter => new(0.2f, -1.55f),
            SeedType.Sunflower => new(0.5f, -1.7f, 0.95f),
            SeedType.Cherrybomb => new(-0.15f, -1.45f, 0.75f),
            SeedType.Wallnut => new(-0.09f, -2.12f, 0.9f),
            SeedType.Potatomine => new(.17f, -1.06f, 0.75f),
            SeedType.Snowpea => new(0.27f, -1.38f, 0.95f),
            SeedType.Chomper => new(-0.15f, -1.92f, 0.7f),
            SeedType.Repeater => new(0.26f, -1.54f),
            SeedType.Puffshroom => new(0.025f, -0.78f),
            SeedType.Sunshroom => new(0f, -1.9f, 1.6f),
            SeedType.Fumeshroom => new(-0.25f, -1.88f, 0.7f),
            SeedType.Gravebuster => new(0.17f, -1.45f, 0.7f),
            SeedType.Hypnoshroom => new(0, -2.1f, 0.85f),
            SeedType.Scaredyshroom => new(0.52f, -2.28f),
            SeedType.Iceshroom => new(0.05f, -1.8f, 0.7f),
            SeedType.Doomshroom => new(-0.1f, -1.95f, 0.72f),
            SeedType.Lilypad => new(0.025f, -1.68f, 0.75f),
            SeedType.Squash => new(0.11f, -2.25f, 0.82f),
            SeedType.Threepeater => new(0.025f, -1.88f, 0.85f),
            SeedType.Tanglekelp => new(0.23f, -1.3f, 0.7f),
            SeedType.Jalapeno => new(0.12f, -2.12f, 0.82f),
            SeedType.Spikeweed => new(0.015f, -0.92f, 0.74f),
            SeedType.Torchwood => new(-0.04f, -2.36f, 0.78f),
            SeedType.Tallnut => new(0, -2.6f, 0.6f),
            SeedType.Seashroom => new(0.02f, -0.85f, 1.05f),
            SeedType.Plantern => new(0.1f, -1.86f, 0.7f),

            // TODO: Complete
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

    public Sprite RenderSrite(GameObject prefab, PacketRenderSpec<SeedType> renderSpec)
        => packetRenderer.RenderPlantToSprite(prefab, renderSpec);
}
