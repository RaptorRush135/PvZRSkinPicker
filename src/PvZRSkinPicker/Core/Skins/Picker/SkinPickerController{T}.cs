namespace PvZRSkinPicker.Skins.Picker;

using System.Diagnostics.Contracts;

using Il2CppReloaded.Services;

using PvZRSkinPicker.Almanac;
using PvZRSkinPicker.Almanac.UI;
using PvZRSkinPicker.Api;
using PvZRSkinPicker.Data;
using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.Skins.Picker.Selection;

internal sealed class SkinPickerController<T>
    where T : struct, Enum
{
    private readonly AlmanacSelection<T> selection;

    private readonly IReadOnlyDictionary<T, SkinPicker<T>> pickers;

    public SkinPickerController(
        AlmanacSelection<T> selection,
        IEnumerable<ISkinDataDefinition<T>> definitions,
        IReadOnlyDictionary<T, IReadOnlyList<Skin>> extraSkins,
        Action<T, Skin> onSelect)
    {
        ArgumentNullException.ThrowIfNull(selection);
        ArgumentNullException.ThrowIfNull(definitions);
        ArgumentNullException.ThrowIfNull(extraSkins);
        ArgumentNullException.ThrowIfNull(onSelect);

        this.selection = selection;
        this.pickers = definitions
            .Select(d => SkinPicker<T>.TryCreate(d, extraSkins.GetValueOrDefault(d.Type) ?? [], onSelect))
            .WhereNotNull()
            .ToDictionary(picker => picker.Type);
    }

    public void ApplySelections(SkinSelectionSet<T> selectionSet)
    {
        foreach (var (type, picker) in this.pickers)
        {
            if (selectionSet.Selections.TryGetValue(type, out SkinId? id))
            {
                picker.Select(id);
            }
            else
            {
                // TODO: Remove when skin deselection is implemented
                picker.ApplySelection();
            }
        }
    }

    public void Bind(ModButton button)
    {
        this.selection.SelectionChanged += type =>
        {
            bool typeHasPicker = this.pickers.ContainsKey(type);
            button.SetActive(typeHasPicker);
        };

        button.AddOnClick(this.CycleSkin);
    }

    public void CycleSkin()
    {
        if (this.pickers.TryGetValue(this.selection.Value, out var picker))
        {
            AudioServiceApi.PlayWithRandomPitch(FoleyType.LimbsPop);

            Skin skin = picker.Next();
            this.selection.Refresh();
            this.selection.SetName(skin.Name);
        }
    }

    [Pure]
    public SkinSelectionSet<T> GetSelections()
    {
        var selections = this.pickers
            .ToDictionary(
                pair => pair.Key,
                pair => pair.Value.GetSelectedSkin().Id);

        return new SkinSelectionSet<T>(selections);
    }
}
