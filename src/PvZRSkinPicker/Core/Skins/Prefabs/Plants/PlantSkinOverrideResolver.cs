namespace PvZRSkinPicker.Skins.Prefabs.Plants;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Almanac.SeedPackets;
using PvZRSkinPicker.Hooks;
using PvZRSkinPicker.Skins.Prefabs;
using PvZRSkinPicker.Skins.Prefabs.Serialization;

internal sealed class PlantSkinOverrideResolver : SkinOverrideResolver<SeedType>
{
    public static PlantSkinOverrideResolver Instance { get; } = new();

    public static EmulateSkinConditionsPatcher<Plant, SeedType> SkinConditionsPatcher { get; }
        = new EmulateSkinConditionsPatcher<Plant, SeedType>(Instance, p => new(p.mSeedType, p.mBoard, p.mRow));

    protected override PacketThumbnailLookup<SeedType> PacketThumbnailLookup => PlantPacketThumbnailLookup.Instance;

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
