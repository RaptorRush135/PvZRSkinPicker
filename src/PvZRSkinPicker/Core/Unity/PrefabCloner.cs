namespace PvZRSkinPicker.Unity;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

internal static class PrefabCloner
{
    public static GameObject InstantiateInactiveFromPrefabAsset(AssetReferenceGameObject reference)
    {
        GameObject prefab = LoadPrefabReference(reference);

        GameObject clone = InstantiateInactiveFromPrefabAsset(prefab);

        Addressables.Release(reference.OperationHandle);

        return clone;
    }

    public static GameObject InstantiateInactiveFromPrefabAsset(GameObject prefab)
    {
        prefab.SetActive(false);

        var clone = Object.Instantiate(prefab);
        Object.DontDestroyOnLoad(clone);

        prefab.SetActive(true);

        return clone;
    }

    private static GameObject LoadPrefabReference(AssetReferenceGameObject reference)
    {
        AsyncOperationHandle handle = reference.OperationHandle.IsValid()
           ? reference.OperationHandle
           : reference.LoadAssetAsync<GameObject>();

        if (!handle.IsDone)
        {
            handle.WaitForCompletion();
        }

        return handle.Result.Cast<GameObject>();
    }
}
