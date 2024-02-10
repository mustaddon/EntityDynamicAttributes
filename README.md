# EntityDynamicAttributes [![NuGet version](https://badge.fury.io/nu/EntityDynamicAttributes.svg?)](http://badge.fury.io/nu/EntityDynamicAttributes)

## Features
* Lambda expressions
* Property dependencies autodetection 
* Sub-object property configurations
* Async attribute values support
* WebApi endpoint ready


## Create configuration
```C#
public class MyClassConfig : TypeConfig<MyClass>
{
    public MyClassConfig()
    {
        Property(x => x.SomeProp1)
            .Attribute("static_attr")
            .Attribute("expression_attr", x => x.Entity.SomeProp2 > 0)
            .Attribute("value_attr", x => 100 * x.Entity.SomeProp3)
            .Attribute("task_value_attr", x => x.ServiceProvider.GetRequiredService<MyService>().AsyncMethod(x.CancellationToken));

        Property(x => x.ComplexProp.SubProp1)
            .Required(x => x.Entity.SomeProp2 > 0)
            .Hidden(x => !x.Entity.SomeProp3);
    }
}
```


## Add EDA services to your ServiceCollection
```C#
using EntityDynamicAttributes;

builder.Services.AddEntityDynamicAttributes(typeof(MyClassConfig).Assembly);
```


## Build schema with ISchemaBuilder
```C#
var schema = await services.GetRequiredService<ISchemaBuilder<MyClass>>()
    .Build(new MyClass());
```


## Example projects
* [ConsoleApp](https://github.com/mustaddon/EntityDynamicAttributes/tree/main/Examples/Example.ConsoleApp)
* [WebApi](https://github.com/mustaddon/EntityDynamicAttributes/tree/main/Examples/Example.WebApi)

