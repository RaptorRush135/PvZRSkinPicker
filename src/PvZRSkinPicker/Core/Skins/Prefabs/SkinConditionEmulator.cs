namespace PvZRSkinPicker.Skins.Prefabs;

using PvZRSkinPicker.Api;

using PvZRSkinPicker.Skins;

internal static class SkinConditionEmulator
{
    public static bool ApplyGameplayOverridesForSkinType(SkinType skinType)
    {
        GameplayServiceApi.SetOverrides(false);

        switch (skinType)
        {
            case SkinType.PreOrderPlant:
                GameplayServiceApi.PreOrderContentActiveOverride = true;
                return true;
            case SkinType.RetroZombie:
                GameplayServiceApi.RetroContentActiveOverride = true;
                return true;
            case SkinType.PlatformZombie:
                GameplayServiceApi.PlatformContentActiveOverride = true;
                return true;
            case SkinType.China:
                GameplayServiceApi.ChinaModeActiveOverride = true;
                return true;
        }

        return false;
    }
}
