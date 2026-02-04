namespace PvZRSkinPicker.Skins.Custom.Manifest;

using Newtonsoft.Json;

internal sealed record SkinPackHeader(
    [property: JsonRequired] string Name,
    [property: JsonRequired] Guid Id,
    [property: JsonRequired] int Version,
    IReadOnlyList<string>? Authors)
{
    public string FormattedAuthors
        => field ??= this.Authors switch
        {
            null or { Count: 0 } => "???",
            _ => string.Join(", ", this.Authors),
        };

    public override string ToString() => $"{this.Name}-V{this.Version}({this.Id})";

    public string? Validate()
    {
        if (this.Id == Guid.Empty)
        {
            return "Skin pack 'Id' can not be a empty Guid";
        }

        if (this.Version <= 0)
        {
            return "Skin pack 'Version' must be greater than 0";
        }

        return null;
    }
}
