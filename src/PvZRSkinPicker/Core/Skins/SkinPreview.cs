namespace PvZRSkinPicker.Skins;

using Il2CppReloaded.Data;

using UnityEngine;
using UnityEngine.AddressableAssets;

internal sealed record SkinPreview(
    AssetReferenceSprite Sprite,
    Vector2 Offset,
    float Scale)
{
    public static SkinPreview FromDefinition(PlantDefinition definition)
    {
        return new(
            definition.m_previewSprite,
            definition.m_previewSpriteOffset,
            definition.m_previewSpriteScale);
    }

    public static SkinPreview FromChinaDefinition(PlantDefinition definition)
    {
        return FromDefinition(definition) with
        {
            Sprite = definition.m_chinaPreviewSprite,
        };
    }
}
