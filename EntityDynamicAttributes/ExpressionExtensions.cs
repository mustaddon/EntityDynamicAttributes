using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EntityDynamicAttributes;

internal static class ExpressionExtensions
{
    public static string GetMemberPath(this Expression expression)
    {
        return GetMemberInfo(expression is LambdaExpression lambda ? lambda.Body : expression)?.Path
            ?? throw new ArgumentException($"The expression [{expression}] does not refer to a property or field.");
    }

    public static (string Path, Type Param)? GetMemberInfo(this Expression? expression)
    {
        var names = new List<string>();

        while (expression is MemberExpression member)
        {
            names.Insert(0, member.Member.Name);
            expression = member.Expression;
        }

        if (expression is ParameterExpression parameter)
            return (string.Join(".", names), parameter.Type);

        return null;
    }
}
