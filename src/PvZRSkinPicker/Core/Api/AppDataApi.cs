namespace PvZRSkinPicker.Api;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.DataModels;

using PvZRSkinPicker.Events;

[HarmonyPatch]
internal static class AppDataApi
{
    public static readonly OneTimeEvent<AlmanacModel> OnAlmanacBound = new();

    public static AlmanacEntriesModel? AlmanacPlantEntriesModel { get; private set; }

    public static AlmanacEntriesModel? AlmanacZombieEntriesModel { get; private set; }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AppDataProvider), nameof(AppDataProvider.OnBind))]
    private static void OnBind(AppDataProvider __instance)
    {
        OnAlmanacBound.Invoke(__instance.m_almanacModel);
        AlmanacPlantEntriesModel = __instance.m_almanacModel.m_plantsModel;
        AlmanacZombieEntriesModel = __instance.m_almanacModel.m_zombiesModel;
    }
}
