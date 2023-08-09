using System;
using System.Linq;
using System.Reflection;

namespace EntityDynamicAttributes;

public static class TypeExtensions
{
    public static bool IsEdaConfig(this Type type, out Type? entityType)
    {
        if (!type.IsClass || type.IsAbstract)
        {
            entityType = null;
            return false;
        }

        entityType = type.GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType && !x.IsGenericTypeDefinition && x.GetGenericTypeDefinition() == _typeConfigInterface)
            ?.GetGenericArguments().First();

        return entityType != null;
    }

    internal static object? DefaultValue(this Type type)
    {
        if (type.IsClass)
            return null;

        return _defaultMethod.MakeGenericMethod(type).Invoke(null, Array.Empty<object>());
    }

    private static T? Default<T>() => default;

    private static readonly MethodInfo _defaultMethod = typeof(TypeExtensions).GetMethod(nameof(Default), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly Type _typeConfigInterface = typeof(ITypeConfig<>);
}
