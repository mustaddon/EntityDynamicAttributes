using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EntityDynamicAttributes;

public interface ISchemaBuilder<TEntity>
{
    Task<Schema> Build(TEntity entity, string[]? onlyForProperties = null, CancellationToken cancellationToken = default);
}

public class Schema
{
    public readonly Dictionary<string, PropertySchema> Properties = new();
}

public class PropertySchema
{
    public PropertySchema(string path, IReadOnlyCollection<string> deps)
    {
        Path = path;
        Dependencies = deps;
    }

    public readonly string Path;
    public readonly IReadOnlyCollection<string> Dependencies;
    public readonly Dictionary<string, object> Attributes = new();
    public readonly Dictionary<string, object> Errors = new();
}
