namespace PvZRSkinPicker.Skins.Prefabs.Plants.Patches.PreOrder;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Api;

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class PlantFirePatch
{
    [HarmonyPrefix]
    private static void Prefix(Plant __instance)
    {
        if (__instance.mSeedType != SeedType.Peashooter
            || !__instance.mController.IsRetroContent)
        {
            return;
        }

        GameplayServiceApi.PreOrderContentActiveOverride = true;
    }

    [HarmonyPostfix]
    private static void Postfix()
    {
        GameplayServiceApi.PreOrderContentActiveOverride = null;
    }
}
