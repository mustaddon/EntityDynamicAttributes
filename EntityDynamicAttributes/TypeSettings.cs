using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityDynamicAttributes;

internal class TypeSettings<TEntity>
{
    public TypeSettings(TypeConfig<TEntity> config)
    {
        AddProperties(config);
    }

    public readonly Dictionary<string, PropertySettings<TEntity>> Properties = new();

    void AddProperties(TypeConfig<TEntity> config)
    {
        var deepDeps = config.Properties.Values
            .Where(x => !x.IsEmpty)
            .ToDictionary(x => x.Path, x => new HashSet<string>(GetDeepDeps(new HashSet<string> { x.Path }, config.Properties)
                .Where(xx => xx.Count > 1)
                .SelectMany(xx => xx)
                .Where(xx => xx != x.Path)));

        var sortedProps = SortProps(deepDeps
            .SelectMany(x => x.Value)
            .Where(x => !config.Properties.ContainsKey(x))
            .Distinct()
            .Select(x => new PropertyConfig<TEntity>(x, typeof(object)))
            .Concat(config.Properties.Values)
            .ToDictionary(x => x.Path));

        deepDeps = sortedProps.Keys.Where(deepDeps.ContainsKey).ToDictionary(x => x, x => deepDeps[x]);

        foreach (var x in sortedProps.Values)
            Properties.Add(x.Path, new PropertySettings<TEntity>(x, deepDeps));
    }

    IEnumerable<HashSet<string>> GetDeepDeps(HashSet<string> current, Dictionary<string, PropertyConfig<TEntity>> configs)
    {
        if (!configs.TryGetValue(current.Last(), out var cfg) || cfg.Dependencies.Count == 0)
            yield return current;
        else
            foreach (var kvp in cfg.Dependencies)
            {
                if (kvp.Key == cfg.Path)
                    continue;

                var next = new HashSet<string>(current);

                if (!next.Add(kvp.Key))
                    throw new Exception($"[{typeof(TEntity).FullName}] Сircular dependencies found: {string.Join(" > ", current)} > {kvp.Key}");

                if (!kvp.Value)
                {
                    yield return next;
                }
                else
                {
                    foreach (var x in GetDeepDeps(next, configs))
                        yield return x;
                }
            }
    }

    Dictionary<string, PropertyConfig<TEntity>> SortProps(Dictionary<string, PropertyConfig<TEntity>> configProps)
    {
        if (configProps.Count == 0)
            return configProps;

        var sorted = new Dictionary<string, PropertyConfig<TEntity>>();

        var left = configProps.Values.ToList()
            .Where(x => !sorted.ContainsKey(x.Path));

        foreach (var x in left)
            if (!x.Dependencies.Any(xx => xx.Value && configProps.ContainsKey(xx.Key)))
                sorted.Add(x.Path, x);

        if (!sorted.Any())
            throw new Exception($"[{typeof(TEntity).FullName}] No properties found without dependencies.");

        bool added;

        do
        {
            added = false;
            foreach (var x in left)
                if (!x.Dependencies.Any(xx => xx.Value && configProps.ContainsKey(xx.Key) && !sorted.ContainsKey(xx.Key)))
                {
                    sorted.Add(x.Path, x);
                    added = true;
                }
        } while (added);

        if (left.Any())
            throw new Exception($"[{typeof(TEntity).FullName}] Circular dependencies in properties: {string.Join(", ", left.Select(x => x.Path))}");

        return sorted;
    }

}
