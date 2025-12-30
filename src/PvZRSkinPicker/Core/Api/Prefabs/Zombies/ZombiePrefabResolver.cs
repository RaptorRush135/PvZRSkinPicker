namespace PvZRSkinPicker.Api.Prefabs.Zombies;

using System.Collections.ObjectModel;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Api.Prefabs.Zombies.Patches;
using PvZRSkinPicker.NativeUtils;
using PvZRSkinPicker.Skins;

internal static class ZombiePrefabResolver
{
    private static readonly Dictionary<ZombieType, Skin> OverridesMap = [];

    public static ReadOnlyDictionary<ZombieType, Skin> Overrides { get; } = OverridesMap.AsReadOnly();

    public static IFunctionHook Initialize()
    {
        return ZombieDeserializePatch.Initialize();
    }

    public static void SetOverride(ZombieType type, Skin skin)
    {
        OverridesMap[type] = skin;
    }

    public static bool EmulateSkinConditions(ZombieType zombieType)
    {
        if (!OverridesMap.TryGetValue(zombieType, out var skin))
        {
            return false;
        }

        GameplayServiceApi.SetOverrides(false);

        switch (skin.Type)
        {
            case SkinType.Retro:
                GameplayServiceApi.RetroContentActiveOverride = true;
                return true;
            case SkinType.Platform:
                GameplayServiceApi.PlatformContentActiveOverride = true;
                return true;
            case SkinType.China:
                GameplayServiceApi.ChinaModeActiveOverride = true;
                return true;
        }

        return false;
    }

    public static void ClearGameplayOverrides()
    {
        GameplayServiceApi.SetOverrides(null);
    }
}
