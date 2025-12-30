namespace PvZRSkinPicker.Extensions;

using System.Reflection;
using System.Runtime.InteropServices;

using Il2CppInterop.Common;

internal static class ReflectionExtensions
{
    public static MethodInfo GetMethodOrThrow(this Type type, string name, params Type[] types)
    {
        return type.GetMethod(name, types)
            ?? throw new MissingMethodException(type.FullName, name);
    }

    public static IntPtr GetIl2CppMethodPointer(this MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);

        FieldInfo field = Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method);
        var fieldValue = (IntPtr)field.GetValue(null)!;
        return Marshal.ReadIntPtr(fieldValue);
    }

    public static T ToIl2CppDelegate<T>(this MethodInfo method)
        where T : Delegate
    {
        ArgumentNullException.ThrowIfNull(method);

        IntPtr methodPtr = method.GetIl2CppMethodPointer();
        return Marshal.GetDelegateForFunctionPointer<T>(methodPtr);
    }
}
