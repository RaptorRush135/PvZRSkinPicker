namespace PvZRSkinPicker.Skins.Custom.Manifest;

using System.Diagnostics.Contracts;

using Newtonsoft.Json;

internal sealed record SkinPackHeader(
    [property: JsonRequired] string Name,
    [property: JsonRequired] Guid Id,
    [property: JsonRequired] int Version,
    IReadOnlyList<string>? Authors)
{
    // TODO: Make required?
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
        if (this.Id == Guid.Empty)
        {
            return $"Skin pack '{nameof(this.Id)}' can not be a empty Guid";
        }

        if (this.Version <= 0)
        {
            return $"Skin pack '{nameof(this.Version)}' must be greater than 0";
        }

        return null;
    }
}
