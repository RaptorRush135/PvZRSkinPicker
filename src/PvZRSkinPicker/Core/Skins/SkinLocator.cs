namespace PvZRSkinPicker.Skins;

using Il2CppReloaded.Data;
using Il2CppReloaded.Services;

using PvZRSkinPicker.Extensions;

using UnityEngine.AddressableAssets;

internal static class SkinLocator
{
    public static IEnumerable<Skin> GetSkins(
        PlantDefinition definition,
        IPlatformService platformService)
    {
        IEnumerable<Skin?> skins =
        [
            TryCreateSkin(SkinType.Normal, definition.m_prefab),
            TryCreateSkin(SkinType.PreOrder, definition.m_preorderGameObject, platformService.PreOrderDLCAvailable),
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
        IEnumerable<Skin?> skins =
        [
            TryCreateSkin(SkinType.Normal, definition.m_prefab),
            TryCreateSkin(SkinType.Retro, definition.m_retroGameObject, platformService.RetroContentAvailable),
            TryCreateSkin(SkinType.Platform, definition.m_platformGameObject, platformService.PlatformContentAvailable),
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
