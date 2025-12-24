namespace PvZRSkinPicker.Api;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.DataModels;

[HarmonyPatch]
public static class AppDataApi
{
    public static event Action<AlmanacModel>? OnAlmanacBound;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AppDataProvider), nameof(AppDataProvider.OnBind))]
    private static void OnBind(AppDataProvider __instance)
    {
        OnAlmanacBound?.Invoke(__instance.m_almanacModel);
    }
}
