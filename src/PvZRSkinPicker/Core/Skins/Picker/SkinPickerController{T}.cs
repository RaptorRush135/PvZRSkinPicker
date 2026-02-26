namespace PvZRSkinPicker.Skins.Picker;

using Il2CppTekly.DataModels.Binders;

using PvZRSkinPicker.Almanac;
using PvZRSkinPicker.Almanac.UI;
using PvZRSkinPicker.Data;
using PvZRSkinPicker.Extensions;

internal sealed class SkinPickerController<T>
    where T : struct, Enum
{
    private readonly StringBinder nameBinder;

    private readonly AlmanacSelection<T> selection;

    private readonly Dictionary<T, SkinPicker<T>> pickers;

    public SkinPickerController(
        StringBinder nameBinder,
        AlmanacSelection<T> selection,
        IEnumerable<ISkinDataDefinition<T>> definitions,
        IReadOnlyDictionary<T, IReadOnlyList<Skin>> extraSkins,
        Action<T, Skin> onSelect)
    {
        ArgumentNullException.ThrowIfNull(nameBinder);
        ArgumentNullException.ThrowIfNull(selection);
        ArgumentNullException.ThrowIfNull(definitions);
        ArgumentNullException.ThrowIfNull(extraSkins);
        ArgumentNullException.ThrowIfNull(onSelect);

        this.nameBinder = nameBinder;
        this.selection = selection;
        this.pickers = definitions
            .Select(d => SkinPicker<T>.TryCreate(d, extraSkins.GetValueOrDefault(d.Type) ?? [], onSelect))
            .WhereNotNull()
            .ToDictionary(picker => picker.Type);
    }

    public void ApplySelections()
    {
        foreach (var picker in this.pickers.Values)
        {
            picker.ApplySelection();
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
            Skin skin = picker.Next();
            this.selection.Refresh();
            this.nameBinder.m_text.text = skin.Name;
        }
    }
}
