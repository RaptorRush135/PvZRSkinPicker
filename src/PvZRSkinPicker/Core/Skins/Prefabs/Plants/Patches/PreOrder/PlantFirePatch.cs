namespace PvZRSkinPicker.Skins.Prefabs.Plants.Patches.PreOrder;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Skins.Prefabs.Patches;

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class PlantFirePatch
{
    [HarmonyPrefix]
    private static void Prefix(Plant __instance, out EmulateSkinConditionsPatchState __state)
    {
        SeedType seedType = __instance.mSeedType;
        if (seedType != SeedType.Peashooter)
        {
            __state = new(NeedsClear: false);
            return;
        }

        PlantPrefabResolver.SkinConditionsPatcher.Prefix(__instance.mSeedType, out __state);
    }

    [HarmonyPostfix]
    private static void Postfix(EmulateSkinConditionsPatchState __state)
    {
        PlantPrefabResolver.SkinConditionsPatcher.Postfix(__state);
    }
}
