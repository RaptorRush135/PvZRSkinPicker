namespace PvZRSkinPicker.Skins.Prefabs.Plants.Patches.PreOrder;

#pragma warning disable S3265 // Non-flags enums should not be used in bitwise operations
#pragma warning disable RCS1130 // Bitwise operation on enum without Flags attribute

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Api;

[HarmonyPatch(typeof(GameplayPooler), nameof(GameplayPooler.GetProjectileController))]
internal static class GetProjectileControllerPatch
{
    private const ProjectileType PreorderFlag = (ProjectileType)(1 << 30);

    [HarmonyPrefix]
    private static void Prefix(ProjectileDefinition projectileDefinition)
    {
        if (projectileDefinition.ProjectileType != ProjectileType.PeashooterPea)
        {
            return;
        }

        if (GameplayServiceApi.Instance.PreOrderContentActive)
        {
            // Unique key for the pool
            projectileDefinition.m_projectileType |= PreorderFlag;
        }
    }

    [HarmonyPostfix]
    private static void Postfix(ProjectileDefinition projectileDefinition)
    {
        projectileDefinition.m_projectileType &= ~PreorderFlag;
    }
}
