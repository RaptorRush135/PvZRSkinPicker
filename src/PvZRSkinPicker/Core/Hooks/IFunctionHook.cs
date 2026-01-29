namespace PvZRSkinPicker.Hooks;

internal interface IFunctionHook
{
    bool IsHooked { get; }

    void Attach();

    void Detach();
}
