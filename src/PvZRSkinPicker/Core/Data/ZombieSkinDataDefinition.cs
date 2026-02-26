namespace PvZRSkinPicker.Data;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Skins;

internal sealed class ZombieSkinDataDefinition(
    ZombieDefinition definition,
    SkinLocator skinLocator)
    : ISkinDataDefinition<ZombieType>
{
    public ZombieType Type => definition.ZombieType;

    public IEnumerable<Skin> GetSkins() => skinLocator.GetSkins(definition);
}
