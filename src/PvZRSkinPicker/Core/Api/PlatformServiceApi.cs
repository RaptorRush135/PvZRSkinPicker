namespace PvZRSkinPicker.Api;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Services;

using Il2CppSource.DataModels;

using PvZRSkinPicker.Events;

[HarmonyPatch]
internal static class PlatformServiceApi
{
    public static readonly OneTimeEvent<IPlatformService> OnReady = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlatformDataModel), nameof(PlatformDataModel.OnPlatformReady))]
    private static void OnPlatformReady(PlatformDataModel __instance)
    {
        OnReady.Invoke(__instance.m_platformService);
    }
}
