namespace PvZRSkinPicker.Extensions;

using CppCollections = Il2CppSystem.Collections.Generic;

internal static class CppCollectionsExtensions
{
    public static IEnumerable<T> AsEnumerable<T>(
        this CppCollections.IReadOnlyList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);

        return AsEnumerableIterator();

        IEnumerable<T> AsEnumerableIterator()
        {
            var collection = list.Cast<CppCollections.IReadOnlyCollection<T>>();
            int initialCount = collection.Count;

            for (int i = 0; i < collection.Count; i++)
            {
                if (collection.Count != initialCount)
                {
                    throw new InvalidOperationException(
                        "Collection was modified. " +
                        $"Initial count: {initialCount}, Current count: {collection.Count}.");
                }

                yield return list[i];
            }
        }
    }
}
