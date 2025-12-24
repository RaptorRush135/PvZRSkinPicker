namespace PvZRSkinPicker.Api;

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Services;

using Il2CppTekly.Injectors;

[HarmonyPatch]
public static class DataServiceApi
{
    public static event Action<IDataService>? OnReady;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AppCore), nameof(AppCore.Provide))]
    private static void Provide(InjectorContainer container)
    {
        var dataService = container.Get<IDataService>();

        dataService.add_OnReady((Action)(() => OnReady?.Invoke(dataService)));
    }
}
