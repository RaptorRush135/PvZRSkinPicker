namespace PvZRSkinPicker.Api;

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Services;

using Il2CppTekly.Localizations;

using PvZRSkinPicker.Events;

[HarmonyPatch]
internal static class AppCoreApi
{
    public static readonly OneTimeEvent<IGameplayService> OnGameplayServiceReady = new();

    public static readonly OneTimeEvent<IDataService> OnDataServiceReady = new();

    public static readonly OneTimeEvent<IPlatformService> OnPlatformServiceReady = new();

    public static readonly OneTimeEvent<ILocalizer> OnLocalizerReady = new();

    public static readonly OneTimeEvent<IAudioService> OnAudioServiceReady = new();

    static AppCoreApi()
    {
        FrontendApi.OnFirstActivation.Subscribe(ResolveServices);
    }

    private static void ResolveServices()
    {
        var gameplayService = AppCore.GetService<IGameplayService>();
        var dataService = AppCore.GetService<IDataService>();
        var platformService = AppCore.GetService<IPlatformService>();
        var localizer = GetLocalizer();
        var audioService = AppCore.GetService<IAudioService>();

        OnGameplayServiceReady.Invoke(gameplayService);
        dataService.add_OnReady((Action)(() => OnDataServiceReady.Invoke(dataService)));
        platformService.add_MainUserIdentified((Action)(() => OnPlatformServiceReady.Invoke(platformService)));
        OnLocalizerReady.Invoke(localizer);
        OnAudioServiceReady.Invoke(audioService);
    }

    private static ILocalizer GetLocalizer()
    {
        var localizerType = Il2CppSystem.Type.GetType("Tekly.Localizations.Localizer, Tekly.Localizations.Runtime")
            ?? throw new TypeLoadException("Failed to get Localizer type.");

        const string InstancePropertyName = nameof(Localizer.Instance);
        var instanceProperty = localizerType.BaseType.GetProperty(InstancePropertyName)
            ?? throw new MissingMemberException(localizerType.BaseType.FullName, InstancePropertyName);

        var localizerInstance = instanceProperty.GetValue(null);
        return localizerInstance.Cast<ILocalizer>();
    }
}
