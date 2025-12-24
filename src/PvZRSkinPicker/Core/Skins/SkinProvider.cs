namespace PvZRSkinPicker.Skins;

using Il2CppReloaded.Data;
using Il2CppReloaded.Services;

using PvZRSkinPicker.Extensions;

using UnityEngine.AddressableAssets;

internal static class SkinProvider
{
    public static IEnumerable<Skin> GetSkins(IPlatformService platformService, PlantDefinition definition)
    {
        IEnumerable<Skin?> skins =
        [
            TryCreate("Default", definition.m_prefab),
            TryCreate("Preorder", definition.m_preorderGameObject, platformService.PreOrderDLCAvailable),
            TryCreate("EasterEgg", definition.m_easterEggGameObject),
            TryCreate("Xmas", definition.m_decemberGameObject),
            TryCreate("China", definition.m_chinaGameObject),
        ];

        return skins.WhereNotNull();
    }

    public static IEnumerable<Skin> GetSkins(IPlatformService platformService, ZombieDefinition definition)
    {
        IEnumerable<Skin?> skins =
        [
            TryCreate("Default", definition.m_prefab),
            TryCreate("Platform", definition.m_platformGameObject, platformService.PlatformContentAvailable),
            TryCreate("EasterEgg", definition.m_easterEggGameObject),
            TryCreate("Xmas", definition.m_decemberGameObject),
            TryCreate("China", definition.m_chinaGameObject),
        ];

        return skins.WhereNotNull();
    }

    private static Skin? TryCreate(string name, AssetReferenceGameObject prefab, bool enabled = true)
    {
        return string.IsNullOrEmpty(prefab.AssetGUID)
            || !enabled
            ? null : new(name, prefab);
    }
}
