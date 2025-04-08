using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.Core.Extensions;

public static class CollectionExtension
{
    public static TCollection AddAll<TCollection, T>(this TCollection collection, ICollection<T> items) where TCollection : ICollection<T>
    {
        foreach (T item in items)
        {
            collection.Add(item);
        }
        return collection;
    }
}
