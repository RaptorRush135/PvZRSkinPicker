namespace PvZRSkinPicker.Skins.Custom;

using Il2CppReloaded.Gameplay;

internal sealed record CustomSkinSet(
    IReadOnlyDictionary<SeedType, IReadOnlyList<Skin>> Plants,
    IReadOnlyDictionary<ZombieType, IReadOnlyList<Skin>> Zombies);
