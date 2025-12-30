namespace PvZRSkinPicker.Data;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

using PvZRSkinPicker.Skins;

internal sealed class ZombieSkinDataDefinition(
    ZombieDefinition definition,
    IPlatformService platformService)
    : ISkinDataDefinition<ZombieType>
{
    public ZombieType Type => definition.ZombieType;

    public IEnumerable<Skin> GetSkins()
        => SkinLocator.GetSkins(definition, platformService);
}
