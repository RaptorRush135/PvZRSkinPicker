namespace PvZRSkinPicker.Skins.Prefabs;

using Microsoft.Extensions.Logging;

internal sealed class SpawnContextContainer<T>(
    ILogger<SpawnContextContainer<T>> logger)
    where T : struct, Enum
{
    private SpawnContext<T>? value;

    public SpawnContext<T>? Get() => this.value;

    public void Set(SpawnContext<T> value)
    {
        if (this.value != null)
        {
            this.Warning($"was already set ({value})");
        }

        this.value = value;
    }

    public void Clear()
    {
        if (this.value == null)
        {
            this.Warning("is already empty");
            return;
        }

        this.value = null;
    }

    public void Warning(string message)
    {
        logger.LogWarning("Context<{TypeName}> {Message}", typeof(T).Name, message);
    }
}
