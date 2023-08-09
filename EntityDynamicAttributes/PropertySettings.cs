using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace EntityDynamicAttributes;

internal class PropertySettings<TEntity>
{
    public PropertySettings(PropertyConfig<TEntity> prop, Dictionary<string, HashSet<string>> deepDeps)
    {
        Type = prop.Type;
        Path = prop.Path;
        Getter = CreateGetter(prop.Path.Split('.'));

        DeepRelations = new ReadOnlyCollection<string>(new HashSet<string>(deepDeps.Where(xx => xx.Key != prop.Path && xx.Value.Contains(prop.Path)).Select(xx => xx.Key)));

        IsEmpty = prop.IsEmpty && DeepRelations.Count == 0;

        if (prop.IsEmpty)
        {
            Attributes = Empty.ReadOnlyDictionaryDelegate;
            Errors = Empty.ReadOnlyDictionaryDelegate;
            AggregatedDependencies = Empty.ReadOnlyCollection;
        }
        else
        {
            Attributes = new ReadOnlyDictionary<string, Delegate>(prop.Attributes.ToDictionary(x => x.Key, x => x.Value));
            Errors = new ReadOnlyDictionary<string, Delegate>(prop.Errors.ToDictionary(x => x.Key, x => x.Value));
            AggregatedDependencies = new ReadOnlyCollection<string>(new HashSet<string>(deepDeps[prop.Path]
                .Concat(DeepRelations.Where(deepDeps.ContainsKey).SelectMany(x => deepDeps[x]))
                .Where(x => x != prop.Path)));
        }
    }

    public readonly bool IsEmpty;
    public readonly Type Type;
    public readonly string Path;
    public readonly Func<TEntity, object?> Getter;
    public readonly IReadOnlyCollection<string> AggregatedDependencies;
    public readonly IReadOnlyCollection<string> DeepRelations;
    public readonly IReadOnlyDictionary<string, Delegate> Attributes;
    public readonly IReadOnlyDictionary<string, Delegate> Errors;


    static Func<TEntity, object?> CreateGetter(string[] pathParts)
    {
        var result = new Func<TEntity, object?>(x => x);
        var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var currentType = typeof(TEntity);

        for (var i = 0; i < pathParts.Length; i++)
        {
            var part = pathParts[i];

            if (string.IsNullOrEmpty(part))
                continue;

            var propInfo = currentType.GetProperty(part, bindingFlags);

            if (propInfo != null)
            {
                result = Append(result, propInfo.PropertyType, propInfo.GetValue);
                currentType = propInfo.PropertyType;
                continue;
            }

            var fieldInfo = currentType.GetField(part, bindingFlags);

            if (fieldInfo != null)
            {
                result = Append(result, fieldInfo.FieldType, fieldInfo.GetValue);
                currentType = fieldInfo.FieldType;
                continue;
            }

            throw new Exception($"Property or field [{string.Join(".", pathParts.Take(i + 1))}] not found in [{currentType.FullName}]");
        }

        return result;
    }

    static Func<TEntity, object?> Append(Func<TEntity, object?> source, Type returnType, Func<object?, object?> fn)
    {
        return x =>
        {
            if (x is null)
                return returnType.DefaultValue();

            var sourceVal = source(x);

            if (sourceVal is null)
                return returnType.DefaultValue();

            return fn(sourceVal);
        };
    }
}
