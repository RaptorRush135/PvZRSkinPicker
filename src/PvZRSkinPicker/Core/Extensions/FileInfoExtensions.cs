namespace PvZRSkinPicker.Extensions;

internal static class FileInfoExtensions
{
    public static byte[] ReadAllBytes(this FileInfo file)
        => File.ReadAllBytes(file.FullName);
}
