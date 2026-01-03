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
        return projectile.mController.gameObject.name == "Projectile-Pea_Retro(Clone)";
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
