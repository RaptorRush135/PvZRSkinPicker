namespace PvZRSkinPicker.Skins.Picker;

using System.Diagnostics.Contracts;

using Microsoft.Extensions.Logging;

using PvZRSkinPicker.Data;
using PvZRSkinPicker.Skins;

using SolarApi;
using SolarApi.Collections.Extensions;

internal sealed class SkinPicker<T>
    where T : struct, Enum
{
    private static readonly ILogger<SkinPicker<T>> Logger
        = Solar<SkinPickerMod>.GetLogger<SkinPicker<T>>();

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
        IEnumerable<Skin> extraSkins,
        Action<T, Skin> onSelect)
    {
        var allSkins = definition.GetSkins().Concat(extraSkins);
        return TryCreate(definition.Type, allSkins, onSelect);
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

    public Skin Next() => this.ShiftSelection(1);

    public Skin Previous() => this.ShiftSelection(-1);

    public void Select(SkinId id)
    {
        Skin? selectedSkin = this.Skins.FirstOrDefault(s => s.Id == id);
        if (selectedSkin == null)
        {
            Logger.LogWarning("Skin with id '{SkinId}' not found", id.Id);
            return;
        }

        this.selectedIndex = this.Skins.IndexOf(selectedSkin);
        this.ApplySelection();
    }

    public Skin ApplySelection()
    {
        Skin selectedSkin = this.GetSelectedSkin();
        this.OnSelect(this.Type, selectedSkin);
        return selectedSkin;
    }

    [Pure]
    public Skin GetSelectedSkin() => this.Skins[this.selectedIndex];

    private Skin ShiftSelection(int offset)
    {
        this.selectedIndex = (this.selectedIndex + offset + this.Skins.Count) % this.Skins.Count;
        return this.ApplySelection();
    }
}
