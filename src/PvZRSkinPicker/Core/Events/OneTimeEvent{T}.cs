namespace PvZRSkinPicker.Events;

using System.Diagnostics.CodeAnalysis;

using MelonLoader;

internal sealed class OneTimeEvent<T>(
    MelonLogger.Instance logger)
{
    private readonly MelonEvent<T> @event = new();

    private T? value;

    public OneTimeEvent()
        : this(Melon<Core>.Logger)
    {
    }

    public bool Disposed => this.@event.Disposed;

    [MemberNotNullWhen(true, nameof(value))]
    public bool Invoked { get; private set; }

    public void Subscribe(Action<T> action)
    {
        if (this.Disposed)
        {
            return;
        }

        if (this.Invoked)
        {
            action.Invoke(this.value);
            return;
        }

        this.@event.Subscribe(new(action));
    }

    public void Invoke(T value)
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

        this.value = value;
        this.Invoked = true;
        this.@event.Invoke(value);
    }
}
