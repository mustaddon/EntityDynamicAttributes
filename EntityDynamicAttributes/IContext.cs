using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace EntityDynamicAttributes;

public interface IContext<TEntity, TProp>
{
    TProp? Value { get; }
    TEntity Entity { get; }
    IServiceProvider ServiceProvider { get; }
    CancellationToken CancellationToken { get; }

    IReadOnlyDictionary<string, object> Attributes();
    IReadOnlyDictionary<string, object> Attributes(string propertyPath);
    IReadOnlyDictionary<string, object> Attributes<TargetProp>(Expression<Func<TEntity, TargetProp>> propertyExpression);

    IReadOnlyDictionary<string, object> Errors();
    IReadOnlyDictionary<string, object> Errors(string propertyPath);
    IReadOnlyDictionary<string, object> Errors<TargetProp>(Expression<Func<TEntity, TargetProp>> propertyExpression);
}
