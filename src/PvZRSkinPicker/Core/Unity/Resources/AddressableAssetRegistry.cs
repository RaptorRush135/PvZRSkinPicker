namespace PvZRSkinPicker.Unity.Resources;

using System.Security.Cryptography;
using System.Text;

using Il2Cpp;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceProviders;

internal sealed class AddressableAssetRegistry
{
    private readonly ResourceLocationMap locator;

    private readonly InMemorySpriteProvider spriteProvider;

    private AddressableAssetRegistry(
        string id,
        ResourceLocationMap locator,
        InMemorySpriteProvider spriteProvider)
    {
        this.Id = id;
        this.locator = locator;
        this.spriteProvider = spriteProvider;
    }

    public string Id { get; }

    public static AddressableAssetRegistry Create(string id)
    {
        ArgumentNullException.ThrowIfNull(id);

        var locator = new ResourceLocationMap(id);

        Addressables.AddResourceLocator(
            locator.Cast<IResourceLocator>());

        var provider = new InMemorySpriteProvider();

        Addressables.ResourceManager.ResourceProviders
            .Cast<ListWithEvents<IResourceProvider>>()
            .Add(provider.Cast<IResourceProvider>());

        return new AddressableAssetRegistry(id, locator, provider);
    }

    public AssetReferenceSprite AddSprite(string key, Sprite sprite)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(sprite);

        sprite.hideFlags |= HideFlags.HideAndDontSave;

        var location = this.spriteProvider.AddAsset(this.KeyToGuid(key), sprite, out string assetKey);

        this.locator.Add(assetKey, location);

        return new AssetReferenceSprite(assetKey);
    }

    private Guid KeyToGuid(string key)
    {
        var data = Encoding.UTF8.GetBytes($"{this.Id}:{key}");
        var hash = SHA256.HashData(data);
        return new Guid(hash[..16]);
    }
}
