namespace PvZRSkinPicker.Skins.Prefabs.Zombies.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Skins.Prefabs.Zombies;

[HarmonyPatch(typeof(Zombie), nameof(Zombie.ZombieInitialize))]
internal static class ZombieInitializePatch
{
    [HarmonyPrefix]
    private static void Prefix(Zombie __instance, int theRow, ZombieType theType)
    {
        var context = new SpawnContext<ZombieType>(theType, __instance.mBoard, theRow);
        ZombieSkinOverrideResolver.SkinConditionsPatcher.Prefix(context);
    }

    [HarmonyPostfix]
    private static void Postfix()
    {
        ZombieSkinOverrideResolver.SkinConditionsPatcher.Postfix();
    }
}
