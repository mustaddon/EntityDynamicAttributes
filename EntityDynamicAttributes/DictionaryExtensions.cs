using System;
using System.Collections.Generic;

namespace EntityDynamicAttributes;

internal static class DictionaryExtensions
{
    public static Dictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TKey : notnull
    {
        if (dictionary.ContainsKey(key))
            dictionary[key] = value;
        else
            dictionary.Add(key, value);

        return dictionary;
    }

    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var value))
            return value;

        value = valueFactory(key);

        dictionary.Add(key, value);

        return value;
    }

}
