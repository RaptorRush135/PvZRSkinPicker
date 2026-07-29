namespace PvZRSkinPicker.Skins.Prefabs.Serialization;

using System.Reflection;

using Il2CppReloaded.Gameplay;

using Il2CppSource.Serialization;

using Microsoft.Extensions.Logging;

using PvZRSkinPicker.Skins.Prefabs;

using SolarApi;
using SolarApi.Hooks;

internal sealed class ReloadedDeserializePatch<TClass, T>
    where TClass : ReloadedObject
    where T : struct, Enum
{
    private readonly SkinOverrideResolver<T> skinOverrideResolver;

    private readonly Il2CppHook<ReloadedDeserializeDelegate> hook;

    private readonly ILogger<ReloadedDeserializePatch<TClass, T>> logger
        = Solar<SkinPickerMod>.GetLogger<ReloadedDeserializePatch<TClass, T>>();

    public ReloadedDeserializePatch(SkinOverrideResolver<T> skinOverrideResolver)
    {
        this.skinOverrideResolver = skinOverrideResolver;
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
        var spawnContext = TryGetSpawnContext();

        if (spawnContext == null)
        {
            InvokeOriginalMethod();
            return;
        }

        this.skinOverrideResolver.EmulateSkinConditions(spawnContext.Value);

        InvokeOriginalMethod();

        this.skinOverrideResolver.ClearSkinConditions();

        void InvokeOriginalMethod()
            => this.hook.Trampoline.Invoke(instance, reader, version, methodInfo);

        SpawnContext<T>? TryGetSpawnContext()
        {
            var readerInstance = ReloadedDeserializeHelper.GetBinaryReaderInstance(reader);
            int originalPos = readerInstance._position;

            try
            {
                return ReadSpawnContext(readerInstance);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to read SpawnContext");
                return null;
            }
            finally
            {
                readerInstance._position = originalPos;
            }
        }

        SpawnContext<T> ReadSpawnContext(ReloadedBinaryReader readerInstance)
        {
            var board = new Zombie(instance).mBoard;

            int row = ReloadedDeserializeHelper.ReadObject(reader, version, methodInfo).mRow;

            var type = ReloadedDeserializeHelper.ReadIntEnum<T>(readerInstance);

            return new(type, board, row);
        }
    }
}
