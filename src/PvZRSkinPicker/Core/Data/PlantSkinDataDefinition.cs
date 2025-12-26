namespace PvZRSkinPicker.Data;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

using PvZRSkinPicker.Skins;

internal sealed class PlantSkinDataDefinition(
    PlantDefinition definition,
    IPlatformService platformService)
    : ISkinDataDefinition<SeedType>
{
    public SeedType Type => definition.SeedType;

    public IEnumerable<Skin> GetSkins()
        => SkinProvider.GetSkins(definition, platformService);
}
