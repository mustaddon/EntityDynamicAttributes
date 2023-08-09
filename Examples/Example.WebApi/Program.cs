using EntityDynamicAttributes;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEntityDynamicAttributes(typeof(Example.MyUserConfig).Assembly);


var app = builder.Build();

// Endpoint
app.MapEntityDynamicAttributes();

app.Run();