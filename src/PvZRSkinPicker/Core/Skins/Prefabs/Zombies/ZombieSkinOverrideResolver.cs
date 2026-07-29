namespace PvZRSkinPicker.Skins.Prefabs.Zombies;

using Il2CppReloaded.Gameplay;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using PvZRSkinPicker.Almanac.SeedPackets;
using PvZRSkinPicker.Skins.Prefabs;
using PvZRSkinPicker.Skins.Prefabs.Serialization;

using SolarApi;
using SolarApi.Hooks;

internal sealed class ZombieSkinOverrideResolver(
    ILogger<ZombieSkinOverrideResolver> logger,
    SpawnContextContainer<ZombieType> currentContext)
    : SkinOverrideResolver<ZombieType>(logger, currentContext)
{
    public static ZombieSkinOverrideResolver Instance { get; }
        = Solar<SkinPickerMod>.Provider.GetRequiredService<ZombieSkinOverrideResolver>();

    public static EmulateSkinConditionsPatcher<Zombie, ZombieType> SkinConditionsPatcher { get; }
        = new EmulateSkinConditionsPatcher<Zombie, ZombieType>(Instance, z => new(z.mZombieType, z.mBoard, z.mRow));

    protected override PacketThumbnailLookup<ZombieType>? PacketThumbnailLookup => null;

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
