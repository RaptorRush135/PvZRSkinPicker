namespace PvZRSkinPicker.Almanac;

using Il2CppReloaded.Data;

using Il2CppTekly.DataModels.Binders;
using Il2CppTekly.DataModels.Models;

using MelonLoader;

using PvZRSkinPicker.Almanac.UI;

internal sealed class AlmanacSelection<T>
    where T : struct, Enum
{
    private readonly StringValueModel selectedModel;

    private readonly StringBinder nameBinder;

    private AlmanacSelection(StringValueModel selectedModel, StringBinder nameBinder)
    {
        ArgumentNullException.ThrowIfNull(selectedModel);
        ArgumentNullException.ThrowIfNull(nameBinder);

        selectedModel.Subscribe(
            (Action<string>)this.Changed);

        this.selectedModel = selectedModel;
        this.nameBinder = nameBinder;
    }

    public event Action<T>? SelectionChanged;

    public T Value { get; private set; }

    public static AlmanacSelection<T> Create(AlmanacEntryType type, StringValueModel selectedModel)
    {
        StringBinder nameBinder = AlmanacUI.GetSelectedItemNameBinder(type);
        return new(selectedModel, nameBinder);
    }

    public void Refresh()
    {
        this.selectedModel.Emit(
            this.selectedModel.Value);
    }

    public void SetName(string name)
    {
        this.nameBinder.m_text.text = name;
    }

    private void Changed(string value)
    {
        if (!int.TryParse(value, out var typeIndex))
        {
            Melon<Core>.Logger.Warning(
                $"Expected integer in selection, but received '{value}'");

            return;
        }

        this.Value = (T)Enum.ToObject(typeof(T), typeIndex);

        this.SelectionChanged?.Invoke(this.Value);
    }
}
