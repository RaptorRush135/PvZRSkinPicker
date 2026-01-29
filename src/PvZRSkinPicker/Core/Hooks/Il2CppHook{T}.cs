namespace PvZRSkinPicker.Hooks;

using System.Reflection;
using System.Runtime.InteropServices;

using MelonLoader.NativeUtils;

using PvZRSkinPicker.Extensions;

internal sealed class Il2CppHook<T> : IFunctionHook
    where T : Delegate
{
    private readonly NativeHook<T> nativeHook;

    private readonly GCHandle detourHandle;

    private Il2CppHook(IntPtr targetPtr, IntPtr detourPtr, T detour)
    {
        ArgumentNullException.ThrowIfNull(detour);

        this.nativeHook = new NativeHook<T>(targetPtr, detourPtr);
        this.detourHandle = GCHandle.Alloc(detour);
    }

    public bool IsHooked => this.nativeHook.IsHooked;

    public T Trampoline => this.nativeHook.Trampoline;

    public static Il2CppHook<T> Create(MethodInfo method, T detour)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(detour);

        IntPtr targetPtr = method.GetIl2CppMethodPointer();
        IntPtr detourPtr = Marshal.GetFunctionPointerForDelegate(detour);

        return new Il2CppHook<T>(targetPtr, detourPtr, detour);
    }

    public void Attach() => this.nativeHook.Attach();

    public void Detach()
    {
        this.nativeHook.Detach();

        if (this.detourHandle.IsAllocated)
        {
            this.detourHandle.Free();
        }
    }
}
