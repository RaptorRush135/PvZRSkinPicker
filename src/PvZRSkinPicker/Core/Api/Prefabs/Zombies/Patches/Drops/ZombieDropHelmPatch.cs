namespace PvZRSkinPicker.Api.Prefabs.Zombies.Patches.Drops;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Api.Prefabs.Patches;

[HarmonyPatch(typeof(Zombie), nameof(Zombie.DropHelm))]
internal static class ZombieDropHelmPatch
{
    [HarmonyPrefix]
    private static void Prefix(Zombie __instance, out EmulateSkinConditionsPatchState __state)
    {
        ZombiePrefabResolver.SkinConditionsPatcher.Prefix(__instance, out __state);
    }

    [HarmonyPostfix]
    private static void Postfix(EmulateSkinConditionsPatchState __state)
    {
        ZombiePrefabResolver.SkinConditionsPatcher.Postfix(__state);
    }
}
