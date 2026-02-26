namespace PvZRSkinPicker.Skins;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

using Il2CppTekly.Localizations;

using PvZRSkinPicker.Extensions;

using UnityEngine.AddressableAssets;

internal sealed class SkinLocator(
    IPlatformService platformService,
    ILocalizer localizer)
{
    public IEnumerable<Skin> GetSkins(PlantDefinition definition)
    {
        // TODO: Filter to only in almanac?
        var type = definition.SeedType;
        string name = this.Localize(definition.PlantName);

        IEnumerable<Skin?> skins =
        [
            TryCreateSkin(SkinType.Normal, definition.m_prefab),
            TryCreateSkin(SkinType.PreOrderPlant, definition.m_preorderGameObject, platformService.PreOrderDLCAvailable),
            TryCreateSkin(SkinType.China, definition.m_chinaGameObject),
            TryCreateSkin(SkinType.EasterEgg, definition.m_easterEggGameObject),
            TryCreateSkin(SkinType.December, definition.m_decemberGameObject),
        ];

        return skins.WhereNotNull();

        Skin? TryCreateSkin(
            SkinType skinType,
            AssetReferenceGameObject prefab,
            bool enabled = true)
        {
            return SkinLocator.TryCreateSkin(type, name, skinType, prefab, enabled);
        }
    }

    public IEnumerable<Skin> GetSkins(ZombieDefinition definition)
    {
        var type = definition.ZombieType;

        // "DuckyTube" uses the skins of "Normal", so do not scan for skins
        if (type == ZombieType.DuckyTube)
        {
            return [];
        }

        string name = this.Localize(definition.ZombieName);

        IEnumerable<Skin?> skins =
        [
            TryCreateSkin(SkinType.Normal, definition.m_prefab),
            TryCreateSkin(SkinType.RetroZombie, definition.m_retroGameObject, platformService.RetroContentAvailable),
            TryCreateSkin(SkinType.PlatformZombie, definition.m_platformGameObject, platformService.PlatformContentAvailable),
            TryCreateSkin(SkinType.China, definition.m_chinaGameObject),
            TryCreateSkin(SkinType.EasterEgg, definition.m_easterEggGameObject),
            TryCreateSkin(SkinType.December, definition.m_decemberGameObject),
        ];

        return skins.WhereNotNull();

        Skin? TryCreateSkin(
            SkinType skinType,
            AssetReferenceGameObject prefab,
            bool enabled = true)
        {
            return SkinLocator.TryCreateSkin(type, name, skinType, prefab, enabled);
        }
    }

    private static Skin? TryCreateSkin<T>(
        T type,
        string name,
        SkinType skinType,
        AssetReferenceGameObject prefab,
        bool enabled)
        where T : struct, Enum
    {
        return string.IsNullOrEmpty(prefab.AssetGUID) || !enabled
            ? null : new VanillaSkin<T>(type, name, skinType, prefab);
    }

    private string Localize(string name) => localizer.Localize($"${name}");
}
