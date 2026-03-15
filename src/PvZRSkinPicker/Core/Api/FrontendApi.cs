namespace PvZRSkinPicker.Api;

using HarmonyLib;

using Il2CppReloaded.TreeStateActivities;

using PvZRSkinPicker.Events;

[HarmonyPatch]
internal static class FrontendApi
{
    public static readonly OneTimeEvent OnFirstActivation = new();

    private static bool hasActivatedOnce;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(FrontendActivity), nameof(FrontendActivity.ActiveStarted))]
    private static void ActiveStarted()
    {
        if (!hasActivatedOnce)
        {
            hasActivatedOnce = true;
            OnFirstActivation.Invoke();
        }
    }
}
