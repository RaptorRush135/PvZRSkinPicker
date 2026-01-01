namespace PvZRSkinPicker.Api.Prefabs.Zombies;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Api.Prefabs.Patches;
using PvZRSkinPicker.Api.Prefabs.Serialization;
using PvZRSkinPicker.NativeUtils;

internal static class ZombiePrefabResolver
{
    public static PrefabResolver<ZombieType> Instance { get; } = new PrefabResolver<ZombieType>();

    public static EmulateSkinConditionsPatcher<Zombie, ZombieType> SkinConditionsPatcher { get; }
        = new EmulateSkinConditionsPatcher<Zombie, ZombieType>(Instance, z => z.mZombieType);

    public static IFunctionHook Initialize()
    {
        return new ReloadedDeserializePatch<Zombie, ZombieType>(Instance).Initialize();
    }
}
