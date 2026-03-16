// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#pragma warning disable

namespace System.Linq;

internal static class EnumerableExtensions
{
    public static IEnumerable<(int Index, TSource Item)> Index<TSource>(this IEnumerable<TSource> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return IndexIterator(source);
    }

    private static IEnumerable<(int Index, TSource Item)> IndexIterator<TSource>(IEnumerable<TSource> source)
    {
        int index = -1;
        foreach (TSource element in source)
        {
            checked
            {
                index++;
            }

            yield return (index, element);
        }
    }
}
