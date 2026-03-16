namespace PvZRSkinPicker.Skins.Custom.Manifest;

using System.Diagnostics.Contracts;

using Newtonsoft.Json;

using PvZRSkinPicker.Configuration;

internal sealed record SkinPackHeader(
    [property: JsonRequired] string Name,
    [property: JsonRequired] Guid Id,
    [property: JsonRequired] int Version,
    [property: JsonRequired] IReadOnlyList<string> Authors)
{
    public string FormattedAuthors
        => field ??= this.Authors switch
        {
            null or { Count: 0 } => "???",
            _ => string.Join(", ", this.Authors),
        };

    public override string ToString() => $"{this.Name}-V{this.Version}({this.Id})";

    [Pure]
    public string? Validate()
    {
        if (ModConfigValidator.ValidateString(
            this.Name,
            nameof(this.Name),
            ModConfigValidator.IsPrintableAscii,
            30) is { } nameError)
        {
            return nameError;
        }

        if (this.Id == Guid.Empty)
        {
            return $"Skin pack '{nameof(this.Id)}' can not be a empty Guid";
        }

        if (this.Version <= 0)
        {
            return $"Skin pack '{nameof(this.Version)}' must be greater than 0";
        }

        if (this.Authors.Count < 1)
        {
            return "Skin pack must contain at least one author";
        }

        if (ModConfigValidator.ValidateStrings(
            this.Authors,
            nameof(this.Authors),
            ModConfigValidator.IsPrintableAscii,
            30) is { } authors)
        {
            return authors;
        }

        return null;
    }
}
