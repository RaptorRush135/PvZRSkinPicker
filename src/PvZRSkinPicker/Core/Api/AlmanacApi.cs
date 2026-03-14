namespace PvZRSkinPicker.Api;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Data;

using Il2CppTekly.Extensions.PanelViews;

using MelonLoader;

[HarmonyPatch]
internal static class AlmanacApi
{
    public static readonly MelonEvent<AlmanacEntryType> OnAlmanacClosed = new();

    private const string PlantAlmanacId = "almanacPlants";

    private const string ZombieAlmanacId = "almanacZombies";

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PanelViewActivity), nameof(PanelViewActivity.UnloadingStarted))]
    private static void UnloadingStarted(PanelViewActivity __instance)
    {
        switch (__instance.m_panelId)
        {
            case PlantAlmanacId:
                OnAlmanacClosed.Invoke(AlmanacEntryType.Plant);
                break;
            case ZombieAlmanacId:
                OnAlmanacClosed.Invoke(AlmanacEntryType.Zombie);
                break;
        }
    }
}
