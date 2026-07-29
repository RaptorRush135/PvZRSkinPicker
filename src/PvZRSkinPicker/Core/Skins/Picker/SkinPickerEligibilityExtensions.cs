namespace PvZRSkinPicker.Skins.Picker;

using Il2CppReloaded.Gameplay;

internal static class SkinPickerEligibilityExtensions
{
    public static bool IsSkinPickerSupported(this SeedType type)
    {
        return type
            is >= SeedType.Peashooter
            and <= SeedType.Imitater;
    }

    public static bool IsSkinPickerSupported(this ZombieType type)
    {
        return type
            is >= ZombieType.Normal
            and <= ZombieType.Boss
            and not ZombieType.DuckyTube;
    }
}
