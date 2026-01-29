namespace PvZRSkinPicker.Environment;

using MelonLoader.Utils;

using PvZRSkinPicker.Metadata;

internal static class ModEnvironment
{
    public static DirectoryInfo ModDataDirectory { get; }
        = GetDirectoryInternal(MelonEnvironment.UserDataDirectory, ModInfo.Name);

    public static DirectoryInfo SkinsDirectory { get; } = GetDirectory(ensureCreated: true, "Skins");

    public static DirectoryInfo GetDirectory(bool ensureCreated, params IEnumerable<string> paths)
    {
        var directory = GetDirectoryInternal([ModDataDirectory.FullName, .. paths]);
        if (ensureCreated)
        {
            directory.Create();
        }

        return directory;
    }

    private static DirectoryInfo GetDirectoryInternal(params IEnumerable<string> paths)
    {
        string path = Path.Join([.. paths]);
        return new DirectoryInfo(path);
    }
}
