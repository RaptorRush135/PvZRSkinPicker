namespace PvZRSkinPicker.Api;

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Services;

using Il2CppTekly.Injectors;

[HarmonyPatch]
public static class AppCoreApi
{
    public static event Action<IGameplayService>? OnGameplayServiceReady;

    public static event Action<IDataService>? OnDataServiceReady;

    public static event Action<IAudioService>? OnAudioServiceReady;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AppCore), nameof(AppCore.Provide))]
    private static void Provide(InjectorContainer container)
    {
        var gameplayService = container.Get<IGameplayService>();
        var dataService = container.Get<IDataService>();
        var audioService = container.Get<IAudioService>();

        OnGameplayServiceReady?.Invoke(gameplayService);
        dataService.add_OnReady((Action)(() => OnDataServiceReady?.Invoke(dataService)));
        OnAudioServiceReady?.Invoke(audioService);
    }
}
