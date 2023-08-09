using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityDynamicAttributes;

public class PropertyConfig<TEntity, TProp> : PropertyConfig<TEntity>
{
    public PropertyConfig(TypeConfig<TEntity> typeConfig, Expression<Func<TEntity, TProp>> expression)
        : this(typeConfig, expression.GetMemberPath())
    { }

    internal PropertyConfig(TypeConfig<TEntity> typeConfig, string path)
        : base(path, typeof(TProp))
    {
        End = typeConfig;
    }

    public readonly TypeConfig<TEntity> End;

    public PropertyConfig<TEntity, TProp> Dependency<T>(Expression<Func<TEntity, T>> propertyExpression, bool computed = false)
    {
        AddDependency(propertyExpression.GetMemberPath(), computed);
        return this;
    }

    public PropertyConfig<TEntity, TProp> Attribute(string key, Expression<Func<IContext<TEntity, TProp>, object?>>? expression = null)
    {
        AddDependencies(expression);
        Attributes.AddOrUpdate(key, (expression ?? (x => true)).Compile());
        return this;
    }

    public PropertyConfig<TEntity, TProp> Error(string key, Expression<Func<IContext<TEntity, TProp>, object?>> expression)
    {
        AddDependencies(expression);
        Errors.AddOrUpdate(key, expression.Compile());
        return this;
    }

    void AddDependencies(Expression? expression, bool deep = false)
    {
        if (expression == null)
            return;

        if (expression is LambdaExpression lambda)
            expression = lambda.Body;

        if (expression is MemberExpression member)
        {
            var info = expression.GetMemberInfo();

            if (info == null)
                return;

            if (info.Value.Param == typeof(TEntity))
            {
                AddDependency(info.Value.Path, deep);
            }
            else if (info.Value.Param == typeof(IContext<TEntity, TProp>))
            {
                if (info.Value.Path.StartsWith(Names.ContextValue))
                    AddDependency(Path + info.Value.Path.Substring(Names.ContextValue.Length), deep);
                else if (info.Value.Path.StartsWith(Names.ContextEntity))
                    AddDependency(info.Value.Path.Length > Names.ContextEntity.Length
                        ? info.Value.Path.Substring(Names.ContextEntity.Length + 1)
                        : string.Empty,
                        deep);
            }
        }
        else if (expression is ParameterExpression parameter)
        {
            if (parameter.Type == typeof(TEntity))
                AddDependency(string.Empty, deep);
        }
        else if (expression is BinaryExpression binary)
        {
            AddDependencies(binary.Left, deep);
            AddDependencies(binary.Right, deep);
        }
        else if (expression is MethodCallExpression method)
        {
            if (!deep)
                deep = method.Object?.Type == typeof(IContext<TEntity, TProp>)
                    || method.Arguments.Any(x => x.Type == typeof(IContext<TEntity, TProp>));

            foreach (var arg in method.Arguments)
                AddDependencies(arg, deep);

            if (method.Object != null)
                AddDependencies(method.Object, deep);
        }
        else if (expression is UnaryExpression unary)
        {
            AddDependencies(unary.Operand, deep);
        }
    }

    void AddDependency(string path, bool deep = false)
    {
        if (Path == path)
            return;

        if (!Dependencies.TryGetValue(path, out var exist))
            Dependencies.Add(path, deep);
        else if (deep && !exist)
            Dependencies[path] = deep;
    }
}


public class PropertyConfig<TEntity>
{
    internal PropertyConfig(string path, Type type)
    {
        Path = path;
        Type = type;
    }

    internal readonly string Path;
    internal readonly Type Type;
    internal readonly Dictionary<string, bool> Dependencies = new();
    internal readonly Dictionary<string, Delegate> Attributes = new();
    internal readonly Dictionary<string, Delegate> Errors = new();

    internal bool IsEmpty => Dependencies.Count == 0 && Attributes.Count == 0 && Errors.Count == 0;
}