namespace PvZRSkinPicker.Api.Context;

using Il2CppReloaded.DataModels;
using Il2CppReloaded.Services;

internal static class ModContextApi
{
    private static IDataService? dataService;

    private static IPlatformService? platformService;

    private static AlmanacModel? almanac;

    private static bool fired;

    static ModContextApi()
    {
        AudioServiceApi.Initialize();
        AppCoreApi.OnDataServiceReady += value => OnResolve(ref dataService, value);
        PlatformServiceApi.OnReady += value => OnResolve(ref platformService, value);
        AppDataApi.OnAlmanacBound += value => OnResolve(ref almanac, value);
    }

    public static event Action<ModContext>? Ready;

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

        if (dataService is null || almanac is null || platformService is null)
        {
            return;
        }

        fired = true;
        var context = new ModContext(dataService, platformService, almanac);
        Ready?.Invoke(context);
    }
}
