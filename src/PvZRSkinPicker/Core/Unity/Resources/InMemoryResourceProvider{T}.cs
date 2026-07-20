namespace PvZRSkinPicker.Unity.Resources;

using Il2CppInterop.Runtime;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

internal sealed class InMemoryResourceProvider<T>
    where T : UnityEngine.Object
{
    private readonly Dictionary<string, T> assetMap = [];

    public string ProviderId => field ??= this.GetType().FullName!;

    public IResourceLocation AddAsset(Guid id, T asset, out string key)
    {
        key = id.ToString();
        this.assetMap.Add(key, asset);
        return new ResourceLocationBase(key, key, this.ProviderId, Il2CppType.Of<T>())
            .Cast<IResourceLocation>();
    }

    public bool CanProvide(IResourceLocation location)
    {
        return location?.ProviderId == this.ProviderId
            && this.assetMap.ContainsKey(location.InternalId);
    }

    public Il2CppSystem.Type GetDefaultType()
        => Il2CppType.Of<T>();

    public void Provide(ProvideHandle provideHandle)
    {
        string key = provideHandle.Location.InternalId;
        if (!this.assetMap.TryGetValue(key, out var sprite))
        {
            var exception = new InvalidKeyException(
                (Il2CppSystem.String)key, this.GetDefaultType());

            provideHandle.Complete((T?)null, false, exception);
            return;
        }

        provideHandle.Complete(sprite, true, null);
    }
}
