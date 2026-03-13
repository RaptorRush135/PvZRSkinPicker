namespace PvZRSkinPicker.Skins.Picker.Selection;

using MelonLoader;

using PvZRSkinPicker.Extensions;

internal sealed class SkinSelectionSet<T>
    where T : struct, Enum
{
    private readonly Dictionary<T, SkinId> selections;

    public SkinSelectionSet(
        IReadOnlyDictionary<string, string> typeToIdMap,
        Predicate<T> typeValidator,
        MelonLogger.Instance logger)
    {
        ArgumentNullException.ThrowIfNull(typeToIdMap);
        ArgumentNullException.ThrowIfNull(typeValidator);
        ArgumentNullException.ThrowIfNull(logger);

        this.selections = typeToIdMap
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

    private SkinSelectionSet()
    {
        this.selections = [];
    }

    public static SkinSelectionSet<T> Empty => field ??= new();

    public IReadOnlyDictionary<T, SkinId> Selections => field ??= this.selections.AsReadOnly();
}
