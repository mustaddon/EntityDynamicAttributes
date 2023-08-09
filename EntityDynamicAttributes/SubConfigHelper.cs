using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityDynamicAttributes;

internal class SubConfigHelper<TEntity>
{
    public SubConfigHelper(Type subEntity, Type subType, string pathEntity)
    {
        var param = Expression.Parameter(typeof(TEntity), "x");
        var body = pathEntity.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);
        _switcher = Expression.Lambda(body, param).Compile();
        _wrapperGenericMethod = _wrapperMethod.MakeGenericMethod(subEntity, subType);
        _pathEntity = pathEntity;
        _subType = subType;
    }

    private readonly Type _subType;
    private readonly Delegate _switcher;
    private readonly MethodInfo _wrapperGenericMethod;
    private readonly string _pathEntity;

    public PropertyConfig<TEntity> CreateProperty(TypeConfig<TEntity> cfg, string path)
    {
        return (PropertyConfig<TEntity>)_propertyMethod
            .MakeGenericMethod(_subType)
            .Invoke(null, new object[] { cfg, path })!;
    }

    private static readonly MethodInfo _propertyMethod = typeof(SubConfigHelper<TEntity>).GetMethod(nameof(PropertyFn), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static PropertyConfig<TEntity> PropertyFn<TProp>(TypeConfig<TEntity> cfg, string path) => new PropertyConfig<TEntity, TProp>(cfg, path);


    public Delegate WrapDelegate(Delegate subDelegate)
    {
        return (Delegate)_wrapperGenericMethod.Invoke(this, new object[] { subDelegate })!;
    }

    private static readonly MethodInfo _wrapperMethod = typeof(SubConfigHelper<TEntity>).GetMethod(nameof(Wrapper), BindingFlags.NonPublic | BindingFlags.Instance)!;
    private Func<IContext<TEntity, TProp>, object?> Wrapper<TSub, TProp>(Func<IContext<TSub, TProp>, object?> subDelegate)
    {
        return x => subDelegate(new SubPropContext<TEntity, TProp, TSub>(x, (Func<TEntity, TSub>)_switcher, _pathEntity));
    }
}
