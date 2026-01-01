namespace PvZRSkinPicker.Api.Prefabs;

using PvZRSkinPicker.Skins;

internal static class SkinConditionEmulator
{
    public static bool ApplyGameplayOverridesForSkinType(SkinType skinType)
    {
        GameplayServiceApi.SetOverrides(false);

        switch (skinType)
        {
            case SkinType.PreOrder:
                GameplayServiceApi.PreOrderContentActiveOverride = true;
                return true;
            case SkinType.Retro:
                GameplayServiceApi.RetroContentActiveOverride = true;
                return true;
            case SkinType.Platform:
                GameplayServiceApi.PlatformContentActiveOverride = true;
                return true;
            case SkinType.China:
                GameplayServiceApi.ChinaModeActiveOverride = true;
                return true;
        }

        return false;
    }
}
