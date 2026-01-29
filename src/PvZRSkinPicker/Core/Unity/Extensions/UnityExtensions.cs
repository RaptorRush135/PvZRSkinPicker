namespace PvZRSkinPicker.Unity.Extensions;

using System.Diagnostics.CodeAnalysis;

using UnityEngine;
using UnityEngine.AddressableAssets;

using UnityObject = UnityEngine.Object;

internal static class UnityExtensions
{
    private const string UnityNullJustification =
        "UnityEngine.Object does not support ?. or ?? for detached objects; explicit null check required.";

    [SuppressMessage(
        "Style",
        "IDE0029:Use coalesce expression",
        Justification = UnityNullJustification)]
    [SuppressMessage(
        "Roslynator",
        "RCS1084:Use coalesce expression instead of conditional expression",
        Justification = UnityNullJustification)]
    public static T? Ref<T>(this T? obj)
        where T : UnityObject
    {
        return obj != null ? obj : null;
    }

    public static AssetReferenceGameObject ToAssetReference(this GameObject obj)
    {
        var handle = Addressables.ResourceManager.CreateCompletedOperation(obj, string.Empty);
        return new AssetReferenceGameObject(string.Empty)
        {
            OperationHandle = handle,
        };
    }

    extension(GameObject)
    {
        public static GameObject FindOrThrow(string name)
            => GameObject.Find(name).Ref()
            ?? throw new InvalidOperationException(
                $"'{name}' was not found in the scene.");
    }
}
