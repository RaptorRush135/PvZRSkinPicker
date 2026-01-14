namespace PvZRSkinPicker.Api;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable IDE0051 // Remove unused private member

using System.Diagnostics.CodeAnalysis;

using HarmonyLib;

using Il2Cpp;

using Il2CppReloaded.Services;

using MelonLoader;

[HarmonyPatch]
internal static class AudioServiceApi
{
    private static IAudioService? audioService;

    [SuppressMessage(
        "Minor Code Smell",
        "S3963:\"static\" fields should be initialized inline",
        Justification = "False positive (https://github.com/SonarSource/sonar-dotnet/issues/9656).")]
    static AudioServiceApi()
    {
        AppCoreApi.OnAudioServiceReady.Subscribe(value => audioService = value);
    }

    private static bool BypassDebounce { get; set; }

    public static void PlayWithRandomPitch(FoleyType foleyType)
    {
        if (audioService == null)
        {
            Melon<Core>.Logger.Warning(
                $"Audio service is not available. Cannot play sound '{foleyType}'");

            return;
        }

        float pitch = UnityEngine.Random.Range(-10, 10);
        try
        {
            BypassDebounce = true;
            audioService.PlayFoleyPitch(foleyType, pitch);
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
