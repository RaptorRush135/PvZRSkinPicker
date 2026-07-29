namespace PvZRSkinPicker.Skins.Prefabs.Plants;

using Il2CppReloaded.Gameplay;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using PvZRSkinPicker.Almanac.SeedPackets;
using PvZRSkinPicker.Skins.Prefabs;
using PvZRSkinPicker.Skins.Prefabs.Serialization;

using SolarApi;
using SolarApi.Hooks;

internal sealed class PlantSkinOverrideResolver(
    ILogger<PlantSkinOverrideResolver> logger,
    SpawnContextContainer<SeedType> currentContext,
    PacketThumbnailLookup<SeedType> packetThumbnailLookup)
    : SkinOverrideResolver<SeedType>(logger, currentContext)
{
    public static PlantSkinOverrideResolver Instance { get; }
        = Solar<SkinPickerMod>.Provider.GetRequiredService<PlantSkinOverrideResolver>();

    public static EmulateSkinConditionsPatcher<Plant, SeedType> SkinConditionsPatcher { get; }
        = new EmulateSkinConditionsPatcher<Plant, SeedType>(Instance, p => new(p.mSeedType, p.mBoard, p.mRow));

    protected override PacketThumbnailLookup<SeedType> PacketThumbnailLookup => packetThumbnailLookup;

    public static IFunctionHook Initialize()
    {
        return new ReloadedDeserializePatch<Plant, SeedType>(Instance).Initialize();
    }

    public override ReadOnlySpan<SeedType> GetExtraTypeOverrides(SeedType type)
    {
        return type switch
        {
            SeedType.Wallnut => [SeedType.ExplodeONut, SeedType.GiantWallnut],
            SeedType.Repeater => [SeedType.Leftpeater],
            _ => [],
        };
    }
}
