using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using ServiceProviderEndpoint;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityDynamicAttributes;

public static class IEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Adds EntityDynamicAttributes as <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that handles HTTP requests.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="route">The route pattern.</param>
    /// <param name="additionalTypes">Object types for deserializing, casting and extensions.</param>
    /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
    public static IEndpointConventionBuilder MapEntityDynamicAttributes(this IEndpointRouteBuilder builder, string route = "eda", IEnumerable<Type>? additionalTypes = null, Action<SpeOptions>? optionsConfigurator = null)
    {
        return builder.MapServiceProvider(route,
            serviceTypes: new[] { typeof(ISchemaBuilder<>) },
            extensions: IServiceCollectionExtensions.RegisteredEntityTypes
                .Concat(additionalTypes ?? Array.Empty<Type>()),
            optionsConfigurator: optionsConfigurator);
    }
}
