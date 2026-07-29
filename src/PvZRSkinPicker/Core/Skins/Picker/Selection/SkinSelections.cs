namespace PvZRSkinPicker.Skins.Picker.Selection;

using System.Diagnostics.Contracts;

using Il2CppReloaded.Gameplay;

using Microsoft.Extensions.Logging;

using PvZRSkinPicker.Skins.Picker;

internal sealed record SkinSelections(
    SkinSelectionSet<SeedType> Plants,
    SkinSelectionSet<ZombieType> Zombies)
{
    public static SkinSelections Empty => field ??= new(
        SkinSelectionSet<SeedType>.Empty,
        SkinSelectionSet<ZombieType>.Empty);

    [Pure]
    public static SkinSelections Parse(SkinSelectionConfig config, ILogger logger)
    {
        return new(
            ParseSet<SeedType>(config.Plants, p => p.IsSkinPickerSupported()),
            ParseSet<ZombieType>(config.Zombies, z => z.IsSkinPickerSupported()));

        SkinSelectionSet<T> ParseSet<T>(
            IReadOnlyDictionary<string, string> typeToIdMap,
            Predicate<T> typeValidator)
            where T : struct, Enum
            => new(typeToIdMap, typeValidator, logger);
    }

    [Pure]
    public SkinSelectionConfig ToConfig()
    {
        return new()
        {
            Plants = this.Plants.ToStringMap(),
            Zombies = this.Zombies.ToStringMap(),
        };
    }
}
