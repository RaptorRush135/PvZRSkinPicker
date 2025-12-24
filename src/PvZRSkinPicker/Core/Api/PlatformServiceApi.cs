namespace PvZRSkinPicker.Api;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Services;

[HarmonyPatch]
public static class PlatformServiceApi
{
    public static event Action<IPlatformService>? OnReady;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlatformService), nameof(PlatformService._onPlatformAsyncOpsCompleted))]
    private static void Provide(PlatformService __instance)
    {
        OnReady?.Invoke(__instance.Cast<IPlatformService>());
    }
}
