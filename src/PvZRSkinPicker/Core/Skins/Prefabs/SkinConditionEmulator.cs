namespace PvZRSkinPicker.Skins.Prefabs;

using PvZRSkinPicker.Api;

using PvZRSkinPicker.Skins;

internal static class SkinConditionEmulator
{
    public static void ApplyGameplayOverridesForSkinType(SkinType skinType)
    {
        GameplayServiceApi.SetOverrides(false);

        switch (skinType)
        {
            case SkinType.PreOrderPlant:
                GameplayServiceApi.PreOrderContentActiveOverride = true;
                break;
            case SkinType.RetroZombie:
                GameplayServiceApi.RetroContentActiveOverride = true;
                break;
            case SkinType.PlatformZombie:
                GameplayServiceApi.PlatformContentActiveOverride = true;
                break;
            case SkinType.China:
                GameplayServiceApi.ChinaModeActiveOverride = true;
                break;
        }
    }
}
