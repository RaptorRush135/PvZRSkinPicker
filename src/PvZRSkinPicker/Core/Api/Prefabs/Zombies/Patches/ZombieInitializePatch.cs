namespace PvZRSkinPicker.Api.Prefabs.Zombies.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Api.Prefabs.Patches;

[HarmonyPatch(typeof(Zombie), nameof(Zombie.ZombieInitialize))]
internal static class ZombieInitializePatch
{
    [HarmonyPrefix]
    private static void Prefix(ZombieType theType, out EmulateSkinConditionsPatchState __state)
    {
        ZombiePrefabResolver.SkinConditionsPatcher.Prefix(theType, out __state);
    }

    [HarmonyPostfix]
    private static void Postfix(EmulateSkinConditionsPatchState __state)
    {
        ZombiePrefabResolver.SkinConditionsPatcher.Postfix(__state);
    }
}
