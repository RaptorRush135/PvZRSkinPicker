namespace PvZRSkinPicker.Skins.Picker.Selection;

using System.Collections.Immutable;
using System.Diagnostics.Contracts;

using PvZRSkinPicker.Configuration;

internal sealed record SkinSelectionConfig : IModConfig
{
    public static readonly int CurrentFormatVersion = 1;

    public int FormatVersion { get; init; } = CurrentFormatVersion;

    public IReadOnlyDictionary<string, string> Plants { get; init; } = Empty;

    public IReadOnlyDictionary<string, string> Zombies { get; init; } = Empty;

    private static IReadOnlyDictionary<string, string> Empty
        => ImmutableDictionary<string, string>.Empty;

    [Pure]
    public static SkinSelectionConfig Load(Stream stream)
    {
        return ModConfig.Load<SkinSelectionConfig>(CurrentFormatVersion, stream);
    }

    public void Write(Stream stream)
    {
        ModConfig.Write(this, stream);
    }
}
