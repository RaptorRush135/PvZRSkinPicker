namespace PvZRSkinPicker.Api;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Data;

using Il2CppTekly.Extensions.PanelViews;

using MelonLoader;

[HarmonyPatch]
internal static class AlmanacApi
{
    public static readonly MelonEvent<AlmanacEntryType> OnAlmanacOpened = new();

    public static readonly MelonEvent<AlmanacEntryType> OnAlmanacClosed = new();

    private const string PlantAlmanacId = "almanacPlants";

    private const string ZombieAlmanacId = "almanacZombies";

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PanelViewActivity), nameof(PanelViewActivity.IsDoneLoading))]
    private static void IsDoneLoading(PanelViewActivity __instance)
    {
        RaiseAlmanacEvent(__instance, OnAlmanacOpened);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PanelViewActivity), nameof(PanelViewActivity.UnloadingStarted))]
    private static void UnloadingStarted(PanelViewActivity __instance)
    {
        RaiseAlmanacEvent(__instance, OnAlmanacClosed);
    }

    private static void RaiseAlmanacEvent(PanelViewActivity panelViewActivity, MelonEvent<AlmanacEntryType> @event)
    {
        switch (panelViewActivity.m_panelId)
        {
            case PlantAlmanacId:
                @event.Invoke(AlmanacEntryType.Plant);
                break;
            case ZombieAlmanacId:
                @event.Invoke(AlmanacEntryType.Zombie);
                break;
        }
    }
}
