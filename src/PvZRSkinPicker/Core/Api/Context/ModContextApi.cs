namespace PvZRSkinPicker.Api.Context;

using HarmonyLib;

using Il2CppReloaded.DataModels;
using Il2CppReloaded.Services;

using Il2CppTekly.Localizations;

using PvZRSkinPicker.Events;
using PvZRSkinPicker.Hooks;
using PvZRSkinPicker.Skins.Prefabs.Plants;
using PvZRSkinPicker.Skins.Prefabs.Zombies;

internal static class ModContextApi
{
    public static readonly OneTimeEvent<ModContext> Ready = new();

    private static readonly HookStore HookStore = new();

    private static IDataService? dataService;

    private static IPlatformService? platformService;

    private static ILocalizer? localizer;

    private static AlmanacModel? almanac;

    private static bool fired;

    public static void Initialize(Harmony harmony)
    {
        using (var scope = new SanityCheckDetourBypass(harmony))
        {
            HookStore.Add(PlantSkinOverrideResolver.Initialize());
            HookStore.Add(ZombieSkinOverrideResolver.Initialize());
        }

        AppCoreApi.OnDataServiceReady.Subscribe(value => OnResolve(ref dataService, value));
        AppCoreApi.OnPlatformServiceReady.Subscribe(value => OnResolve(ref platformService, value));
        AppCoreApi.OnLocalizerReady.Subscribe(value => OnResolve(ref localizer, value));
        AppDataApi.OnAlmanacBound.Subscribe(value => OnResolve(ref almanac, value));
    }

    public static void Dispose()
    {
        HookStore.DetachAll();
    }

    private static void OnResolve<T>(ref T field, T value)
    {
        field = value;
        TryFire();
    }

    private static void TryFire()
    {
        if (fired)
        {
            throw new InvalidOperationException(
                "Required services were already resolved.");
        }

        if (dataService is null || platformService is null || localizer is null || almanac is null)
        {
            return;
        }

        fired = true;
        var context = new ModContext(dataService, platformService, localizer, almanac);
        Ready.Invoke(context);
    }
}
