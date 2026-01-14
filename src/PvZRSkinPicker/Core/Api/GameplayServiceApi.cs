namespace PvZRSkinPicker.Api;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable IDE0051 // Remove unused private member

using HarmonyLib;

using Il2CppReloaded.Services;

using MelonLoader;

[HarmonyPatch]
internal static class GameplayServiceApi
{
    static GameplayServiceApi()
    {
        AppCoreApi.OnGameplayServiceReady.Subscribe(service => Instance = service);
    }

    public static bool? PreOrderContentActiveOverride { get; set; }

    public static bool? RetroContentActiveOverride { get; set; }

    public static bool? PlatformContentActiveOverride { get; set; }

    public static bool? ChinaModeActiveOverride { get; set; }

    public static IGameplayService? Instance
    {
        get
        {
            if (field == null)
            {
                Melon<Core>.Logger.Warning("GameplayService not ready");
            }

            return field;
        }
        private set;
    }

    public static void SetOverrides(bool? value)
    {
        PreOrderContentActiveOverride = value;
        RetroContentActiveOverride = value;
        PlatformContentActiveOverride = value;
        ChinaModeActiveOverride = value;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameplayService), nameof(GameplayService.PreOrderContentActive), MethodType.Getter)]
    private static bool PreOrderContentActive(ref bool __result)
    {
        return ApplyOverride(PreOrderContentActiveOverride, ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameplayService), nameof(GameplayService.RetroContentActive), MethodType.Getter)]
    private static bool RetroContentActive(ref bool __result)
    {
        return ApplyOverride(RetroContentActiveOverride, ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameplayService), nameof(GameplayService.PlatformContentActive), MethodType.Getter)]
    private static bool PlatformContentActive(ref bool __result)
    {
        return ApplyOverride(PlatformContentActiveOverride, ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameplayService), nameof(GameplayService.ChinaModeActive), MethodType.Getter)]
    private static bool ChinaModeActive(ref bool __result)
    {
        return ApplyOverride(ChinaModeActiveOverride, ref __result);
    }

    private static bool ApplyOverride(bool? @override, ref bool result)
    {
        if (@override is { } overrideValue)
        {
            result = overrideValue;
            return false;
        }

        return true;
    }
}
