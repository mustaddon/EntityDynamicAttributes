using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EntityDynamicAttributes;

internal static class Empty
{
    internal static readonly ReadOnlyDictionary<string, object> ReadOnlyDictionary = new(new Dictionary<string, object>());

    internal static readonly ReadOnlyCollection<string> ReadOnlyCollection = new(new HashSet<string>());

    internal static readonly ReadOnlyDictionary<string, Delegate> ReadOnlyDictionaryDelegate = new(new Dictionary<string, Delegate>());
}
