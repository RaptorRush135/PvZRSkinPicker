namespace PvZRSkinPicker.Api;

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Services;

using Il2CppTekly.Injectors;

using PvZRSkinPicker.Events;

[HarmonyPatch]
internal static class AppCoreApi
{
    public static readonly OneTimeEvent<IGameplayService> OnGameplayServiceReady = new();

    public static readonly OneTimeEvent<IDataService> OnDataServiceReady = new();

    public static readonly OneTimeEvent<IAudioService> OnAudioServiceReady = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AppCore), nameof(AppCore.Provide))]
    private static void Provide(InjectorContainer container)
    {
        var gameplayService = container.Get<IGameplayService>();
        var dataService = container.Get<IDataService>();
        var audioService = container.Get<IAudioService>();

        OnGameplayServiceReady.Invoke(gameplayService);
        dataService.add_OnReady((Action)(() => OnDataServiceReady.Invoke(dataService)));
        OnAudioServiceReady.Invoke(audioService);
    }
}
