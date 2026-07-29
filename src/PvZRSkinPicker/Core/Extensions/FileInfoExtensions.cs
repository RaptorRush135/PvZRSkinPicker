namespace PvZRSkinPicker.Extensions;

using PvZRSkinPicker.Assets;

using SolarApi.IO.Extensions;

internal static class FileInfoExtensions
{
    public static BytesAsset ReadBytesAsset(this FileInfo file)
        => new(file.ReadAllBytes());
}
