namespace PvZRSkinPicker.Skins.Picker;

using PvZRSkinPicker.Almanac;
using PvZRSkinPicker.Almanac.UI;
using PvZRSkinPicker.Data;
using PvZRSkinPicker.Extensions;

internal sealed class SkinPickerController<T>
    where T : struct, Enum
{
    private readonly AlmanacSelection<T> selection;

    private readonly Dictionary<T, SkinPicker<T>> pickers;

    public SkinPickerController(
        AlmanacSelection<T> selection,
        IEnumerable<ISkinDataDefinition<T>> definitions,
        Action<T, Skin> onSelect)
    {
        ArgumentNullException.ThrowIfNull(selection);
        ArgumentNullException.ThrowIfNull(definitions);

        this.selection = selection;
        this.pickers = definitions
            .Select(d => SkinPicker<T>.TryCreate(d, onSelect))
            .WhereNotNull()
            .ToDictionary(picker => picker.Type);
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
            picker.Next();
            this.selection.Refresh();
        }
    }
}
