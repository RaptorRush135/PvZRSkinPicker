namespace PvZRSkinPicker.Skins.Picker;

using PvZRSkinPicker.Almanac;
using PvZRSkinPicker.Almanac.UI;
using PvZRSkinPicker.Data;
using PvZRSkinPicker.Extensions;

internal sealed class SkinPickerController<T, TPicker>
    where T : struct, Enum
    where TPicker : SkinPicker<T>, new()
{
    private readonly AlmanacSelection<T> selection;

    private readonly Dictionary<T, TPicker> pickers;

    public SkinPickerController(
        AlmanacSelection<T> selection,
        IEnumerable<ISkinDataDefinition<T>> definitions)
    {
        ArgumentNullException.ThrowIfNull(selection);
        ArgumentNullException.ThrowIfNull(definitions);

        this.selection = selection;
        this.pickers = definitions
            .Select(SkinPicker<T>.TryCreate<TPicker>)
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
