namespace PvZRSkinPicker.Data;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Skins;

internal sealed class PlantSkinDataDefinition(
    PlantDefinition definition,
    SkinLocator skinLocator)
    : ISkinDataDefinition<SeedType>
{
    public SeedType Type => definition.SeedType;

    public IEnumerable<Skin> GetSkins() => skinLocator.GetSkins(definition);
}
