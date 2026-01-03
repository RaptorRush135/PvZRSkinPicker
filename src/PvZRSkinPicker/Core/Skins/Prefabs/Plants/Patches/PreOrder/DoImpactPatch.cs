namespace PvZRSkinPicker.Skins.Prefabs.Plants.Patches.PreOrder;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Gameplay;

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class DoImpactPatch
{
    [HarmonyPrefix]
    private static void Prefix(Projectile __instance)
    {
        if (PreOrderConditionPatchHelper.IsRetroPea(__instance))
        {
            PreOrderConditionPatchHelper.EnablePreOrderContent();
        }
    }

    [HarmonyPostfix]
    private static void Postfix()
    {
        PreOrderConditionPatchHelper.ClearPreOrderContent();
    }
}
