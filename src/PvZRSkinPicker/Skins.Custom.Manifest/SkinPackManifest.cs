namespace PvZRSkinPicker.Skins.Custom.Manifest;

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal sealed record SkinPackManifest(
    [property: JsonRequired] SkinPackHeader Header,
    [property: JsonRequired] SkinCatalog Skins)
{
    public static readonly int CurrentFormatVersion = 1;

    public const string FormatVersionKey = "format_version";

    private static readonly JsonLoadSettings LoadSettings = new()
    {
        CommentHandling = CommentHandling.Ignore,
        LineInfoHandling = LineInfoHandling.Ignore,
        DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error,
    };

    public static SkinPackManifest Load(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(reader);

        var root = JObject.Load(jsonReader, LoadSettings);

        if (!root.TryGetValue(FormatVersionKey, out var token) ||
            token.Type != JTokenType.Integer)
        {
            throw new InvalidDataException($"Missing or invalid {FormatVersionKey}.");
        }

        var version = token.Value<int>();
        if (version != CurrentFormatVersion)
        {
            throw new InvalidDataException(); // TODO
        }

        var serializer = JsonSerializer.Create();
        return root.ToObject<SkinPackManifest>(serializer);
    }

    public override string ToString() => this.Header.ToString();

    public bool Validate([MaybeNullWhen(true)] out string error)
    {
        error = this.Header.Validate();
        return error == null;
    }
}
