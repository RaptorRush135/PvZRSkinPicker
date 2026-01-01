namespace PvZRSkinPicker.Api.Prefabs;

#pragma warning disable CA1822 // Mark members as static

using System.Collections.ObjectModel;

using PvZRSkinPicker.Skins;

internal sealed class PrefabResolver<T>
    where T : struct, Enum
{
    private readonly Dictionary<T, Skin> overridesMap = [];

    public PrefabResolver()
    {
        this.Overrides = this.overridesMap.AsReadOnly();
    }

    public ReadOnlyDictionary<T, Skin> Overrides { get; }

    public void SetOverride(T type, Skin skin)
    {
        this.overridesMap[type] = skin;
    }

    public bool EmulateSkinConditions(T type)
    {
        if (!this.overridesMap.TryGetValue(type, out var skin))
        {
            return false;
        }

        return SkinConditionEmulator.ApplyGameplayOverridesForSkinType(skin.Type);
    }

    public void ClearSkinConditions()
    {
        GameplayServiceApi.SetOverrides(null);
    }
}
