namespace PvZRSkinPicker.Api;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.DataModels;

using Il2CppTekly.DataModels.Models;

[HarmonyPatch]
internal static class GameplayDataProviderApi
{
    public static GameplayDataModel? CurrentModel { get; private set; }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameplayDataProvider), nameof(GameplayDataProvider.OnBind))]
    private static void OnBind(GameplayDataProvider __instance)
    {
        CurrentModel = __instance.m_gameplayDataModel;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DisposableObjectModel), nameof(DisposableObjectModel.OnDispose))]
    private static void OnDispose(DisposableObjectModel __instance)
    {
        if (__instance == CurrentModel)
        {
            CurrentModel = null;
        }
    }
}
