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
    public static SkinPackManifest Load(Stream stream, Action<string>? logger)
    {
        var manifest = ModConfig.Load<SkinPackManifest>(CurrentFormatVersion, stream);

        var authors = manifest.Header.Authors;

        var nonEmptyAuthors = authors.Select(a => a?.Trim() ?? string.Empty)
            .Where((author, index) =>
            {
                if (author.Length == 0)
                {
                    logger?.Invoke($"Skin pack '{manifest}': Empty author name (Index: {index})");
                    return false;
                }

                return true;
            })
            .ToArray();

        if (nonEmptyAuthors.SequenceEqual(authors, StringComparer.Ordinal))
        {
            return manifest;
        }

        return manifest with
        {
            Header = manifest.Header with
            {
                Authors = nonEmptyAuthors,
            },
        };
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
