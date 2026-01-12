namespace PvZRSkinPicker.Skins.Prefabs.Zombies.Patches.Drops;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Gameplay;

[HarmonyPatch(typeof(Zombie), nameof(Zombie.DropHelm))]
internal static class ZombieDropHelmPatch
{
    [HarmonyPrefix]
    private static void Prefix(Zombie __instance)
    {
        ZombieSkinOverrideResolver.SkinConditionsPatcher.Prefix(__instance);
    }

    [HarmonyPostfix]
    private static void Postfix()
    {
        ZombieSkinOverrideResolver.SkinConditionsPatcher.Postfix();
    }
}
