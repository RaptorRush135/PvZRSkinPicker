namespace PvZRSkinPicker.Skins.Picker.Selection;

using System.Collections.Immutable;

using MelonLoader;

using PvZRSkinPicker.Extensions;

internal sealed class SkinSelectionSet<T>
    where T : struct, Enum
{
    public SkinSelectionSet(
        IReadOnlyDictionary<string, string> typeToIdMap,
        Predicate<T> typeValidator,
        MelonLogger.Instance logger)
    {
        ArgumentNullException.ThrowIfNull(typeToIdMap);
        ArgumentNullException.ThrowIfNull(typeValidator);
        ArgumentNullException.ThrowIfNull(logger);

        this.Selections = typeToIdMap
            .Select(TryParsePair)
            .WhereNotNull()
            .ToDictionary();

        KeyValuePair<T, SkinId>? TryParsePair(KeyValuePair<string, string> pair)
        {
            if (!Enum.TryParse<T>(pair.Key, ignoreCase: true, out var targetType)
                || !typeValidator.Invoke(targetType))
            {
                logger.Warning($"Could not parse skin type: '{pair.Key}'");
                return null;
            }

            if (!SkinId.TryParse(pair.Value, out var skinId))
            {
                logger.Warning($"Could not parse skin id: '{pair.Value}'");
                return null;
            }

            return new(targetType, skinId);
        }
    }

    public SkinSelectionSet(
        IReadOnlyDictionary<T, SkinId> selections)
    {
        ArgumentNullException.ThrowIfNull(selections);

        this.Selections = new Dictionary<T, SkinId>(selections);
    }

    private SkinSelectionSet()
    {
        this.Selections = ImmutableDictionary<T, SkinId>.Empty;
    }

    public static SkinSelectionSet<T> Empty => field ??= new();

    public IReadOnlyDictionary<T, SkinId> Selections { get; }

    public Dictionary<string, string> ToStringMap()
    {
        return this.Selections.ToDictionary(
            pair => pair.Key.ToString(),
            pair => pair.Value.Id);
    }
}
