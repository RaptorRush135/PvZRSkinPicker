namespace PvZRSkinPicker.Api;

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Services;

using Il2CppTekly.Injectors;
using Il2CppTekly.Localizations;

using PvZRSkinPicker.Events;

[HarmonyPatch]
internal static class AppCoreApi
{
    public static readonly OneTimeEvent<IGameplayService> OnGameplayServiceReady = new();

    public static readonly OneTimeEvent<IDataService> OnDataServiceReady = new();

    public static readonly OneTimeEvent<ILocalizer> OnLocalizerReady = new();

    public static readonly OneTimeEvent<IAudioService> OnAudioServiceReady = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AppCore), nameof(AppCore.Provide))]
    private static void Provide(InjectorContainer container)
    {
        var gameplayService = container.Get<IGameplayService>();
        var dataService = container.Get<IDataService>();
        var localizer = GetLocalizer();
        var audioService = container.Get<IAudioService>();

        OnGameplayServiceReady.Invoke(gameplayService);
        dataService.add_OnReady((Action)(() => OnDataServiceReady.Invoke(dataService)));
        OnLocalizerReady.Invoke(localizer);
        OnAudioServiceReady.Invoke(audioService);
    }

    private static ILocalizer GetLocalizer()
    {
        var localizerType = Il2CppSystem.Type.GetType("Tekly.Localizations.Localizer, Tekly.Localizations.Runtime")
            ?? throw new TypeLoadException("Failed to get Localizer type.");

        const string InstancePropertyName = "Instance";
        var instanceProperty = localizerType.BaseType.GetProperty(InstancePropertyName)
            ?? throw new MissingMemberException(localizerType.BaseType.FullName, InstancePropertyName);

        Il2CppSystem.Object localizerInstance = instanceProperty.GetValue(null);
        return localizerInstance.Cast<ILocalizer>();
    }
}
