namespace PvZRSkinPicker.NativeUtils;

internal interface IFunctionHook
{
    bool IsHooked { get; }

    void Attach();

    void Detach();
}
