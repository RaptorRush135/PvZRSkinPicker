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

    public Sprite RenderSrite(GameObject prefab, PacketRenderSpec<SeedType> renderSpec)
        => packetRenderer.RenderPlantToSprite(prefab, renderSpec);
}
