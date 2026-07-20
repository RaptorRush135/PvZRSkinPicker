namespace PvZRSkinPicker.Unity.Resources;

using Il2CppInterop.Runtime.Injection;

using MelonLoader;

using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

[RegisterTypeInIl2Cpp]
internal sealed class InMemorySpriteProvider : ResourceProviderBase
{
    private readonly InMemoryResourceProvider<Sprite> provider = new();

    public InMemorySpriteProvider(IntPtr ptr)
        : base(ptr)
    {
    }

    public InMemorySpriteProvider()
        : base(ClassInjector.DerivedConstructorPointer<InMemorySpriteProvider>())
    {
        ClassInjector.DerivedConstructorBody(this);
    }

    public override string ProviderId => this.provider.ProviderId;

    public IResourceLocation AddAsset(Guid id, Sprite asset, out string key)
        => this.provider.AddAsset(id, asset, out key);

    public override bool CanProvide(Il2CppSystem.Type t, IResourceLocation location)
        => this.provider.CanProvide(location);

    public override Il2CppSystem.Type GetDefaultType(IResourceLocation location)
        => this.provider.GetDefaultType();

    public override void Provide(ProvideHandle provideHandle)
        => this.provider.Provide(provideHandle);
}
