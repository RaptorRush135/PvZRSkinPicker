// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#pragma warning disable

namespace System.Collections.Generic;

using System.Collections.ObjectModel;

internal static class CollectionExtensions
{
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
        => new ReadOnlyDictionary<TKey, TValue>(dictionary);
}
