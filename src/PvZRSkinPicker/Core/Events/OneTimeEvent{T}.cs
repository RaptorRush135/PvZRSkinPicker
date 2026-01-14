namespace PvZRSkinPicker.Events;

using MelonLoader;

internal sealed class OneTimeEvent<T>(
    MelonLogger.Instance logger)
    where T : class
{
    private readonly MelonEvent<T> @event = new();

    private T? value;

    public OneTimeEvent()
        : this(Melon<Core>.Logger)
    {
    }

    public bool Disposed => this.@event.Disposed;

    public void Subscribe(Action<T> action)
    {
        if (this.Disposed)
        {
            return;
        }

        var lemonAction = new LemonAction<T>(action);

        if (this.value != null)
        {
            lemonAction.Invoke(this.value);
            return;
        }

        this.@event.Subscribe(lemonAction);
    }

    public void Invoke(T value)
    {
        if (this.value != null)
        {
            logger.Warning("One-time event was already invoked");
            return;
        }

        if (this.Disposed)
        {
            return;
        }

        this.value = value;
        this.@event.Invoke(value);
    }
}
