namespace PvZRSkinPicker.Api.Prefabs.Zombies;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.NativeUtils;
using PvZRSkinPicker.Skins;

using UnityEngine.AddressableAssets;

[HarmonyPatch]
internal static class ZombiePrefabResolver
{
    private static Dictionary<ZombieType, Skin> Overrides { get; } = [];

    public static IFunctionHook Initialize()
    {
        return ZombieDeserializePatch.Initialize();
    }

    public static void SetOverride(ZombieType type, Skin skin)
    {
        Overrides[type] = skin;
    }

    public static bool EmulateSkinConditions(ZombieType zombieType)
    {
        if (!Overrides.TryGetValue(zombieType, out var skin))
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

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZombieDefinition), nameof(ZombieDefinition.Prefab), MethodType.Getter)]
    private static bool GetPrefabPrefix(
        ZombieDefinition __instance,
        ref AssetReferenceGameObject __result)
    {
        // TODO: Check if almanac open?
        if (Overrides.TryGetValue(__instance.ZombieType, out var skin))
        {
            __result = skin.Prefab;
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameplayPooler), nameof(GameplayPooler.GetZombieController))]
    private static void GetZombieControllerPrefix(
        ZombieDefinition zombieDefinition,
        ref bool easterEggAllowed,
        ref bool forceDecember,
        ref bool forceNormal,
        out GetZombieControllerPatchState __state)
    {
        /*
         * Zombie.ZombieInitialize:
         *   easterEggAllowed = true
         *   forceDecember    = false
         *   forceNormal      = false
         *
         * Zombie.Deserialize:
         *   easterEggAllowed = false
         *   forceDecember    = saveFile
         *   forceNormal      = !forceDecember
         */
        __state = new(zombieDefinition.EasterEggChance);

        if (forceDecember)
        {
            return;
        }

        if (!Overrides.TryGetValue(zombieDefinition.ZombieType, out var skin))
        {
            return;
        }

        SkinType skinType = skin.Type;

        if (skinType == SkinType.Normal)
        {
            forceNormal = true;
            return;
        }

        forceNormal = false;

        if (skinType == SkinType.EasterEgg)
        {
            easterEggAllowed = true;
            zombieDefinition.m_easterEggChance100 = 100;
            return;
        }

        easterEggAllowed = false;

        if (skinType == SkinType.December)
        {
            forceDecember = true;
        }
    }

    [HarmonyFinalizer]
    [HarmonyPatch(typeof(GameplayPooler), nameof(GameplayPooler.GetZombieController))]
    private static void GetZombieControllerFinalizer(
        ZombieDefinition zombieDefinition,
        GetZombieControllerPatchState __state)
    {
        zombieDefinition.m_easterEggChance100 = __state.OriginalEasterEggChance;
    }

    private readonly record struct GetZombieControllerPatchState(
        float OriginalEasterEggChance);
}
