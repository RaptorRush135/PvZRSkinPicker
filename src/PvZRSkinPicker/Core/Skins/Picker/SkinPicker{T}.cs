namespace PvZRSkinPicker.Skins.Picker;

using System.Diagnostics.Contracts;

using Il2CppReloaded.Services;

using PvZRSkinPicker.Api;
using PvZRSkinPicker.Data;
using PvZRSkinPicker.Skins;

internal sealed class SkinPicker<T>
    where T : struct, Enum
{
    private int selectedIndex;

    private SkinPicker(
        T type,
        IReadOnlyList<Skin> skins,
        Action<T, Skin> onSelect)
    {
        this.Type = type;
        this.Skins = skins;
        this.OnSelect = onSelect;
    }

    public T Type { get; }

    public IReadOnlyList<Skin> Skins { get; }

    private Action<T, Skin> OnSelect { get; }

    [Pure]
    public static SkinPicker<T>? TryCreate(
        ISkinDataDefinition<T> definition,
        Action<T, Skin> onSelect)
    {
        return TryCreate(definition.Type, definition.GetSkins(), onSelect);
    }

    [Pure]
    public static SkinPicker<T>? TryCreate(
        T type,
        IEnumerable<Skin> skins,
        Action<T, Skin> onSelect)
    {
        var skinArray = skins.ToArray();
        return skinArray.Length <= 1
            ? null
            : new SkinPicker<T>(type, skinArray, onSelect);
    }

    public void Next()
    {
        this.selectedIndex = (this.selectedIndex + 1) % this.Skins.Count;
        this.ApplySelection();
        AudioServiceApi.PlayWithRandomPitch(FoleyType.LimbsPop);
    }

    public void ApplySelection()
    {
        Skin selectedSkin = this.Skins[this.selectedIndex];
        this.OnSelect(this.Type, selectedSkin);
    }
}
