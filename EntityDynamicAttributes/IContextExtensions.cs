using System;
using System.Linq.Expressions;

namespace EntityDynamicAttributes;


public static class IContextExtensions
{
    public static object? Error<TEntity, TProp>(this IContext<TEntity, TProp> ctx, string key)
    {
        return ctx.Errors().TryGetValue(key, out var val) ? val : null;
    }

    public static object? Error<TEntity, TProp, TargetProp>(this IContext<TEntity, TProp> ctx, Expression<Func<TEntity, TargetProp>> propertyExpression, string key)
    {
        return ctx.Errors(propertyExpression).TryGetValue(key, out var val) ? val : null;
    }

    public static bool HasError<TEntity, TProp>(this IContext<TEntity, TProp> ctx, string key)
    {
        return ctx.Errors().ContainsKey(key);
    }

    public static bool HasError<TEntity, TProp, TargetProp>(this IContext<TEntity, TProp> ctx, Expression<Func<TEntity, TargetProp>> propertyExpression, string key)
    {
        return ctx.Errors(propertyExpression).ContainsKey(key);
    }

    public static object? Attribute<TEntity, TProp>(this IContext<TEntity, TProp> ctx, string key)
    {
        return ctx.Attributes().TryGetValue(key, out var val) ? val : null;
    }

    public static object? Attribute<TEntity, TProp, TargetProp>(this IContext<TEntity, TProp> ctx, Expression<Func<TEntity, TargetProp>> propertyExpression, string key)
    {
        return ctx.Attributes(propertyExpression).TryGetValue(key, out var val) ? val : null;
    }

    public static bool HasAttribute<TEntity, TProp>(this IContext<TEntity, TProp> ctx, string key)
    {
        return ctx.Attributes().ContainsKey(key);
    }

    public static bool HasAttribute<TEntity, TProp, TargetProp>(this IContext<TEntity, TProp> ctx, Expression<Func<TEntity, TargetProp>> propertyExpression, string key)
    {
        return ctx.Attributes(propertyExpression).ContainsKey(key);
    }

    public static bool IsHidden<TEntity, TProp>(this IContext<TEntity, TProp> ctx)
    {
        return ctx.HasAttribute(Attributes.Hidden);
    }

    public static bool IsHidden<TEntity, TProp, TargetProp>(this IContext<TEntity, TProp> ctx, Expression<Func<TEntity, TargetProp>> propertyExpression)
    {
        return ctx.HasAttribute(propertyExpression, Attributes.Hidden);
    }

    public static bool IsDisabled<TEntity, TProp>(this IContext<TEntity, TProp> ctx)
    {
        return ctx.HasAttribute(Attributes.Disabled);
    }

    public static bool IsDisabled<TEntity, TProp, TargetProp>(this IContext<TEntity, TProp> ctx, Expression<Func<TEntity, TargetProp>> propertyExpression)
    {
        return ctx.HasAttribute(propertyExpression, Attributes.Disabled);
    }

    public static bool IsReadonly<TEntity, TProp>(this IContext<TEntity, TProp> ctx)
    {
        return ctx.HasAttribute(Attributes.Readonly);
    }

    public static bool IsReadonly<TEntity, TProp, TargetProp>(this IContext<TEntity, TProp> ctx, Expression<Func<TEntity, TargetProp>> propertyExpression)
    {
        return ctx.HasAttribute(propertyExpression, Attributes.Readonly);
    }

    public static bool IsRequired<TEntity, TProp>(this IContext<TEntity, TProp> ctx)
    {
        return ctx.HasAttribute(Attributes.Required);
    }

    public static bool IsRequired<TEntity, TProp, TargetProp>(this IContext<TEntity, TProp> ctx, Expression<Func<TEntity, TargetProp>> propertyExpression)
    {
        return ctx.HasAttribute(propertyExpression, Attributes.Required);
    }
}
