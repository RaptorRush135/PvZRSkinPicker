namespace PvZRSkinPicker.Skins.Custom;

using Il2CppReloaded.Gameplay;

using Microsoft.Extensions.Logging;

internal sealed class SkinLoaderFactory(
    ISkinTypeHandler<SeedType> plantTypeHandler)
{
    public SkinLoader<SeedType> CreateForPlants(ILogger logger)
        => new(logger, plantTypeHandler);
}
