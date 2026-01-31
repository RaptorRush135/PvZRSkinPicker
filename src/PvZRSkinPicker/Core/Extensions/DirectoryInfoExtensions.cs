namespace PvZRSkinPicker.Extensions;

internal static class DirectoryInfoExtensions
{
    public static DirectoryInfo GetDirectory(this DirectoryInfo directory, string relativePath)
        => new(Path.Join(directory.FullName, relativePath));

    public static FileInfo GetFile(this DirectoryInfo directory, string relativePath)
        => new(Path.Join(directory.FullName, relativePath));

    public static FileInfo? GetFileIfExists(this DirectoryInfo directory, string relativePath)
    {
        var file = directory.GetFile(relativePath);
        return file.Exists ? file : null;
    }
}
