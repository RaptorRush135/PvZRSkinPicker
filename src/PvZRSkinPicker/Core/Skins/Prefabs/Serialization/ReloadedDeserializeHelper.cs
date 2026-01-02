namespace PvZRSkinPicker.Skins.Prefabs.Serialization;

using System.Reflection;
using System.Runtime.CompilerServices;

using Il2CppInterop.Runtime.Runtime;

using Il2CppReloaded.Gameplay;

using Il2CppSource.Serialization;

using PvZRSkinPicker.Extensions;

internal static class ReloadedDeserializeHelper
{
    private static readonly ReloadedDeserializeDelegate ObjectDeserialize
        = GetDeserializeMethod<ReloadedObject>().ToIl2CppDelegate<ReloadedDeserializeDelegate>();

    public static MethodInfo GetDeserializeMethod<T>()
        where T : ReloadedObject
    {
        return typeof(T).GetMethodOrThrow(
            nameof(ReloadedObject.Deserialize),
            typeof(ReloadedBinaryReader).MakeByRefType(),
            typeof(int));
    }

    public static ReloadedBinaryReader GetBinaryReaderInstance(IntPtr binaryReader)
    {
        // Need the original, not a copy
        // Does not work:
        // - new Il2CppObjectBase(reader).Cast<ReloadedBinaryReader>()
        // - IL2CPP.PointerToValueGeneric<ReloadedBinaryReader>(reader, false, false)
        // - new ReloadedBinaryReader(
        //      IL2CPP.il2cpp_value_box(Il2CppClassPointerStore<ReloadedBinaryReader>.NativeClassPtr, reader))
        // Source:
        // https://discord.com/channels/623153565053222947/754333645199900723/982590743438839818
        return new(binaryReader - Unsafe.SizeOf<Il2CppObject>());
    }

    public static void AdvanceReaderByBaseDeserialize(IntPtr binaryReader, int version, IntPtr methodInfo)
    {
        var dummyObject = new ReloadedObject();
        ObjectDeserialize.Invoke(dummyObject.Pointer, binaryReader, version, methodInfo);
    }

    public static T ReadIntEnum<T>(ReloadedBinaryReader binaryReader)
        where T : struct, Enum
    {
        int value = -1;
        binaryReader.Int32(ref value); // .Enum<T> crashes
        var result = (T)Enum.ToObject(typeof(T), value);
        if (!Enum.IsDefined(result))
        {
            throw new InvalidDataException(
                $"Value '{value}' is not a valid '{typeof(T).Name}'.");
        }

        return result;
    }
}
