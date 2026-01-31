namespace PvZRSkinPicker.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Text;

using PvZRSkinPicker.Skins.Custom.Manifest;

internal static class SkinPackManifestExtensions
{
    extension(SkinPackManifest)
    {
        public static SkinPackManifest Parse(
            [StringSyntax(StringSyntaxAttribute.Json)] string json)
        {
            using var stream = StringToStream(json);
            var manifest = SkinPackManifest.Load(stream);

            if (!manifest.Validate(out string? error))
            {
                throw new InvalidDataException($"Validation error: {error}.");
            }

            return manifest;
        }
    }

    private static MemoryStream StringToStream(
        string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        return new MemoryStream(bytes);
    }
}
