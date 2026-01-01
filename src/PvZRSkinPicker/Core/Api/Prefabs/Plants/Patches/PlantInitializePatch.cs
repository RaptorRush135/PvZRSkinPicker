namespace PvZRSkinPicker.Api.Prefabs.Plants.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Api.Prefabs.Patches;

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PlantInitializePatch
{
    [HarmonyPrefix]
    private static void Prefix(SeedType theSeedType, out EmulateSkinConditionsPatchState __state)
    {
        PlantPrefabResolver.SkinConditionsPatcher.Prefix(theSeedType, out __state);
    }

    [HarmonyPostfix]
    private static void Postfix(EmulateSkinConditionsPatchState __state)
    {
        PlantPrefabResolver.SkinConditionsPatcher.Postfix(__state);
    }
}
