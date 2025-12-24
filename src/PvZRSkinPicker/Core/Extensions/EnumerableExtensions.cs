namespace PvZRSkinPicker.Extensions;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(
        this IEnumerable<T?> source)
        where T : class
        => source.Where(x => x != null)!;
}
