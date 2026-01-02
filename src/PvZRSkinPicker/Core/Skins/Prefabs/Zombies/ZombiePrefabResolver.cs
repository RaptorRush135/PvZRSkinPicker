namespace PvZRSkinPicker.Skins.Prefabs.Zombies;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.NativeUtils;
using PvZRSkinPicker.Skins.Prefabs;
using PvZRSkinPicker.Skins.Prefabs.Patches;
using PvZRSkinPicker.Skins.Prefabs.Serialization;

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
