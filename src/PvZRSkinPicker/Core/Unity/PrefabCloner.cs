namespace PvZRSkinPicker.Unity;

using MelonLoader;

using PvZRSkinPicker.Unity.Extensions;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

internal static class PrefabCloner
{
    public static GameObject InstantiateInactiveFromPrefabAsset(
        AssetReferenceGameObject reference,
        bool expectLoaded = false)
    {
        ArgumentNullException.ThrowIfNull(reference);

        GameObject prefab = LoadPrefabReference(reference, expectLoaded);

        GameObject clone = InstantiateInactiveFromPrefabAsset(prefab);

        Addressables.Release(reference.OperationHandle);

        return clone;
    }

    public static GameObject InstantiateInactiveFromPrefabAsset(
        GameObject prefab)
    {
        ArgumentNullException.ThrowIfNull(prefab.Ref());

        prefab.SetActive(false);

        var clone = Object.Instantiate(prefab);
        clone.AddComponent<RequiresActivationMarker>();
        Object.DontDestroyOnLoad(clone);

        prefab.SetActive(true);

        return clone;
    }

    private static GameObject LoadPrefabReference(
        AssetReferenceGameObject reference,
        bool expectLoaded)
    {
        AsyncOperationHandle handle = reference.OperationHandle.IsValid()
           ? reference.OperationHandle
           : reference.LoadAssetAsync<GameObject>();

        bool isLoaded = handle.IsDone;
        if (!isLoaded)
        {
            handle.WaitForCompletion();
        }

        if (handle.Result == null)
        {
            throw new InvalidOperationException(
                $"Failed to load prefab from AssetReference. AssetGUID: '{reference.AssetGUID}'.");
        }

        var result = handle.Result.Cast<GameObject>();

        if (expectLoaded && !isLoaded)
        {
            Melon<Core>.Logger.Warning($"Expected prefab '{result.name}' to be loaded");
        }

        return result;
    }
}
