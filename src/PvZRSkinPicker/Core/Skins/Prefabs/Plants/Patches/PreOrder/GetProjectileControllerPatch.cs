namespace PvZRSkinPicker.Skins.Prefabs.Plants.Patches.PreOrder;

#pragma warning disable S3265 // Non-flags enums should not be used in bitwise operations
#pragma warning disable RCS1130 // Bitwise operation on enum without Flags attribute

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

[HarmonyPatch(typeof(GameplayPooler), nameof(GameplayPooler.GetProjectileController))]
internal static class GetProjectileControllerPatch
{
    private static readonly ProjectileType PreorderFlag
        = PreOrderConditionPatchHelper.GetUniquePreOrderKey<ProjectileType>();

    [HarmonyPrefix]
    private static void Prefix(ProjectileDefinition projectileDefinition)
    {
        if (projectileDefinition.ProjectileType is ProjectileType.PeashooterPea or ProjectileType.PeashooterFireball
            && PreOrderConditionPatchHelper.IsContentActive)
        {
            projectileDefinition.m_projectileType |= PreorderFlag;
        }
    }

    [HarmonyPostfix]
    private static void Postfix(ProjectileDefinition projectileDefinition)
    {
        projectileDefinition.m_projectileType &= ~PreorderFlag;
    }
}
