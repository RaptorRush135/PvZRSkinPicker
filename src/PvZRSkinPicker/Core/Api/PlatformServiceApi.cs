namespace PvZRSkinPicker.Api;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Services;

using PvZRSkinPicker.Events;

[HarmonyPatch]
internal static class PlatformServiceApi
{
    public static readonly OneTimeEvent<IPlatformService> OnReady = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlatformService), nameof(PlatformService._onPlatformAsyncOpsCompleted))]
    private static void Provide(PlatformService __instance)
    {
        OnReady.Invoke(__instance.Cast<IPlatformService>());
    }
}
