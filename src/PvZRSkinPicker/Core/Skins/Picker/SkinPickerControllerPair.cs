namespace PvZRSkinPicker.Skins.Picker;

using Il2CppReloaded.Gameplay;

internal sealed record SkinPickerControllerPair(
    SkinPickerController<SeedType> Plant,
    SkinPickerController<ZombieType> Zombie);
