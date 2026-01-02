namespace PvZRSkinPicker.Skins.Prefabs.Serialization;

using System.Runtime.InteropServices;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void ReloadedDeserializeDelegate(
    IntPtr instance,
    IntPtr reader,
    int version,
    IntPtr methodInfo);
