using System.Collections;
using System.Collections.Generic;

namespace EntityDynamicAttributes;

internal class ReadOnlyCollection<T> : IReadOnlyCollection<T>
{
    public ReadOnlyCollection(ICollection<T> source)
    {
        _source = source;
    }

    private readonly ICollection<T> _source;

    public int Count => _source.Count;
    public bool Contains(T item) => _source.Contains(item);
    public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _source.GetEnumerator();
}
