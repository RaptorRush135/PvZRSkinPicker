namespace PvZRSkinPicker.Skins;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

using PvZRSkinPicker.Extensions;

using UnityEngine.AddressableAssets;

internal static class SkinLocator
{
    public static IEnumerable<Skin> GetSkins(
        PlantDefinition definition,
        IPlatformService platformService)
    {
        // TODO: Filter to only in almanac?
        IEnumerable<Skin?> skins =
        [
            new Skin(SkinType.Normal, definition.m_prefab),
            TryCreateSkin(SkinType.PreOrderPlant, definition.m_preorderGameObject, platformService.PreOrderDLCAvailable),
            TryCreateSkin(SkinType.China, definition.m_chinaGameObject),
            TryCreateSkin(SkinType.EasterEgg, definition.m_easterEggGameObject),
            TryCreateSkin(SkinType.December, definition.m_decemberGameObject),
        ];

        return skins.WhereNotNull();
    }

    public static IEnumerable<Skin> GetSkins(
        ZombieDefinition definition,
        IPlatformService platformService)
    {
        // "DuckyTube" uses the skins of "Normal", so do not scan for skins
        if (definition.ZombieType == ZombieType.DuckyTube)
        {
            return [];
        }

        IEnumerable<Skin?> skins =
        [
            new Skin(SkinType.Normal, definition.m_prefab),
            TryCreateSkin(SkinType.RetroZombie, definition.m_retroGameObject, platformService.RetroContentAvailable),
            TryCreateSkin(SkinType.PlatformZombie, definition.m_platformGameObject, platformService.PlatformContentAvailable),
            TryCreateSkin(SkinType.China, definition.m_chinaGameObject),
            TryCreateSkin(SkinType.EasterEgg, definition.m_easterEggGameObject),
            TryCreateSkin(SkinType.December, definition.m_decemberGameObject),
        ];

        return skins.WhereNotNull();
    }

    private static Skin? TryCreateSkin(SkinType type, AssetReferenceGameObject prefab, bool enabled = true)
    {
        return string.IsNullOrEmpty(prefab.AssetGUID)
            || !enabled
            ? null : new(type, prefab);
    }
}
