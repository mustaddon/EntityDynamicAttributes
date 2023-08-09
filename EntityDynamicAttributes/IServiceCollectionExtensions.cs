using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityDynamicAttributes;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddEntityDynamicAttributes(this IServiceCollection serviceCollection, params Assembly[] assembliesWithConfigs)
    {
        return AddEntityDynamicAttributes(serviceCollection, assembliesWithConfigs.SelectMany(x => x.GetTypes()));
    }

    public static IServiceCollection AddEntityDynamicAttributes(this IServiceCollection serviceCollection, IEnumerable<Type> configTypes)
    {
        foreach (var type in configTypes)
            if (type.IsEdaConfig(out Type? entityType))
            {
                serviceCollection.AddSingleton(typeof(TypeConfig<>).MakeGenericType(entityType!), type);
                RegisteredEntityTypes.Add(entityType!);
            }

        serviceCollection.AddTransient(typeof(ISchemaBuilder<>), typeof(SchemaBuilder<>));

        return serviceCollection;
    }


    public static readonly HashSet<Type> RegisteredEntityTypes = new();
}
