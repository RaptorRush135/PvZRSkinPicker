namespace PvZRSkinPicker.Almanac.Extensions;

using Il2CppReloaded.Gameplay;

internal static class AlmanacTypeExtensions
{
    public static bool IsInAlmanac(this SeedType type)
    {
        return type is >= SeedType.Peashooter
            && type is <= SeedType.Imitater;
    }

    public static bool IsInAlmanac(this ZombieType type)
    {
        return type is >= ZombieType.Normal
            && type is <= ZombieType.Boss;
    }
}
