namespace PvZRSkinPicker.Skins.Prefabs.Zombies;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.NativeUtils;
using PvZRSkinPicker.Skins.Prefabs;
using PvZRSkinPicker.Skins.Prefabs.Serialization;

internal sealed class ZombieSkinOverrideResolver : SkinOverrideResolver<ZombieType>
{
    public static ZombieSkinOverrideResolver Instance { get; } = new();

    public static EmulateSkinConditionsPatcher<Zombie, ZombieType> SkinConditionsPatcher { get; }
        = new EmulateSkinConditionsPatcher<Zombie, ZombieType>(Instance, z => new(z.mZombieType, z.mBoard, z.mRow));

    public static IFunctionHook Initialize()
    {
        return new ReloadedDeserializePatch<Zombie, ZombieType>(Instance).Initialize();
    }

    protected override bool IsSkinCompatible(SkinType skinType, SpawnContext<ZombieType> context)
    {
        return !(skinType == SkinType.China
            && context.IsAtRowType(PlantRowType.Pool)
            && context.Type != ZombieType.Bungee);
    }
}
