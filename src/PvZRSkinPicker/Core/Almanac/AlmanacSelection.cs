namespace PvZRSkinPicker.Almanac;

using Il2CppTekly.DataModels.Models;

using MelonLoader;

internal sealed class AlmanacSelection<T>
    where T : struct, Enum
{
    private readonly StringValueModel selectedModel;

    public AlmanacSelection(StringValueModel selectedModel)
    {
        ArgumentNullException.ThrowIfNull(selectedModel);

        selectedModel.Subscribe(
            (Action<string>)this.Changed);

        this.selectedModel = selectedModel;
    }

    public event Action<T>? SelectionChanged;

    public T Value { get; private set; }

    public void Refresh()
    {
        this.selectedModel.Emit(
            this.selectedModel.Value);
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
