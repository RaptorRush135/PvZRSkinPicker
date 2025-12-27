namespace PvZRSkinPicker.Api;

using System.Diagnostics.CodeAnalysis;

using Il2CppReloaded.Services;

using MelonLoader;

internal static class AudioServiceApi
{
    private static IAudioService? audioService;

    [SuppressMessage(
        "Minor Code Smell",
        "S3963:\"static\" fields should be initialized inline",
        Justification = "False positive (https://github.com/SonarSource/sonar-dotnet/issues/9656).")]
    static AudioServiceApi()
    {
        AppCoreApi.OnAudioServiceReady += value => audioService = value;
    }

    public static void Initialize() => GC.KeepAlive(audioService);

    public static void PlayWithRandomPitch(FoleyType foleyType)
    {
        if (audioService == null)
        {
            Melon<Core>.Logger.Warning(
                $"Audio service is not available. Cannot play sound '{foleyType}'");

            return;
        }

        float pitch = UnityEngine.Random.Range(-10, 10);
        audioService.PlayFoleyPitch(foleyType, pitch);
    }
}
