namespace PvZRSkinPicker.Api;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable IDE0051 // Remove unused private member

using HarmonyLib;

using Il2Cpp;

using Il2CppReloaded.Services;

using Microsoft.Extensions.DependencyInjection;

using SolarApi;

[HarmonyPatch]
internal static class AudioServiceApi
{
    private static IAudioService AudioService
        => field ??= Solar<SkinPickerMod>.Provider.GetRequiredService<IAudioService>();

    private static bool BypassDebounce { get; set; }

    public static void PlayWithRandomPitch(FoleyType foleyType)
    {
        float pitch = UnityEngine.Random.Range(-10, 10);
        try
        {
            BypassDebounce = true;
            AudioService.PlayFoleyPitch(foleyType, pitch);
        }
        finally
        {
            BypassDebounce = false;
        }
    }

    [HarmonyPatch(typeof(AudioSourceWrapper), nameof(AudioSourceWrapper.TimeSinceLastPlayed), MethodType.Getter)]
    private static bool Prefix(ref float __result)
    {
        if (BypassDebounce)
        {
            __result = 1;
            return false;
        }

        return true;
    }
}
