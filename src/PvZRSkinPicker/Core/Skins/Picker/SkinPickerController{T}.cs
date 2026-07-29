namespace PvZRSkinPicker.Skins.Picker;

using System.Diagnostics.Contracts;

using Il2CppReloaded.Services;

using PvZRSkinPicker.Almanac;
using PvZRSkinPicker.Almanac.UI;
using PvZRSkinPicker.Api;
using PvZRSkinPicker.Data;
using PvZRSkinPicker.Skins.Picker.Selection;

using SolarApi.Collections.Extensions;

internal sealed class SkinPickerController<T>
    where T : struct, Enum
{
    private readonly AlmanacSelection<T> selection;

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
        this.Pickers = definitions
            .Select(d => SkinPicker<T>.TryCreate(d, extraSkins.GetValueOrDefault(d.Type) ?? [], onSelect))
            .WhereNotNull()
            .ToDictionary(picker => picker.Type);
    }

    public IReadOnlyDictionary<T, SkinPicker<T>> Pickers { get; }

    public void ApplySelections(SkinSelectionSet<T> selectionSet)
    {
        foreach (var (type, picker) in this.Pickers)
        {
            if (selectionSet.Selections.TryGetValue(type, out SkinId? id))
            {
                picker.Select(id);
                continue;
            }

            // TODO: Remove when skin deselection is implemented
            picker.ApplySelection();
        }
    }

    public void Bind(ModButton button)
    {
        this.selection.SelectionChanged += type =>
        {
            if (!this.Pickers.TryGetValue(type, out var picker))
            {
                button.SetActive(false);
                return;
            }

            button.SetActive(true);
            this.RefreshName(picker, overrideUntilNextNameSet: true);
        };

        button.AddOnClick(this.CycleSkin);
    }

    public void CycleSkin()
    {
        if (this.Pickers.TryGetValue(this.selection.Value, out var picker))
        {
            AudioServiceApi.PlayWithRandomPitch(FoleyType.LimbsPop);

            picker.Next();
            this.selection.Refresh();
        }
    }

    public void RefreshName(bool overrideUntilNextNameSet)
    {
        if (this.Pickers.TryGetValue(this.selection.Value, out var picker))
        {
            this.RefreshName(picker, overrideUntilNextNameSet);
        }
    }

    [Pure]
    public SkinSelectionSet<T> GetSelections()
    {
        var selections = this.Pickers
            .ToDictionary(
                pair => pair.Key,
                pair => pair.Value.GetSelectedSkin().Id);

        return new SkinSelectionSet<T>(selections);
    }

    private void RefreshName(SkinPicker<T> picker, bool overrideUntilNextNameSet)
    {
        Skin skin = picker.GetSelectedSkin();
        if (skin.Id.Type != SkinType.Custom)
        {
            return;
        }

        if (overrideUntilNextNameSet)
        {
            this.selection.OverrideNextNameSet(skin.Name);
            return;
        }

        this.selection.SetName(skin.Name);
    }
}
