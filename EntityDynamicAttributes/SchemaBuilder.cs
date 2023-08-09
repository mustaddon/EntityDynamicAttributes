using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EntityDynamicAttributes;

public class SchemaBuilder<TEntity> : ISchemaBuilder<TEntity>
{
    public SchemaBuilder(TypeConfig<TEntity> config, IServiceProvider serviceProvider)
    {
        _config = config;
        _serviceProvider = serviceProvider;
    }

    private readonly TypeConfig<TEntity> _config;
    private readonly IServiceProvider _serviceProvider;

    public Task<Schema> Build(TEntity entity, string[]? onlyForProperties = null, CancellationToken cancellationToken = default)
    {
        var ctx = new CommonContext<TEntity>(entity, _serviceProvider, cancellationToken);

        if (onlyForProperties == null)
            return Build(ctx);

        if (onlyForProperties.Length == 1)
            return Build(ctx, onlyForProperties[0]);

        return Build(ctx, onlyForProperties);
    }

    private async Task<Schema> Build(CommonContext<TEntity> ctx)
    {
        var settings = _config.Settings();

        foreach (var x in settings.Properties.Values)
            if (!x.IsEmpty)
                await AddProperty(ctx, x);

        return ctx.Schema;
    }

    private async Task<Schema> Build(CommonContext<TEntity> ctx, string prop)
    {
        var settings = _config.Settings();

        if (settings.Properties.TryGetValue(prop, out var propSettings))
        {
            if (!propSettings.IsEmpty)
                await AddProperty(ctx, propSettings);

            foreach (var rel in propSettings.DeepRelations)
            {
                var relSettings = settings.Properties[rel];

                if (!relSettings.IsEmpty)
                    await AddProperty(ctx, relSettings);
            }
        }

        return ctx.Schema;
    }

    private async Task<Schema> Build(CommonContext<TEntity> ctx, string[] props)
    {
        var settings = _config.Settings();

        var paths = new HashSet<string>(props
            .Where(settings.Properties.ContainsKey)
            .SelectMany(x => PathAndRelations(settings.Properties[x])));

        foreach (var x in settings.Properties.Values)
            if (!x.IsEmpty && paths.Contains(x.Path))
                await AddProperty(ctx, x);

        return ctx.Schema;
    }

    private static IEnumerable<string> PathAndRelations(PropertySettings<TEntity> settings)
    {
        yield return settings.Path;

        foreach (var rel in settings.DeepRelations)
            yield return rel;
    }

    private static async Task AddProperty(CommonContext<TEntity> ctx, PropertySettings<TEntity> propSettings)
    {
        var propSchema = new PropertySchema(propSettings.Path, propSettings.AggregatedDependencies);
        var propCtx = ctx.CreateContext(propSettings.Type, () => propSettings.Getter(ctx.Entity), propSchema);

        foreach (var kvp in propSettings.Attributes)
        {
            var attrVal = await kvp.Value.DynamicInvokeAdvanced(propCtx);

            if (attrVal != null && !(attrVal is bool boolVal && boolVal == false))
                propSchema.Attributes.Add(kvp.Key, attrVal);
        }

        foreach (var kvp in propSettings.Errors)
        {
            if (propSchema.Attributes.ContainsKey(Attributes.Hidden))
                continue;

            var errorVal = await kvp.Value.DynamicInvokeAdvanced(propCtx);

            if (errorVal != null && !(errorVal is bool boolVal && boolVal == false))
                propSchema.Errors.Add(kvp.Key, errorVal);
        }

        ctx.Schema.Properties.Add(propSettings.Path, propSchema);
    }
}
