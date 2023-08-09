using Microsoft.Extensions.DependencyInjection;
using EntityDynamicAttributes;
using Example;
using System.Text.Json;
using System.Text.Json.Serialization;

var services = new ServiceCollection()
    .AddEntityDynamicAttributes(typeof(MyUserConfig).Assembly)
    .BuildServiceProvider();

var myUserSchema = await services.GetRequiredService<ISchemaBuilder<MyUser>>()
    .Build(new MyUser());

Console.WriteLine(JsonSerializer.Serialize(myUserSchema, new JsonSerializerOptions { 
    ReferenceHandler = ReferenceHandler.IgnoreCycles,
    IncludeFields = true,
    PropertyNameCaseInsensitive = true,
    WriteIndented = true,
}));