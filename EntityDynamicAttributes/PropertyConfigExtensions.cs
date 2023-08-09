using System;
using System.Collections;
using System.Linq.Expressions;

namespace EntityDynamicAttributes;

public static class PropertyConfigExtensions
{
    public static PropertyConfig<TEntity, TProp> Hidden<TEntity, TProp>(this PropertyConfig<TEntity, TProp> cfg,
        Expression<Func<IContext<TEntity, TProp>, object?>>? expression = null)
    {
        return cfg.Attribute(Attributes.Hidden, expression);
    }

    public static PropertyConfig<TEntity, TProp> Disabled<TEntity, TProp>(this PropertyConfig<TEntity, TProp> cfg,
        Expression<Func<IContext<TEntity, TProp>, object?>>? expression = null)
    {
        return cfg.Attribute(Attributes.Disabled, expression);
    }

    public static PropertyConfig<TEntity, TProp> Readonly<TEntity, TProp>(this PropertyConfig<TEntity, TProp> cfg,
        Expression<Func<IContext<TEntity, TProp>, object?>>? expression = null)
    {
        return cfg.Attribute(Attributes.Readonly, expression);
    }

    public static PropertyConfig<TEntity, TProp> Required<TEntity, TProp>(this PropertyConfig<TEntity, TProp> cfg,
        object? errorValue)
    {
        return cfg.Required(x => true, errorValue);
    }

    public static PropertyConfig<TEntity, TProp> Required<TEntity, TProp>(this PropertyConfig<TEntity, TProp> cfg,
        Expression<Func<IContext<TEntity, TProp>, object?>>? expression = null,
        object? errorValue = null)
    {
        cfg.Attribute(Attributes.Required, expression);

        errorValue ??= true;

        return cfg.Error(Attributes.Required, x => x.Attributes().ContainsKey(Attributes.Required) && IsEmpty(x.Value) ? errorValue : null);
    }

    static bool IsEmpty<TProp>(TProp value)
    {
        return value == null ? true
            : value is string str ? string.IsNullOrWhiteSpace(str)
            : value is IEnumerable many ? !many.GetEnumerator().MoveNext()
            : false;
    }
}

