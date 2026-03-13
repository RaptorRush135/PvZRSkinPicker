namespace PvZRSkinPicker.Skins.Picker.Selection;

using System.Collections.Immutable;
using System.Diagnostics.Contracts;

using PvZRSkinPicker.Config;

internal sealed record SkinSelectionConfig : IModConfig
{
    public static readonly int CurrentFormatVersion = 1;

    public int FormatVersion { get; init; }

    public IReadOnlyDictionary<string, string> Plants { get; init; } = Empty;

    public IReadOnlyDictionary<string, string> Zombies { get; init; } = Empty;

    private static IReadOnlyDictionary<string, string> Empty
        => ImmutableDictionary<string, string>.Empty;

    [Pure]
    public static SkinSelectionConfig Load(Stream stream)
    {
        return ModConfigReader.Load<SkinSelectionConfig>(CurrentFormatVersion, stream);
    }
}
