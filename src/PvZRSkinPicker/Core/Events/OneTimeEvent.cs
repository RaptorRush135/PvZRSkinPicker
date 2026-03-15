namespace PvZRSkinPicker.Events;

using MelonLoader;

internal sealed class OneTimeEvent(
    MelonLogger.Instance logger)
{
    private readonly MelonEvent @event = new();

    public OneTimeEvent()
        : this(Melon<Core>.Logger)
    {
    }

    public bool Disposed => this.@event.Disposed;

    public bool Invoked { get; private set; }

    public void Subscribe(Action action)
    {
        if (this.Disposed)
        {
            return;
        }

        if (this.Invoked)
        {
            action.Invoke();
            return;
        }

        this.@event.Subscribe(new(action));
    }

    public void Invoke()
    {
        if (this.Invoked)
        {
            logger.Warning("One-time event was already invoked");
            return;
        }

        if (this.Disposed)
        {
            return;
        }

        this.Invoked = true;
        this.@event.Invoke();
    }
}
