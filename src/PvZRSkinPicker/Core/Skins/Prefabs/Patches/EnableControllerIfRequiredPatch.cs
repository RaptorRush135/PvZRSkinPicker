namespace PvZRSkinPicker.Skins.Prefabs.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppSource.Controllers;

using PvZRSkinPicker.Unity;

[HarmonyPatch]
internal static class EnableControllerIfRequiredPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ReloadedController), nameof(ReloadedController.Init))]
    private static void Prefix(ReloadedController __instance)
    {
        RequiresActivationMarker.ActivateIfRequired(__instance.gameObject);
    }
}
