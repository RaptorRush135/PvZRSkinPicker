namespace PvZRSkinPicker.Unity;

using MelonLoader;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

internal static class PrefabCloner
{
    public static GameObject InstantiateInactiveFromPrefabAsset(
        AssetReferenceGameObject reference,
        bool expectLoaded = false)
    {
        GameObject prefab = LoadPrefabReference(reference, expectLoaded);

        GameObject clone = InstantiateInactiveFromPrefabAsset(prefab);

        Addressables.Release(reference.OperationHandle);

        return clone;
    }

    public static GameObject InstantiateInactiveFromPrefabAsset(
        GameObject prefab)
    {
        prefab.SetActive(false);

        var clone = Object.Instantiate(prefab);
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

        var result = handle.Result.Cast<GameObject>();

        if (expectLoaded && !isLoaded)
        {
            Melon<Core>.Logger.Warning($"Expected prefab '{result.name}' to be loaded");
        }

        return result;
    }
}
