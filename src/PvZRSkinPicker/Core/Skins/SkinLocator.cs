namespace PvZRSkinPicker.Skins;

using Il2CppReloaded.Data;
using Il2CppReloaded.Services;

using Il2CppTekly.Localizations;

using PvZRSkinPicker.Skins.Picker;

using SolarApi.Collections.Extensions;

using UnityEngine.AddressableAssets;

internal sealed class SkinLocator(
    IPlatformService platformService,
    ILocalizer localizer)
{
    public IEnumerable<Skin> GetSkins(PlantDefinition definition)
    {
        var type = definition.SeedType;
        if (!type.IsSkinPickerSupported())
        {
            return [];
        }

        string name = this.Localize(definition.PlantName);

        var defaultPreview = SkinPreview.FromDefinition(definition);
        var chinaPreview = SkinPreview.FromChinaDefinition(definition);

        IEnumerable<Skin?> skins =
        [
            TryCreateSkin(SkinType.Normal, definition.m_prefab),
            TryCreateSkin(SkinType.PreOrderPlant, definition.m_preorderGameObject, platformService.PreOrderDLCAvailable),
            TryCreateSkin(SkinType.China, definition.m_chinaGameObject, image: definition.m_chinaPlantImage, preview: chinaPreview),
            TryCreateSkin(SkinType.EasterEgg, definition.m_easterEggGameObject),
            TryCreateSkin(SkinType.December, definition.m_decemberGameObject),
        ];

        return skins.WhereNotNull();

        Skin? TryCreateSkin(
            SkinType skinType,
            AssetReferenceGameObject prefab,
            bool enabled = true,
            AssetReferenceSprite? image = null,
            SkinPreview? preview = null)
        {
            return SkinLocator.TryCreateSkin(
                name, skinType, prefab, enabled, image ?? definition.m_plantImage, preview ?? defaultPreview);
        }
    }

    public IEnumerable<Skin> GetSkins(ZombieDefinition definition)
    {
        var type = definition.ZombieType;

        if (!type.IsSkinPickerSupported())
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
            return SkinLocator.TryCreateSkin(
                name, skinType, prefab, enabled, definition.m_previewSprite, preview: null);
        }
    }

    private static Skin? TryCreateSkin(
        string name,
        SkinType skinType,
        AssetReferenceGameObject prefab,
        bool enabled,
        AssetReferenceSprite image,
        SkinPreview? preview)
    {
        return string.IsNullOrEmpty(prefab.AssetGUID) || !enabled
            ? null : Skin.Create(name, skinType, prefab, image, preview);
    }

    private string Localize(string name) => localizer.Localize($"${name}");
}
