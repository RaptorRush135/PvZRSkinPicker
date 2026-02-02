namespace PvZRSkinPicker.Assets;

using System.Diagnostics.Contracts;
using System.Reflection;

using PvZRSkinPicker.Extensions;

internal sealed class EmbeddedResourceAsset(string fileName) : IModAsset
{
    private readonly string resourceName = GetResourceName(fileName);

    public string GetNameWithoutExtension() => Path.GetFileNameWithoutExtension(this.resourceName);

    public byte[] LoadBytes()
    {
        using var stream = this.LoadStream();
        return stream.ToArray();
    }

    public override string ToString() => this.resourceName;

    [Pure]
    private static string GetResourceName(string fileName)
    {
        return $"{nameof(PvZRSkinPicker)}.{nameof(Assets)}.{fileName}";
    }

    [Pure]
    private Stream LoadStream()
    {
        var assembly = Assembly.GetExecutingAssembly();

        return assembly.GetManifestResourceStream(this.resourceName)
            ?? throw new InvalidOperationException($"Resource not found: '{this.resourceName}'.");
    }
}
