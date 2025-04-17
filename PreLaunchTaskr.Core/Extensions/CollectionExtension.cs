using System.Collections.Generic;

namespace PreLaunchTaskr.Core.Extensions;

public static class CollectionExtension
{
    public static TCollection AddAll<TCollection, T>(this TCollection collection, IEnumerable<T> items) where TCollection : ICollection<T>
    {
        foreach (T item in items)
        {
            collection.Add(item);
        }
        return collection;
    }
}
