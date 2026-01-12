namespace PvZRSkinPicker.Skins.Prefabs.Plants.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Gameplay;

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PlantInitializePatch
{
    [HarmonyPrefix]
    private static void Prefix(Plant __instance, int theGridY, SeedType theSeedType)
    {
        var context = new SpawnContext<SeedType>(theSeedType, __instance.mBoard, theGridY);
        PlantSkinOverrideResolver.SkinConditionsPatcher.Prefix(context);
    }

    [HarmonyPostfix]
    private static void Postfix()
    {
        PlantSkinOverrideResolver.SkinConditionsPatcher.Postfix();
    }
}
