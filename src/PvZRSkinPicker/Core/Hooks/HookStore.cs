namespace PvZRSkinPicker.Hooks;

internal sealed class HookStore
{
    private readonly List<IFunctionHook> hooks = [];

    public void Add(IFunctionHook hook)
    {
        ArgumentNullException.ThrowIfNull(hook);
        this.hooks.Add(hook);
    }

    public void Add(params ReadOnlySpan<IFunctionHook> hooks)
    {
        foreach (var hook in hooks)
        {
            this.Add(hook);
        }
    }

    public void DetachAll()
    {
        this.hooks.ForEach(h => h.Detach());
        this.hooks.Clear();
    }
}
