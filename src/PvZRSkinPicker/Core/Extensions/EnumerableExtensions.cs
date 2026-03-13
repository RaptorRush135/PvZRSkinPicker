namespace PvZRSkinPicker.Extensions;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(
        this IEnumerable<T?> source)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Where(x => x != null)!;
    }

    public static IEnumerable<T> WhereNotNull<T>(
        this IEnumerable<T?> source)
        where T : struct
    {
        ArgumentNullException.ThrowIfNull(source);

        return WhereNotNullIterator();

        IEnumerable<T> WhereNotNullIterator()
        {
            foreach (var item in source)
            {
                if (item.HasValue)
                {
                    yield return item.Value;
                }
            }
        }
    }

    public static int IndexOf<T>(this IEnumerable<T> source, T item)
    {
        ArgumentNullException.ThrowIfNull(source);

        int index = 0;
        foreach (var value in source)
        {
            if (EqualityComparer<T>.Default.Equals(value, item))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    public static Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(
        this IEnumerable<KeyValuePair<TKey, TElement>> pairs)
        where TKey : notnull
    {
        return new(pairs);
    }
}
