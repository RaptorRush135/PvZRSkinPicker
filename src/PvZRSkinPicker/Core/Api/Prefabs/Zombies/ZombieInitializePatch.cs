namespace PvZRSkinPicker.Api.Prefabs.Zombies;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Gameplay;

[HarmonyPatch(typeof(Zombie), nameof(Zombie.ZombieInitialize))]
internal static class ZombieInitializePatch
{
    [HarmonyPrefix]
    private static void Prefix(ZombieType theType, out ZombieInitializePatchState __state)
    {
        bool needsClear = ZombiePrefabResolver.EmulateSkinConditions(theType);
        __state = new ZombieInitializePatchState(needsClear);
    }

    [HarmonyPostfix]
    private static void Postfix(ZombieInitializePatchState __state)
    {
        if (__state.NeedsClear)
        {
            ZombiePrefabResolver.ClearGameplayOverrides();
        }
    }

    private readonly record struct ZombieInitializePatchState(
        bool NeedsClear);
}
