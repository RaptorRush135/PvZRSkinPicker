namespace PvZRSkinPicker.Skins.Prefabs.Plants;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.NativeUtils;
using PvZRSkinPicker.Skins.Prefabs;
using PvZRSkinPicker.Skins.Prefabs.Patches;
using PvZRSkinPicker.Skins.Prefabs.Serialization;

internal static class PlantPrefabResolver
{
    public static PrefabResolver<SeedType> Instance { get; } = new PrefabResolver<SeedType>();

    public static EmulateSkinConditionsPatcher<Plant, SeedType> SkinConditionsPatcher { get; }
        = new EmulateSkinConditionsPatcher<Plant, SeedType>(Instance, p => p.mSeedType);

    public static IFunctionHook Initialize()
    {
        return new ReloadedDeserializePatch<Plant, SeedType>(Instance).Initialize();
    }
}
