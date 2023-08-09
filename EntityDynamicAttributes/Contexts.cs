using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace EntityDynamicAttributes;

internal sealed class SubPropContext<TEntity, TProp, TSub> : IContext<TSub, TProp>
{
    internal SubPropContext(IContext<TEntity, TProp> context, Func<TEntity, TSub> switcher, string path)
    {
        _context = context;
        _switcher = switcher;
        _path = path;
    }

    private readonly IContext<TEntity, TProp> _context;
    private readonly Func<TEntity, TSub> _switcher;
    private readonly string _path;

    public TProp? Value => _context.Value;
    public TSub Entity => _switcher(_context.Entity);
    public IServiceProvider ServiceProvider => _context.ServiceProvider;
    public CancellationToken CancellationToken => _context.CancellationToken;

    public IReadOnlyDictionary<string, object> Attributes() => _context.Attributes();
    public IReadOnlyDictionary<string, object> Attributes<TargetProp>(Expression<Func<TSub, TargetProp>> propertyExpression) => Attributes(propertyExpression.GetMemberPath());
    public IReadOnlyDictionary<string, object> Attributes(string propertyPath) => _context.Attributes(GetFullPath(propertyPath));

    public IReadOnlyDictionary<string, object> Errors() => _context.Errors();
    public IReadOnlyDictionary<string, object> Errors<TargetProp>(Expression<Func<TSub, TargetProp>> propertyExpression) => Errors(propertyExpression.GetMemberPath());
    public IReadOnlyDictionary<string, object> Errors(string propertyPath) => _context.Errors(GetFullPath(propertyPath));

    string GetFullPath(string propertyPath) => string.IsNullOrEmpty(propertyPath) ? _path : string.Concat(_path, ".", propertyPath);
}


internal sealed class PropContext<TEntity, TProp> : IContext<TEntity, TProp>
{
    internal PropContext(CommonContext<TEntity> commonContext,
        Func<object?> value, PropertySchema schema)
    {
        _commonContext = commonContext;
        _value = new Lazy<TProp?>(() => (TProp?)value());
        _schema = schema;
    }

    private readonly CommonContext<TEntity> _commonContext;
    private readonly Lazy<TProp?> _value;
    private readonly PropertySchema _schema;

    public TProp? Value => _value.Value;
    public TEntity Entity => _commonContext.Entity;
    public IServiceProvider ServiceProvider => _commonContext.ServiceProvider;
    public CancellationToken CancellationToken => _commonContext.CancellationToken;

    public IReadOnlyDictionary<string, object> Attributes() => _schema.Attributes;
    public IReadOnlyDictionary<string, object> Attributes<TargetProp>(Expression<Func<TEntity, TargetProp>> propertyExpression) => Attributes(propertyExpression.GetMemberPath());
    public IReadOnlyDictionary<string, object> Attributes(string propertyPath)
    {
        return _commonContext.Schema.Properties.TryGetValue(propertyPath, out var exist)
            ? exist.Attributes
            : Empty.ReadOnlyDictionary;
    }

    public IReadOnlyDictionary<string, object> Errors() => _schema.Errors;
    public IReadOnlyDictionary<string, object> Errors<TargetProp>(Expression<Func<TEntity, TargetProp>> propertyExpression) => Errors(propertyExpression.GetMemberPath());
    public IReadOnlyDictionary<string, object> Errors(string propertyPath)
    {
        return _commonContext.Schema.Properties.TryGetValue(propertyPath, out var exist)
            ? exist.Errors
            : Empty.ReadOnlyDictionary;
    }
}


internal sealed class CommonContext<TEntity>
{
    internal CommonContext(TEntity entity,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        Entity = entity;
        ServiceProvider = serviceProvider;
        CancellationToken = cancellationToken;
    }

    public readonly Schema Schema = new();
    public readonly TEntity Entity;
    public readonly IServiceProvider ServiceProvider;
    public readonly CancellationToken CancellationToken;


    public object CreateContext(Type type, Func<object?> value, PropertySchema schema)
    {
        return _contextMethod.MakeGenericMethod(type).Invoke(this, new object[] { value, schema })!;
    }

    static readonly MethodInfo _contextMethod = typeof(CommonContext<TEntity>).GetMethod(nameof(Context), BindingFlags.NonPublic | BindingFlags.Instance)!;
    PropContext<TEntity, TProp> Context<TProp>(Func<object?> value, PropertySchema schema) => new(this, value, schema);
}