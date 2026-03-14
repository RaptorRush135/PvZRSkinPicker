namespace PvZRSkinPicker.Skins.Picker.Selection;

using System.Diagnostics.Contracts;

using Il2CppReloaded.Gameplay;

using MelonLoader;

using PvZRSkinPicker.Almanac.Extensions;

internal sealed record SkinSelections(
    SkinSelectionSet<SeedType> Plants,
    SkinSelectionSet<ZombieType> Zombies)
{
    public static SkinSelections Empty => field ??= new(
        SkinSelectionSet<SeedType>.Empty,
        SkinSelectionSet<ZombieType>.Empty);

    [Pure]
    public static SkinSelections Parse(SkinSelectionConfig config)
    {
        return new(
            ParseSet<SeedType>(config.Plants, p => p.IsInAlmanac()),
            ParseSet<ZombieType>(config.Zombies, z => z.IsInAlmanac()));

        static SkinSelectionSet<T> ParseSet<T>(
            IReadOnlyDictionary<string, string> typeToIdMap,
            Predicate<T> typeValidator)
            where T : struct, Enum
            => new(typeToIdMap, typeValidator, Melon<Core>.Logger);
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
