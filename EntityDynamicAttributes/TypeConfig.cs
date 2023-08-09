using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityDynamicAttributes;


public class TypeConfig<TEntity> : ITypeConfig<TEntity>
{
    internal readonly Dictionary<string, PropertyConfig<TEntity>> Properties = new();

    public PropertyConfig<TEntity, TProp> Property<TProp>(Expression<Func<TEntity, TProp>> propertyExpression, TypeConfig<TProp>? subConfig = null)
    {
        var config = (PropertyConfig<TEntity, TProp>)Properties.GetOrAdd(
            propertyExpression.GetMemberPath(), 
            k => new PropertyConfig<TEntity, TProp>(this, k));

        if (subConfig != null)
            foreach (var source in subConfig.Properties.Values)
            {
                var subHelper = new SubConfigHelper<TEntity>(typeof(TProp), source.Type, config.Path);
                var target = Properties.GetOrAdd(
                    string.IsNullOrEmpty(source.Path) ? config.Path : string.Concat(config.Path, ".", source.Path),
                    k => subHelper.CreateProperty(this, k));

                foreach (var kvp in source.Dependencies)
                    target.Dependencies.Add(string.IsNullOrEmpty(kvp.Key) ? config.Path : string.Concat(config.Path, ".", kvp.Key), kvp.Value);

                foreach (var kvp in source.Attributes)
                    target.Attributes.Add(kvp.Key, subHelper.WrapDelegate(kvp.Value));

                foreach (var kvp in source.Errors)
                    target.Errors.Add(kvp.Key, subHelper.WrapDelegate(kvp.Value));
            }

        return config;
    }



    internal TypeSettings<TEntity> Settings()
    {
        if (_settings != null)
            return _settings;

        lock (_locker)
        {
            _settings ??= new TypeSettings<TEntity>(this);
            return _settings;
        }
    }

    private TypeSettings<TEntity>? _settings;
    private readonly object _locker = new();
}
