namespace PvZRSkinPicker.Skins.Prefabs.Plants.Patches.PreOrder;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Api;

internal static class PreOrderConditionPatchHelper
{
    public static bool IsContentActive => GameplayServiceApi.Instance.PreOrderContentActive;

    public static T GetUniquePreOrderKey<T>()
        where T : struct, Enum
    {
        return (T)Enum.ToObject(typeof(T), 1 << 30);
    }

    public static bool IsRetroPea(Projectile projectile)
    {
        return projectile.mProjectileType switch
        {
            ProjectileType.PeashooterPea
                => MatchesProjectileGameObjectName("Projectile-Pea_Retro(Clone)"),
            ProjectileType.PeashooterFireball
                => MatchesProjectileGameObjectName("Projectile-Fireball_Retro(Clone)"),
            _ => false,
        };

        bool MatchesProjectileGameObjectName(string expectedName)
            => projectile.mController.gameObject.name == expectedName;
    }

    public static void EnablePreOrderContent()
    {
        GameplayServiceApi.PreOrderContentActiveOverride = true;
    }

    public static void ClearPreOrderContent()
    {
        GameplayServiceApi.PreOrderContentActiveOverride = null;
    }
}
