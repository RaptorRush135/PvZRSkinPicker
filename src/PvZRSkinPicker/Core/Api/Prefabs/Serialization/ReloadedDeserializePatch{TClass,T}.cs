namespace PvZRSkinPicker.Api.Prefabs.Serialization;

using System.Reflection;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.NativeUtils;

internal sealed class ReloadedDeserializePatch<TClass, T>
    where TClass : ReloadedObject
    where T : struct, Enum
{
    private readonly PrefabResolver<T> prefabResolver;

    private readonly Il2CppHook<ReloadedDeserializeDelegate> hook;

    public ReloadedDeserializePatch(PrefabResolver<T> prefabResolver)
    {
        this.prefabResolver = prefabResolver;
        this.hook = this.CreateDeserializeHook();
    }

    public IFunctionHook Initialize()
    {
        this.hook.Attach();
        return this.hook;
    }

    private Il2CppHook<ReloadedDeserializeDelegate> CreateDeserializeHook()
    {
        MethodInfo targetMethod = ReloadedDeserializeHelper.GetDeserializeMethod<TClass>();
        return Il2CppHook<ReloadedDeserializeDelegate>.Create(targetMethod, this.DeserializeDetour);
    }

    private void DeserializeDetour(
        IntPtr instance,
        IntPtr reader,
        int version,
        IntPtr methodInfo)
    {
        var readerInstance = ReloadedDeserializeHelper.GetBinaryReaderInstance(reader);

        int originalPos = readerInstance._position;

        ReloadedDeserializeHelper.AdvanceReaderByBaseDeserialize(reader, version, methodInfo);

        var type = ReloadedDeserializeHelper.ReadIntEnum<T>(readerInstance);

        readerInstance._position = originalPos;

        bool needsClear = this.prefabResolver.EmulateSkinConditions(type);

        this.hook.Trampoline.Invoke(instance, reader, version, methodInfo);

        if (needsClear)
        {
            this.prefabResolver.ClearSkinConditions();
        }
    }
}
