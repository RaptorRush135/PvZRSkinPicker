namespace PvZRSkinPicker.Skins.Custom.Manifest;

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

using Newtonsoft.Json;

using PvZRSkinPicker.Configuration;

internal sealed record SkinPackManifest(
    [property: JsonRequired] SkinPackHeader Header,
    [property: JsonRequired] SkinCatalog Skins)
    : IModConfig
{
    public static readonly int CurrentFormatVersion = 1;

    public int FormatVersion { get; init; }

    [Pure]
    public static SkinPackManifest Load(Stream stream)
    {
        return ModConfig.Load<SkinPackManifest>(CurrentFormatVersion, stream);
    }

    public override string ToString() => this.Header.ToString();

    [Pure]
    public bool Validate([MaybeNullWhen(true)] out string error)
    {
        error = this.Header.Validate();
        if (error != null)
        {
            return false;
        }

        error = this.Skins.Validate();
        return error == null;
    }
}
