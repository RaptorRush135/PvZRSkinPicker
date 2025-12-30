namespace PvZRSkinPicker.Api.Prefabs.Zombies;

using System.Reflection;
using System.Runtime.InteropServices;

using Il2CppInterop.Runtime.Runtime;

using Il2CppReloaded.Gameplay;

using Il2CppSource.Serialization;

using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.NativeUtils;

internal static class ZombieDeserializePatch
{
    private static readonly DeserializeDelegate ReloadedObjectDeserialize = CreateReloadedObjectDeserializeDelegate();

    private static readonly Il2CppHook<DeserializeDelegate> Hook = CreateZombieDeserializeHook();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void DeserializeDelegate(
        IntPtr instance,
        IntPtr reader,
        int version,
        IntPtr methodInfo);

    public static IFunctionHook Initialize()
    {
        Hook.Attach();
        return Hook;
    }

    private static Il2CppHook<DeserializeDelegate> CreateZombieDeserializeHook()
    {
        MethodInfo targetMethod = typeof(Zombie)
            .GetMethodOrThrow(
                nameof(Zombie.Deserialize),
                typeof(ReloadedBinaryReader).MakeByRefType(),
                typeof(int));

        return Il2CppHook<DeserializeDelegate>.Create(targetMethod, DeserializeDetour);
    }

    private static DeserializeDelegate CreateReloadedObjectDeserializeDelegate()
    {
        MethodInfo targetMethod = typeof(ReloadedObject)
            .GetMethodOrThrow(
                nameof(ReloadedObject.Deserialize),
                typeof(ReloadedBinaryReader).MakeByRefType(),
                typeof(int));

        return targetMethod.ToIl2CppDelegate<DeserializeDelegate>();
    }

    private static unsafe void DeserializeDetour(
        IntPtr instance,
        IntPtr reader,
        int version,
        IntPtr methodInfo)
    {
        // Need the original, not a copy
        // Does not work:
        // - new Il2CppObjectBase(reader).Cast<ReloadedBinaryReader>()
        // - IL2CPP.PointerToValueGeneric<ReloadedBinaryReader>(reader, false, false)
        // - new ReloadedBinaryReader(
        //      IL2CPP.il2cpp_value_box(Il2CppClassPointerStore<ReloadedBinaryReader>.NativeClassPtr, reader))
        // Source:
        // https://discord.com/channels/623153565053222947/754333645199900723/982590743438839818
        var readerInstance = new ReloadedBinaryReader(reader - sizeof(Il2CppObject));
        int originalPos = readerInstance._position;

        var dummyObject = new ReloadedObject();
        ReloadedObjectDeserialize.Invoke(dummyObject.Pointer, reader, version, methodInfo);

        ZombieType zombieType = ReadZombieType();

        // This is safe, position is the only mutable state
        readerInstance._position = originalPos;

        bool needsClear = ZombiePrefabResolver.EmulateSkinConditions(zombieType);

        Hook.Trampoline(instance, reader, version, methodInfo);

        if (needsClear)
        {
            ZombiePrefabResolver.ClearGameplayOverrides();
        }

        ZombieType ReadZombieType()
        {
            var zombieType = (int)ZombieType.Invalid;
            readerInstance.Int32(ref zombieType); // .Enum<ZombieType> crashes
            return (ZombieType)zombieType;
        }
    }
}
