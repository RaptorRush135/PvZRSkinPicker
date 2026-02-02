namespace PvZRSkinPicker.Extensions;

using PvZRSkinPicker.Assets;

internal static class FileInfoExtensions
{
    public static BytesAsset ReadBytesAsset(this FileInfo file)
        => new(file.ReadAllBytes());

    public static byte[] ReadAllBytes(this FileInfo file)
        => File.ReadAllBytes(file.FullName);

    public static string ReadAllText(this FileInfo file)
        => File.ReadAllText(file.FullName);
}
