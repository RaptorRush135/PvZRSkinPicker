namespace PvZRSkinPicker.Environment;

using SolarApi.MelonLoader.Environment;

internal sealed class SkinPickerModEnvironment(string modName)
    : ModEnvironment(modName)
{
    public DirectoryInfo SkinPacksDirectory
        => field ??= this.GetDirectory(ensureCreated: true, "SkinPacks");
}
